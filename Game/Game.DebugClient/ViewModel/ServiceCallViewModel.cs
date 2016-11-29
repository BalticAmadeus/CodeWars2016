using System.ComponentModel;
using Game.DebugClient.Infrastructure;
using Game.ClientCommon.Infrastructure;
using Game.ClientCommon.Utilites;
using Prism.Mvvm;
using System;

namespace Game.DebugClient.ViewModel
{
    public abstract class ServiceCallViewModel : BindableBase
    {
        protected readonly ICommonDataManager CommonDataManager;
        protected readonly IServiceCallInvoker ServiceCallInvoker;

        private string _authCode;

        private string _authCodeString;

        private string _password;

        private int _sequenceNumber;

        private string _serviceUrl;

        private int _sessionId;

        private string _teamName;

        private string _username;

        protected ServiceCallViewModel(ICommonDataManager commonDataManager, IServiceCallInvoker serviceCallInvoker)
        {
            CommonDataManager = commonDataManager;
            ServiceCallInvoker = serviceCallInvoker;

            ServiceUrl = CommonDataManager.ServerUrl;
            Username = CommonDataManager.Username;
            TeamName = CommonDataManager.TeamName;
            Password = CommonDataManager.Password;
            SessionId = CommonDataManager.SessionId;
            SequenceNumber = CommonDataManager.SequenceNumber;

            CalculateAuthCode();

            CommonDataManager.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ServerUrl")
                {
                    ServiceUrl = CommonDataManager.ServerUrl;
                }
                else if (args.PropertyName == "Username")
                {
                    Username = CommonDataManager.Username;
                }
                else if (args.PropertyName == "TeamName")
                {
                    TeamName = CommonDataManager.TeamName;
                }
                else if (args.PropertyName == "Password")
                {
                    Password = CommonDataManager.Password;
                }
                else if (args.PropertyName == "SessionId")
                {
                    SessionId = CommonDataManager.SessionId;
                }
                else if (args.PropertyName == "SequenceNumber")
                {
                    SequenceNumber = CommonDataManager.SequenceNumber;
                }
            };

            this.PropertyChanged += (sender, args) =>
            {
                string[] authcodeProps = new string[] { "TeamName", "Username", "SessionId", "SequenceNumber", "Password" };
                if (Array.IndexOf(authcodeProps, args.PropertyName) >= 0)
                    CalculateAuthCode();
            };
        }

        public abstract string Title { get; }

        public string ServiceUrl
        {
            get { return _serviceUrl; }
            set { SetProperty(ref _serviceUrl, value); }
        }

        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        public string TeamName
        {
            get { return _teamName; }
            set { SetProperty(ref _teamName, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public int SessionId
        {
            get { return _sessionId; }
            set { SetProperty(ref _sessionId, value); }
        }

        public int SequenceNumber
        {
            get { return _sequenceNumber; }
            set { SetProperty(ref _sequenceNumber, value); }
        }

        public string AuthCode
        {
            get { return _authCode; }
            set { SetProperty(ref _authCode, value); }
        }

        public string AuthCodeString
        {
            get { return _authCodeString; }
            set { SetProperty(ref _authCodeString, value); }
        }

        protected void CalculateAuthCode()
        {
            AuthCodeString = string.Format("{0}:{1}:{2}:{3}{4}", TeamName, Username, SessionId, SequenceNumber, Password);
            AuthCode = AuthCodeManager.GetAuthCode(AuthCodeString);
        }
    }
}