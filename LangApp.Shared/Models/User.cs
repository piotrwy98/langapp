using System;
using static LangApp.Shared.Models.Enums;

namespace LangApp.Shared.Models
{
    public class User : NotifyPropertyChanged
    {
        private Guid _id;
        public Guid Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _username;
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private UserRole _userRole;
        public UserRole UserRole
        {
            get
            {
                return _userRole;
            }
            set
            {
                _userRole = value;
                OnPropertyChanged();
            }
        }
    }
}
