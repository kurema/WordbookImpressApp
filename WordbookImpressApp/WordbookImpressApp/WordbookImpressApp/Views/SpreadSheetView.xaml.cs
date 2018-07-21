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
	public partial class SpreadSheetView : ContentView
	{
        public double SeparatorWidth { get; set; } = 1;
        public Color SeparatorColor { get; set; } = new Color(0.8);

		public SpreadSheetView ()
		{
			InitializeComponent ();

            Init();
		}

        public void SetDitcionary(Dictionary<string, List<string>> dic)
        {
            Init();
            int c = 0;
            foreach(var item in dic)
            {
                AddCell(item.Key, c, 0,true);
                var r = 1;
                foreach(var cell in item.Value)
                {
                    AddCell(cell, c, r);
                    r++;
                }
                c++;
            }
        }

        public void SetDitcionary(Dictionary<WordbookImpressLibrary.Helper.SpreadSheet.RowColumn, string> dic)
        {
            Init();
            foreach(var item in dic)
            {
                AddCell(item.Value, item.Key.Column - 1, item.Key.Row - 1, item.Key.Row == 1);
            }
        }


        public void Init()
        {
            grid.RowDefinitions = new RowDefinitionCollection() { new RowDefinition() { Height = SeparatorWidth } };
            grid.ColumnDefinitions = new ColumnDefinitionCollection() { new ColumnDefinition() { Width = SeparatorWidth } };
            grid.Children.Clear();
            this.SeparatorsRow = new List<BoxView>();
            this.SeparatorsColumn = new List<BoxView>();

            AddSeparatorRow(0);
            AddSeparatorColumn(0);
        }

        public void AddCell(string content, int column, int row, bool header = false)
        {
            var label = !header ?
                new Label()
                {
                    Text = content,
                    VerticalOptions = LayoutOptions.Center,
                } :
                new Label()
                {
                    Text=content,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontAttributes=FontAttributes.Bold,
                };
            AddCell(label, column, row);
        }

        public void AddCell(View content, int column, int row)
        {
            if (column < 0 && row < 0) return;
            SetMaximumColumn(column);
            SetMaximumRows(row);

            Grid.SetColumn(content, column * 2 + 1);
            Grid.SetRow(content, row * 2 + 1);

            grid.Children.Add(content);

        }

        public void SetMaximumRows(int count)
        {
            count++;
            if (grid.RowDefinitions == null) grid.RowDefinitions = new RowDefinitionCollection();
            if (grid.RowDefinitions.Count == count) return;
            for (int i = grid.RowDefinitions.Count; i < count * 2 + 1; i += 2)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition() { Height = SeparatorWidth });
                AddSeparatorRow(i + 1);
            }

            foreach (var item in SeparatorsColumn)
            {
                Grid.SetRowSpan(item, count * 2 + 1);
            }
        }

        public void SetMaximumColumn(int count)
        {
            count++;
            if (grid.ColumnDefinitions == null) grid.ColumnDefinitions = new ColumnDefinitionCollection();
            if (grid.ColumnDefinitions.Count == count) return;
            for (int i = grid.ColumnDefinitions.Count; i < count * 2 + 1; i += 2)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = SeparatorWidth });
                AddSeparatorColumn(i + 1);
            }

            foreach (var item in SeparatorsRow)
            {
                Grid.SetColumnSpan(item, count * 2 + 1);
            }
        }

        public List<BoxView> SeparatorsColumn = new List<BoxView>();

        public void AddSeparatorColumn(int row)
        {
            var box = new BoxView()
            {
                VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = this.SeparatorColor,
                WidthRequest = SeparatorWidth
            };
            Grid.SetColumn(box, row);
            Grid.SetRow(box, 0);
            Grid.SetRowSpan(box, Math.Max(1, grid.RowDefinitions?.Count() ?? 1));

            SeparatorsColumn.Add(box);
            grid.Children.Add(box);
        }

        public List<BoxView> SeparatorsRow = new List<BoxView>();

        public void AddSeparatorRow(int column)
        {
            var box = new BoxView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = this.SeparatorColor,
                HeightRequest = SeparatorWidth
            };
            Grid.SetRow(box, column);
            Grid.SetColumn(box, 0);
            Grid.SetColumnSpan(box, Math.Max(1, grid.ColumnDefinitions?.Count() ?? 1));

            SeparatorsRow.Add(box);
            grid.Children.Add(box);
        }
    }
}