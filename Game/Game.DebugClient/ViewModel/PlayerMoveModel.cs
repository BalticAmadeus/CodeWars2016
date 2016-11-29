using Game.ClientCommon;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.DebugClient.ViewModel
{
    public class PlayerMoveModel : BindableBase
    {
        private int _index;
        private string _name;
        private Point _move;
        private bool _isTecman;

        public PlayerMoveModel Model
        {
            get { return this; }
        }

        public int Index
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public Point Move
        {
            get { return _move; }
            set { SetProperty(ref _move, value); }
        }

        public bool IsTecman
        {
            get { return _isTecman; }
            set { SetProperty(ref _isTecman, value); }
        }
    }
}
