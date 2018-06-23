using System;
using System.Collections.Generic;
using System.Text;

namespace WordbookImpressLibrary.ViewModels
{
    public class ConfigViewModel : BaseViewModel
    {
        public ConfigViewModel()
        {
            LoadConfig();
        }

        public ConfigViewModel(Models.Config config)
        {
            this.Content = config;
        }

        public Models.Config Content
        {
            get => new Models.Config() { SkipChecked = this.skipChecked, SkipMinRate = this.SkipMinRate, SkipMinCorrect = this.skipMinCorrect, ChoiceCount = this.ChoiceCount };
            set
            {
                this.SkipChecked = value.SkipChecked;
                this.skipMinCorrect = value.SkipMinCorrect;
                this.SkipMinRate = value.SkipMinRate;
                this.ChoiceCount = value.ChoiceCount;
            }
        }

        public void LoadConfig()
        {
            this.Content = Storage.ConfigStorage.Content;
        }

        public void SaveConfig()
        {
            this.Content = Storage.ConfigStorage.Content;
            Storage.ConfigStorage.SaveLocalData();
        }

        private bool skipChecked = false;
        public bool SkipChecked { get => skipChecked; set => SetProperty(ref skipChecked, value); }

        private double skipMinRate = 1.0;
        public double SkipMinRate { get => skipMinRate; set => SetProperty(ref skipMinRate, value); }

        private int skipMinCorrect = int.MaxValue;
        public int SkipMinCorrect { get => skipMinCorrect; set => SetProperty(ref skipMinCorrect, value); }

        private int choiceCount = 4;
        public int ChoiceCount { get => choiceCount; set => SetProperty(ref choiceCount, value); }
    }
}
