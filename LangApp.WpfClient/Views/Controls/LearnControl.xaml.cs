using LangApp.Shared.Models;
using LangApp.WpfClient.ViewModels.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LangApp.WpfClient.Views.Controls
{
    /// <summary>
    /// Interaction logic for LearnControl.xaml
    /// </summary>
    public partial class LearnControl : UserControl
    {
        public LearnControl(bool isTest, uint sessionId, Language language, List<uint> categoriesIds, bool isClosedChosen, bool isOpenChosen, bool isSpeakChosen, int numberOfQuestions)
        {
            InitializeComponent();
            DataContext = new LearnViewModel(isTest, sessionId, language, categoriesIds, isClosedChosen, isOpenChosen, isSpeakChosen, numberOfQuestions);
        }

        private void TextBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((sender as TextBox).IsEnabled)
            {
                (sender as TextBox).Focus();
            }
            else
            {
                Keyboard.ClearFocus();
            }
        }

        private void TextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((sender as TextBox).IsVisible)
            {
                (sender as TextBox).Focus();
            }
        }
    }
}
