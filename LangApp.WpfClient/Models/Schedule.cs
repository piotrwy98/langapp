using LangApp.WpfClient.ViewModels;
using Newtonsoft.Json;
using System;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.Models
{
    public class Schedule : NotifyPropertyChanged
    {
        public Guid Id { get; set; }

        private SessionSettings _sessionSettings;
        public SessionSettings SessionSettings
        {
            get
            {
                return _sessionSettings;
            }
            set
            {
                _sessionSettings = value;
                OnPropertyChanged();
            }
        }

        public uint UserId { get; set; }

        private bool _isActive = true;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

        private bool _isEnabled;
        [JsonIgnore]
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public SessionType SessionType { get; set; }

        private uint _intervalMinutes = 20;
        public uint IntervalMinutes
        {
            get
            {
                return _intervalMinutes;
            }
            set
            {
                _intervalMinutes = value;
                OnPropertyChanged();
                OnPropertyChanged("CanIntervalBeDecremented");
            }
        }

        public bool CanIntervalBeDecremented
        {
            get
            {
                return _intervalMinutes > 1;
            }
        }

        public bool CanIntervalBeIncremented
        {
            get
            {
                return _intervalMinutes < uint.MaxValue;
            }
        }
    }
}
