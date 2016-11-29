using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace Game.AdminClient.ViewModels
{
    public class GameViewModel : BindableBase
    {
        public GameViewModel()
        {
            PlayerCollection = new ObservableCollection<PlayerViewModel>();
        }

        private ObservableCollection<PlayerViewModel> _playerCollection;
        public ObservableCollection<PlayerViewModel> PlayerCollection
        {
            get { return _playerCollection; }
            set
            {
                SetProperty(ref _playerCollection, value);
            }
        }
        
        private string _state;
        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }


        private string _label;
        public string Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        private int _gameId;
        public int GameId
        {
            get { return _gameId; }
            set { SetProperty(ref _gameId, value); }
        }

        public override int GetHashCode()
        {
            return GameId;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var gameViewModel = obj as GameViewModel;
            if (gameViewModel == null)
                return false;

            return GetHashCode() == gameViewModel.GetHashCode();
        }
    }
}