using LangApp.WpfClient.ViewModels;
using System;

namespace LangApp.WpfClient.Models
{
    public class ChartItem : NotifyPropertyChanged
    {
        public DateTime DateTime { get; set; }

        private double _value;
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }
    }
}
