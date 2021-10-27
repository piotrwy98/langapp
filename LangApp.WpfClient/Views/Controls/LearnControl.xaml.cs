using LangApp.WpfClient.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace LangApp.WpfClient.Views.Controls
{
    /// <summary>
    /// Interaction logic for LearnControl.xaml
    /// </summary>
    public partial class LearnControl : UserControl
    {
        public LearnControl(Guid languageId, List<Guid> categoriesIds, bool isClosedChosen, bool isOpenChosen, bool isSpeakChosen)
        {
            InitializeComponent();
            DataContext = new LearnViewModel(languageId, categoriesIds, isClosedChosen, isOpenChosen, isSpeakChosen);
        }
    }
}
