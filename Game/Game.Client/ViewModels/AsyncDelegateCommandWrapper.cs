using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace Game.AdminClient.ViewModels
{
    public class AsyncDelegateCommandWrapper : BindableBase
    {
        public AsyncDelegateCommandWrapper(Action action)
        {
            Command = new DelegateCommand(async () =>
            {
                IsExecuting = true;
                await Task.Delay(1);
                action.Invoke();
                IsExecuting = false;
            });

            IsExecuting = false;
        }

        public AsyncDelegateCommandWrapper(Func<Task> action)
        {
            Command = new DelegateCommand(async () =>
            {
                IsExecuting = true;
                await Task.Delay(1);
                await action.Invoke();
                IsExecuting = false;
            });

            IsExecuting = false;
        }

        private bool _isExecuting;
        public bool IsExecuting
        {
            get { return _isExecuting; }
            set { SetProperty(ref _isExecuting, value); }
        }

        private ICommand _command;
        public ICommand Command
        {
            get { return _command; }
            private set { SetProperty(ref _command, value); }
        }
    }

    public class AsyncDelegateCommandWrapper<T> : BindableBase
    {
        public AsyncDelegateCommandWrapper(Action<T> action)
        {
            Command = new DelegateCommand<T>(async obj =>
            {
                IsExecuting = true;
                await Task.Delay(1);
                action.Invoke(obj);
                IsExecuting = false;
            });

            IsExecuting = false;
        }

        public AsyncDelegateCommandWrapper(Func<T, Task> action)
        {
            Command = new DelegateCommand<T>(async obj =>
            {
                IsExecuting = true;
                await Task.Delay(1);
                await action.Invoke(obj);
                IsExecuting = false;
            });

            IsExecuting = false;
        }

        private bool _isExecuting;
        public bool IsExecuting
        {
            get { return _isExecuting; }
            set { SetProperty(ref _isExecuting, value); }
        }

        private ICommand _command;
        public ICommand Command
        {
            get { return _command; }
            private set { SetProperty(ref _command, value); }
        }
    }
}
