﻿using LangApp.WpfClient.ViewModels.Controls;
using System.Windows.Controls;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.Views.Controls
{
    /// <summary>
    /// Interaction logic for LearnSettingsControl.xaml
    /// </summary>
    public partial class LearnSettingsControl : UserControl
    {
        public LearnSettingsControl(SessionType sessionType)
        {
            InitializeComponent();
            DataContext = new LearnSettingsViewModel(sessionType);
        }
    }
}
