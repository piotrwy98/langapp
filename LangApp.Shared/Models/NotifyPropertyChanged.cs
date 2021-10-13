using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LangApp.Shared.Models
{
    public abstract class NotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
