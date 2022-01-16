using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Windows;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class MainScreenViewModel
    {
        #region Commands
        public ICommand AddNewsCommand { get; }
        public ICommand EditNewsCommand { get; }
        public ICommand RemoveNewsCommand { get; }
        public ICommand SaveNewsCommand { get; }
        public ICommand CancelNewsCommand { get; }
        #endregion

        #region Properties
        public ObservableCollection<Post> Posts { get; }
        public bool IsUserAdmin { get; }
        public Configuration Configuration { get; }
        #endregion

        public MainScreenViewModel()
        {
            AddNewsCommand = new RelayCommand(AddNews);
            EditNewsCommand = new RelayCommand(EditNews);
            RemoveNewsCommand = new RelayCommand(RemoveNews);
            SaveNewsCommand = new RelayCommand(SaveNews);
            CancelNewsCommand = new RelayCommand(CancelNews);

            Posts = NewsService.GetInstance().Posts;
            IsUserAdmin = Configuration.User.Role == UserRole.ADMIN;
            Configuration = Configuration.GetInstance();
        }

        private void AddNews(object obj)
        {
            var news = new News()
            {
                UserId = Configuration.User.Id
            };

            Posts.Insert(0, new Post(news)
            {
                IsEditing = true
            });
        }

        private void EditNews(object obj)
        {
            var post = obj as Post;
            if (post != null)
            {
                post.Title = post.News.Title;
                post.Content = post.News.Content;
                post.IsEditing = true;
            }
        }

        private async void RemoveNews(object obj)
        {
            var post = obj as Post;
            if (post != null)
            {
                var confirmationWindow = new ConfirmationWindow(Application.Current.Resources["remove_post"].ToString(),
                    Application.Current.Resources["remove_post_confirmation"].ToString() + " " + post.Title + "?");
                confirmationWindow.ShowDialog();

                if (confirmationWindow.DialogResult == true)
                {
                    if (await NewsService.RemoveNewsAsync(post.News.Id))
                    {
                        Posts.Remove(post);
                    }
                }
            }
        }

        private async void SaveNews(object obj)
        {
            var post = obj as Post;
            if (post != null)
            {
                post.IsSaving = true;
                Mouse.OverrideCursor = Cursors.AppStarting;

                if (post.News.Id != 0)
                {
                    var oldTitle = post.News.Title;
                    var oldContent = post.News.Content;

                    post.News.Title = post.Title;
                    post.News.Content = post.Content;

                    if (await NewsService.UpdateNewsAsync(post.News))
                    {
                        post.IsEditing = false;
                        post.News = post.News;
                    }
                    else
                    {
                        post.News.Title = oldTitle;
                        post.News.Content = oldContent;
                    }
                }
                else
                {
                    post.News.Title = post.Title;
                    post.News.Content = post.Content;

                    var createdNews = await NewsService.CreateNewsAsync(post.News);

                    if (createdNews != null)
                    {
                        post.News = createdNews;
                        post.IsEditing = false;
                    }
                }

                Mouse.OverrideCursor = null;
                post.IsSaving = false;
            }
        }

        private void CancelNews(object obj)
        {
            var post = obj as Post;
            if (post != null)
            {
                var confirmationWindow = new ConfirmationWindow(Application.Current.Resources["discard_changes"].ToString(),
                    Application.Current.Resources["discard_changes_confirmation"].ToString());
                confirmationWindow.ShowDialog();

                if (confirmationWindow.DialogResult == true)
                {
                    if (post.News.Id != 0)
                    {
                        post.Title = post.News.Title;
                        post.Content = post.News.Content;
                        post.IsEditing = false;
                    }
                    else
                    {
                        Posts.Remove(post);
                    }
                }
            }
        }
    }
}
