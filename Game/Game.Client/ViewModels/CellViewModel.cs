using Prism.Mvvm;

namespace Game.AdminClient.ViewModels
{
    public class CellViewModel : BindableBase
    {
        private double _cellSize;
        public double CellSize
        {
            get { return _cellSize; }
            set { SetProperty(ref _cellSize, value); }
        }

        private int _x;
        public int X
        {
            get { return _x; }
            set
            {
                SetProperty(ref _x, value);
                OnPropertyChanged(() => Left);
            }
        }

        private int _y;
        public int Y
        {
            get { return _y; }
            set
            {
                SetProperty(ref _y, value);
                OnPropertyChanged(() => Top);
            }
        }

        private string _state;
        public string State
        {
            get { return _state; }
            set
            {
                SetProperty(ref _state, value);
                OnPropertyChanged(() => Model);
            }
        }

        public double Left => _x*CellSize;

        public double Top => _y*CellSize;

        public CellViewModel Model => this;
    }
}
