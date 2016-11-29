using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Game.DebugClient.Infrastructure
{
    public class CommonDataManager : ICommonDataManager
    {
        private readonly ISettingsManager _settingsManager;

        private string _challenge;

        private int _playerId;

        private int _sequenceNumber;

        private int _sessionId;

        private int _turn;

        private bool _iAmTecman;

        private int _gameId;

        public CommonDataManager(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string TeamName
        {
            get { return _settingsManager.TeamName; }
            set
            {
                _settingsManager.TeamName = value;
                RaisePropertyChanged();
            }
        }

        public string Username
        {
            get { return _settingsManager.Username; }
            set
            {
                _settingsManager.Username = value;
                RaisePropertyChanged();
            }
        }

        public string Password
        {
            get { return _settingsManager.Password; }
            set
            {
                _settingsManager.Password = value;
                RaisePropertyChanged();
            }
        }

        public string ServerUrl
        {
            get { return _settingsManager.ServerUrl; }
            set
            {
                _settingsManager.ServerUrl = value;
                RaisePropertyChanged();
            }
        }

        public int SessionId
        {
            get { return _sessionId; }
            set
            {
                _sessionId = value;
                RaisePropertyChanged();
            }
        }

        public int SequenceNumber
        {
            get { return _sequenceNumber; }
            set
            {
                _sequenceNumber = value;
                RaisePropertyChanged();
            }
        }

        public string Challenge
        {
            get { return _challenge; }
            set
            {
                _challenge = value;
                RaisePropertyChanged();
            }
        }

        public int PlayerId
        {
            get { return _playerId; }
            set
            {
                _playerId = value;
                RaisePropertyChanged();
            }
        }

        public int Turn
        {
            get { return _turn; }
            set
            {
                _turn = value;
                RaisePropertyChanged();
            }
        }

        public bool IAmTecman
        {
            get { return _iAmTecman; }
            set
            {
                _iAmTecman = value;
                RaisePropertyChanged();
            }
        }

        public int GameId
        {
            get { return _gameId; }
            set
            {
                _gameId = value;
                RaisePropertyChanged();
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var args = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, args);
        }
    }
}