using LangApp.WpfClient.Models;
using LangApp.WpfClient.ViewModels.Windows;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Windows.UI.Notifications;

namespace LangApp.WpfClient.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            PrepareSystemTray();
            DataContext = new MainViewModel();
        }

        private void PrepareSystemTray()
        {
            Stream iconStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("LangApp.WpfClient.Resources.icon.ico");

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = new Icon(iconStream);
            _notifyIcon.Visible = true;

            _notifyIcon.DoubleClick += delegate (object sender, EventArgs args)
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

            _notifyIcon.ContextMenu = new ContextMenu();
            _notifyIcon.ContextMenu.MenuItems.Add(openMenuItem);
            _notifyIcon.ContextMenu.MenuItems.Add(closeMenuItem);
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // usunięcie ikony w pasku zadań
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();

            // usunięcie aktualnych notifykacji
            ToastNotificationManagerCompat.History.Clear();

            // usunięcie nadchodzących notyfikacji
            var notifier = ToastNotificationManagerCompat.CreateToastNotifier();
            var scheduledToasts = notifier.GetScheduledToastNotifications();

            foreach(var toast in scheduledToasts)
            {
                notifier.RemoveFromSchedule(toast);
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
        }
    }
}
