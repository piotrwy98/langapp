using LangApp.WpfClient.Models;
using LangApp.WpfClient.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace LangApp.WpfClient.Views.Controls
{
    /// <summary>
    /// Interaction logic for LearnFinishControl.xaml
    /// </summary>
    public partial class LearnFinishControl : UserControl
    {
        public LearnFinishControl(bool isTest, TimeSpan timer, int numberOfQuestions, List<Answer> answers)
        {
            InitializeComponent();
            DataContext = new LearnFinishViewModel(isTest, timer, numberOfQuestions, answers);
        }
    }
}
