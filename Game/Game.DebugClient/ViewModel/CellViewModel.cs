using Game.DebugClient.Infrastructure;
using Prism.Mvvm;

namespace Game.DebugClient.ViewModel
{
    public class CellViewModel : BindableBase
    {
        public const double CellSize = 16;

        private int _x;
        private int _y;
        private CellState _state;


        public int X
        {
            get { return _x; }
            set
            {
                SetProperty(ref _x, value);
                OnPropertyChanged(() => Left);
            }
        }

        public int Y
        {
            get { return _y; }
            set
            {
                SetProperty(ref _y, value);
                OnPropertyChanged(() => Top);
            }
        }

        public CellState State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        public double Left => _x*CellSize;

        public double Top => _y*CellSize;
    }
}