using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SalesAnalysis.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region ОБНОВЛЕНИЕ UI

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
