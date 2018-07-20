using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WordbookImpressApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EntryListView : ContentView
    {
        public EntryListView()
        {
            InitializeComponent();
        }

        public Color PlaceholderColor { get; set; }
        public Color BorderColor { get; set; }
        public double BorderWidth { get; set; }
        public IEnumerable<EntryListViewItem> Entries { get; set; }

        public void Update()
        {
            var entries = Entries.ToArray();
            {
                grid.ColumnDefinitions = new ColumnDefinitionCollection();
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            }

            {
                for (int i = 0; i < entries.Count(); i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height = BorderWidth });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                }
                grid.RowDefinitions.Add(new RowDefinition() { Height = BorderWidth });
            }

            grid.Children.Clear();
            {
                var fc = new FontSizeConverter();
                for (int i = 0; i <= entries.Count(); i++)
                {
                    {
                        if (this.BorderWidth > 0 && this.BorderColor != Color.Transparent)
                        {
                            var box = new BoxView() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = this.BorderColor };
                            grid.Children.Add(box);
                            Grid.SetColumn(box, 0);
                            Grid.SetColumnSpan(box, 2);
                            Grid.SetRow(box, i * 2);
                        }
                    }
                }

                for (int i = 0; i < entries.Count(); i++)
                {
                    var item = entries[i];
                    {
                        Image image=null;
                        if (item.ImageUrl != null)
                        {
                            var view = new Image()
                            {
                                Source = ImageSource.FromFile(item.ImageUrl),
                                VerticalOptions = LayoutOptions.Center,
                                HeightRequest = 0,
                                Margin=new Thickness(5,0)
                            };

                            grid.Children.Add(view);
                            Grid.SetColumn(view, 0);
                            Grid.SetRow(view, i * 2 + 1);

                            image = view;
                        }
                        else if (!String.IsNullOrWhiteSpace(item.Title))
                        {
                            var view = new Label()
                            {
                                Text = item.Title,
                                VerticalOptions = LayoutOptions.Center,
                            };
                            grid.Children.Add(view);
                            Grid.SetColumn(view, 0);
                            Grid.SetRow(view, i * 2 + 1);
                        }
                        {
                            var view = new HumbleEntry()
                            {
                                Placeholder = item.PlaceHolder,
                                PlaceholderColor = this.PlaceholderColor
                            };
                            view.PropertyChanged += (s, e) => item.Action?.Invoke(view.Text);
                            grid.Children.Add(view);
                            Grid.SetColumn(view, 1);
                            Grid.SetRow(view, i * 2 + 1);
                            if (!string.IsNullOrWhiteSpace(item.EntryBinding))
                            {
                                view.SetBinding(Entry.TextProperty, item.EntryBinding);
                            }
                            else if (!string.IsNullOrWhiteSpace(item.EntryDefault))
                            {
                                view.Text = item.EntryDefault;
                            }

                            //Ugly.
                            EventHandler sc= (s, e) => {
                                var h = ((HumbleEntry)s).Height;
                                if (h > 0 && image!=null) {
                                    image.HeightRequest = h;
                                }
                            };
                            view.SizeChanged += sc;
                        }
                    }
                }
            }
        }
    }
    public class EntryListViewItem
    {
        public string Title { get; set; }
        public Action<string> Action { get; set; }
        public string ImageUrl { get; set; }
        public string PlaceHolder { get; set; }
        public string EntryDefault { get; set; }
        public string EntryBinding { get; set; }
    }
}