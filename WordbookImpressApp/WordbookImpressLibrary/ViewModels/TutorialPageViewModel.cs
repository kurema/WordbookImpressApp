using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WordbookImpressLibrary.ViewModels
{
    public class TutorialPageViewModel:BaseViewModel
    {
        private string titleText;
        public string TitleText { get => titleText; set => SetProperty(ref titleText, value); }
        private string descriptionText;
        public string DescriptionText { get => descriptionText; set => SetProperty(ref descriptionText, value); }
        private string imageUrl;
        public string ImageUrl { get => imageUrl; set => SetProperty(ref imageUrl, value); }
        private TutorialPagesViewModel parent;
        public TutorialPagesViewModel Parent { get => parent; set => SetProperty(ref parent, value); }

        public TutorialPageViewModel(string title,string description,string image)
        {
            this.TitleText = title;
            this.DescriptionText = description;
            this.ImageUrl = image;
        }
    }

    public class TutorialPagesViewModel : BaseViewModel
    {
        public TutorialPagesViewModel(ObservableCollection<TutorialPageViewModel> Items)
        {
            this.Items = Items;
        }

        private Action onFinishAction;
        public Action OnFinishAction { get => onFinishAction; set => SetProperty(ref onFinishAction, value); }

        private ObservableCollection<TutorialPageViewModel> items;
        public ObservableCollection<TutorialPageViewModel> Items { get => items; set
            {
                SetProperty(ref items, value);
                SelectedCount = 0;
                foreach(var item in items)
                {
                    item.Parent = this;
                }
            }
        }
        private int selectedCount;
        public int SelectedCount { get => selectedCount; set
            {
                if (value < items.Count && value >= 0)
                {
                    SetProperty(ref selectedCount, value);
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }
        public TutorialPageViewModel SelectedItem
        {
            get
            {
                if (selectedCount < items.Count && selectedCount >= 0)
                {
                    return Items[selectedCount];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                var result= Items.IndexOf(value);
                if (result != -1) SelectedCount = result;
            }
        }
        private Helper.DelegateCommand previousPageComman;
        public Helper.DelegateCommand PreviousPageCommand { get
            {
                if (previousPageComman != null) return previousPageComman;
                previousPageComman = new Helper.DelegateCommand(
                    (o) => (selectedCount + 1 < items.Count && selectedCount + 1 >= 0),
                    (o) => SelectedCount++
                );
                this.PropertyChanged += (s, e) => previousPageComman.OnCanExecuteChanged();
                return previousPageComman;
            }
        }
        private Helper.DelegateCommand nextPageCommand;
        public Helper.DelegateCommand NextPageCommand
        {
            get
            {
                if (nextPageCommand != null) return nextPageCommand;
                nextPageCommand = new Helper.DelegateCommand(
                    (o) => (selectedCount + 1 < items.Count && selectedCount + 1 >= 0),
                    (o) => SelectedCount++
                );
                this.PropertyChanged += (s, e) => nextPageCommand.OnCanExecuteChanged();
                return nextPageCommand;
            }
        }
        public bool IsSinglePage => this.Items.Count == 1;
    }
}
