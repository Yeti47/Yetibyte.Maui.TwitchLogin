using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Maui.TwitchLogin.ViewModels
{
    internal abstract class ViewModel : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Ctors

        protected ViewModel() { }

        #endregion

        #region Methods

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") { 

            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        #endregion
    }
}
