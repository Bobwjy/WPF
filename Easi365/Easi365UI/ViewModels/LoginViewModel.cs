using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows;
using ClientLib.Common;
using Easi365UI.Entities;

namespace Easi365UI.ViewModels
{
    [Export(typeof(IScreen))]
    public class LoginViewModel : Screen
    {
        private IEventAggregator _events;
        private IWindowManager _windowManager;

        private string _userName = "capad";
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; NotifyOfPropertyChange(() => UserName); }
        }

        private string _passWord;
        public string Password
        {
            get { return _passWord; }
            set { _passWord = value; NotifyOfPropertyChange(()=>Password); }
        }

        [ImportingConstructor]
        public LoginViewModel(IEventAggregator events, IWindowManager windowManager)
        {
            _events = events;
            _windowManager = windowManager;
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
        }

        public void Login()
        {
            var username = UserName;
            var password = Password;

            if(!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                _windowManager.ShowWindow(new MainWindowViewModel());

            TryClose();
        }

        public void Close()
        {
            TryClose();
        }
    }
}
