using Prism.Mvvm;

namespace Game.AdminClient.ViewModels
{
    public class PlayerViewModel : BindableBase
    {
        private int _playerId;
        public int PlayerId
        {
            get { return _playerId; }
            set { SetProperty(ref _playerId, value); }
        }

        private string _team;
        public string Team
        {
            get { return _team; }
            set { SetProperty(ref _team, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private int? _gameId;
        public int? GameId
        {
            get { return _gameId; }
            set { SetProperty(ref _gameId, value); }
        }

        public override int GetHashCode()
        {
            return PlayerId;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var playerViewModel = obj as PlayerViewModel;
            if (playerViewModel == null)
                return false;

            return GetHashCode() == playerViewModel.GetHashCode();
        }
    }
}