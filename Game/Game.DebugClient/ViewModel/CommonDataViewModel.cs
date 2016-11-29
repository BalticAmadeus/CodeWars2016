using Game.DebugClient.Infrastructure;
using Prism.Mvvm;

namespace Game.DebugClient.ViewModel
{
    public class CommonDataViewModel : BindableBase
    {
        private readonly ICommonDataManager _commonDataManager;

        public CommonDataViewModel(ICommonDataManager commonDataManager)
        {
            _commonDataManager = commonDataManager;
            _commonDataManager.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case "SessionId":
                        OnPropertyChanged(() => SessionId);
                        break;
                    case "SequenceNumber":
                        OnPropertyChanged(() => SequenceNumber);
                        break;
                    case "PlayerId":
                        OnPropertyChanged(() => PlayerId);
                        break;
                }
            };
        }

        public string ServiceUrl
        {
            get { return _commonDataManager.ServerUrl; }
            set
            {
                _commonDataManager.ServerUrl = value;
                OnPropertyChanged(() => ServiceUrl);
            }
        }

        public string Username
        {
            get { return _commonDataManager.Username; }
            set
            {
                _commonDataManager.Username = value;
                OnPropertyChanged(() => Username);
            }
        }

        public string TeamName
        {
            get { return _commonDataManager.TeamName; }
            set
            {
                _commonDataManager.TeamName = value;
                OnPropertyChanged(() => TeamName);
            }
        }

        public string Password
        {
            get { return _commonDataManager.Password; }
            set
            {
                _commonDataManager.Password = value;
                OnPropertyChanged(() => TeamName);
            }
        }

        public int SessionId
        {
            get { return _commonDataManager.SessionId; }
            set
            {
                _commonDataManager.SessionId = value;
                OnPropertyChanged(() => SessionId);
            }
        }

        public int SequenceNumber
        {
            get { return _commonDataManager.SequenceNumber; }
            set
            {
                _commonDataManager.SequenceNumber = value;
                OnPropertyChanged(() => SequenceNumber);
            }
        }

        public string Challenge
        {
            get { return _commonDataManager.Challenge; }
            set
            {
                _commonDataManager.Challenge = value;
                OnPropertyChanged(() => Challenge);
            }
        }

        public int PlayerId
        {
            get { return _commonDataManager.PlayerId; }
            set
            {
                _commonDataManager.PlayerId = value;
                OnPropertyChanged(() => PlayerId);
            }
        }
    }
}