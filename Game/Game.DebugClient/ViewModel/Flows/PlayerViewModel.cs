using Prism.Mvvm;

namespace Game.DebugClient.ViewModel.Flows
{
    public class PlayerViewModel : BindableBase
    {
        private string _condition;
        private int _index;

        public int Index
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        }

        public string Condition
        {
            get { return _condition; }
            set { SetProperty(ref _condition, value); }
        }
    }
}