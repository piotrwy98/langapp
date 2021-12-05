using LangApp.WpfClient.Models;
using LangApp.WpfClient.ViewModels.Windows;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace LangApp.WpfClient.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            PrepareSystemTray();
        }

        private void PrepareSystemTray()
        {
            Stream iconStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("LangApp.WpfClient.Resources.icon.ico");

            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new Icon(iconStream);
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += delegate (object sender, EventArgs args)
            {
                Show();
                WindowState = WindowState.Normal;
            };

            var openMenuItem = new MenuItem(System.Windows.Application.Current.Resources["open_context"].ToString());
            openMenuItem.Click += delegate (object sender, EventArgs e)
            {
                Show();
                WindowState = WindowState.Normal;
            };

            var closeMenuItem = new MenuItem(System.Windows.Application.Current.Resources["close"].ToString());
            closeMenuItem.Click += delegate (object sender, EventArgs e)
            {
                Close();
            };

            notifyIcon.ContextMenu = new ContextMenu();
            notifyIcon.ContextMenu.MenuItems.Add(openMenuItem);
            notifyIcon.ContextMenu.MenuItems.Add(closeMenuItem);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseWindow_Button_Click(object sender, RoutedEventArgs e)
        {
            if(Settings.GetInstance().MinimizeToSystemTray)
            {
                Hide();
            }
            else
            {
                Close();
            }
        }

        private void MinimizeWindow_Button_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
