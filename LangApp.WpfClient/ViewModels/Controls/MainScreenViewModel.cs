﻿using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using System.Collections.ObjectModel;
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
        #endregion

        #region Properties
        public ObservableCollection<ObjectToChoose> News { get; }
        public bool IsUserAdmin { get; }
        #endregion

        public MainScreenViewModel()
        {
            AddNewsCommand = new RelayCommand(AddNews);
            EditNewsCommand = new RelayCommand(EditNews);
            RemoveNewsCommand = new RelayCommand(RemoveNews);
            SaveNewsCommand = new RelayCommand(SaveNews);

            News = NewsService.GetInstance().News;
            IsUserAdmin = Configuration.User.Role == UserRole.ADMIN;
        }

        private void AddNews(object obj)
        {
            News.Insert(0, new ObjectToChoose()
            {
                Object = new News()
                {
                    UserId = Configuration.User.Id
                },
                IsChosen = true
            });
        }

        private void EditNews(object obj)
        {
            var news = obj as ObjectToChoose;
            if (news != null)
            {
                news.IsChosen = true;
            }
        }

        private async void RemoveNews(object obj)
        {
            var news = obj as ObjectToChoose;
            if (news != null)
            {
                if (await NewsService.RemoveNewsAsync((news.Object as News).Id))
                {
                    News.Remove(news);
                }
            }
        }

        private async void SaveNews(object obj)
        {
            var news = obj as ObjectToChoose;
            if (news != null)
            {
                if((news.Object as News).Id != 0)
                {
                    if (await NewsService.UpdateNewsAsync(news.Object as News))
                    {
                        news.IsChosen = false;
                    }
                }
                else
                {
                    var createdNews = await NewsService.CreateNewsAsync(news.Object as News);

                    if(createdNews != null)
                    {
                        news.Object = createdNews;
                        news.IsChosen = false;
                    }
                }
            }
        }
    }
}
