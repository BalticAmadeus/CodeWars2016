using Prism.Mvvm;

namespace Game.AdminClient.ViewModels
{
    public class PlayerStateViewModel : BindableBase
    {
        private PlayerViewModel _player;
        public PlayerViewModel Player
        {
            get { return _player; }
            set { SetProperty(ref _player, value); }
        }

        private int _colorId;
        public int ColorId
        {
            get { return _colorId; }
            set { SetProperty(ref _colorId, value); }
        }

        private string _condition;
        private int _score;
        private bool _slowTurn;

        public string Condition
        {
            get { return _condition; }
            set { SetProperty(ref _condition, value); }
        }

        public int Score
        {
            get { return _score; }
            set { SetProperty(ref _score, value); }
        }

        public bool SlowTurn
        {
            get { return _slowTurn; }
            set { SetProperty(ref _slowTurn, value); }
        }

        public override int GetHashCode()
        {
            return Player.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var playerStateViewModel = obj as PlayerStateViewModel;
            if (playerStateViewModel == null)
                return false;

            return GetHashCode() == playerStateViewModel.GetHashCode();
        }
    }
}
