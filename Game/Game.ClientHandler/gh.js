/*
** Game client handler for CodeWars 2016 server
** USAGE: node gh.js profile.js
** See sample.js for profile example.
*/

var fs = require('fs');
var crypto = require('crypto');
var request = require('request');
var child_process = require('child_process');


if (process.argv.length != 3) {
    console.error("USAGE: node gh.js profile.js");
    process.exit(1);
}


var profileName = process.argv[2];
var profile = {};
var server;
var handler;

readProfile();
setupSession();


//process.exit(0);

function readProfile() {
    console.log("Reading profile " + profileName);
    var data = fs.readFileSync(profileName, "utf8");
    data = data.replace(/^\uFEFF/, ""); // strip BOM
    profile = JSON.parse(data);
    if (typeof profile.serverUri === "undefined")
        throw "serverUri not specified";
    if (typeof profile.teamName === "undefined")
        throw "teamName not specified";
    if (typeof profile.teamPassword === "undefined")
        throw "teamPassword not specified";
    if (typeof profile.clientName === "undefined")
        throw "clientName not specified";
    if (typeof profile.clientType === "undefined")
        throw "clientType not specified";
    if (typeof profile.sessionDir === "undefined")
        profile.sessionDir = "sessions";

    console.log("serverUri: " + profile.serverUri);
    console.log("teamName: " + profile.teamName);
    console.log("clientName: " + profile.clientName);
    console.log("sessionDir: " + profile.sessionDir);

    if (profile.clientType == "exec") {
        if (typeof profile.execName === "undefined")
            throw "execName not specified";
        handler = execHandler;
        console.log("clientType: exec " + profile.execName);
    }
    else {
        throw "clientType not supported - " + profile.clientType;
    }
}

function setupSession() {
    console.log("Setting up session...");
    var sessionId = Math.floor(Math.random() * 2147483640 + 1);
    server = new ServerSide(profile, sessionId);
    server.createPlayer(function () {
        profile.sessionLogDir = profile.sessionDir + "/" + sessionId;
        if (!fs.existsSync(profile.sessionDir))
            fs.mkdirSync(profile.sessionDir);
        if (!fs.existsSync(profile.sessionLogDir))
            fs.mkdirSync(profile.sessionLogDir);
        turnLoop();
    });
}

function turnLoop() {
    server.waitNextTurn(function () {
        server.getPlayerView(function (view) {
            handler(view, function (move) {
                server.performMove(move, function () {
                    turnLoop();
                });
            });
        });
    });
}

function execHandler(view, next) {
    delete view.isOk;
    delete view.Status;
    delete view.Message;
    var inputFile = profile.sessionLogDir + "/" + server.refTurn + "_view.json";
    var outputFile = profile.sessionLogDir + "/" + server.refTurn + "_move.json";
    fs.writeFileSync(inputFile, JSON.stringify(view));
    child_process.spawnSync(profile.execName, [inputFile, outputFile]);
    var data = fs.readFileSync(outputFile, "utf8");
    data = data.replace(/^\uFEFF/, ""); // strip BOM
    var move = JSON.parse(data);
    next(move);
}

function ServerSide(profile, sessionId) {
    console.log("sessionId: " + sessionId);

    this.serverUri = profile.serverUri;
    this.teamName = profile.teamName;
    this.teamPassword = profile.teamPassword;
    this.clientName = profile.clientName;
    this.sessionId = sessionId;
    this.sequenceNumber = 1;

    this._performCall = function (serviceName, req, handler) {
        var auth = {};
        auth.TeamName = this.teamName;
        auth.ClientName = this.clientName;
        auth.SessionId = this.sessionId;
        auth.SequenceNumber = this.sequenceNumber;
        auth.AuthCode = this._calcAuthCode(auth.TeamName + ":" + auth.ClientName + ":" + auth.SessionId + ":" + auth.SequenceNumber + this.teamPassword);
        req.Auth = auth;

        this.sequenceNumber++;

        var url = this.serverUri + "/json/" + serviceName;

        //console.log("CALL " + serviceName);
        //console.log("REQ " + JSON.stringify(req));

        request({
            url: url,
            method: "POST",
            json: req
        }, function (error, response, body) {
            var resp;
            if (!error && response.statusCode == 200) {
                resp = body;
                resp.isOk = (resp.Status == "OK");
            }
            else {
                resp = {
                    isOk: false,
                    Status: "ERROR",
                    Message: (error) ? error : "Status " + response.statusCode
                };
            }
            //console.log("RESP " + JSON.stringify(resp));
            handler(resp);
        });
    };

    this._calcAuthCode = function (authstr) {
        var md = crypto.createHash("sha1");
        md.update(authstr);
        return md.digest("hex");
    };

    this.createPlayer = function (next) {
        var thisServer = this;
        this._performCall("CreatePlayer", {}, function (resp) {
            if (!resp.isOk)
                throw "CreatePlayer call failed: " + resp.Status + " " + resp.Message;
            thisServer.playerId = resp.PlayerId;
            thisServer.refTurn = 0;
            console.log("PlayerId: " + thisServer.playerId);
            next();
        });
    };

    this.waitNextTurn = function (next) {
        var thisServer = this;
        this._performCall("WaitNextTurn", { PlayerId: thisServer.playerId, RefTurn: thisServer.refTurn }, function (resp) {
            if (!resp.isOk)
                throw "WaitNextTurn call failed: " + resp.Status + " " + resp.Message;
            if (resp.GameFinished) {
                console.log("GAME FINISHED: " + resp.FinishCondition + " " + resp.FinishComment);
                thisServer.refTurn = 0;
                thisServer.waitNextTurn(next);
                return;
            }
            if (!resp.TurnComplete) {
                thisServer.waitNextTurn(next);
                return;
            }
            else {
                next();
            }
        });
    };

    this.getPlayerView = function (next) {
        var thisServer = this;
        this._performCall("GetPlayerView", { PlayerId: thisServer.playerId }, function (resp) {
            if (!resp.isOk)
                throw "GetPlayerView call failed: " + resp.Status + " " + resp.Message;
            thisServer.refTurn = resp.Turn;
            next(resp);
        });
    };

    this.performMove = function (move, next) {
        move.PlayerId = this.playerId;
        this._performCall("PerformMove", move, function (resp) {
            if (!resp.isOk)
                throw "PerformMove call failed: " + resp.Status + " " + resp.Message;
            next();
        });
    };
}
