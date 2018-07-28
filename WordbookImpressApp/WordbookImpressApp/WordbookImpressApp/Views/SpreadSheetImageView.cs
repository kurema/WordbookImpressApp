using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp.Views.Forms;

using System.Linq;

using WordbookImpressLibrary.Helper;

namespace WordbookImpressApp.Views
{
    public class SpreadSheetImageView : SKCanvasView
    {
        private SkiaSharp.SKTypeface font;
        public SkiaSharp.SKTypeface Font => font ?? (font = SkiaSharp.SKTypeface.FromStream(typeof(CalendarGraphView).Assembly.GetManifestResourceStream("WordbookImpressApp.Fonts.NotoSansCJKjp_Regular.otf")));

        private SkiaSharp.SKPaint paint;
        public SkiaSharp.SKPaint Paint => paint ?? (paint = new SkiaSharp.SKPaint());

        public float FontSize { get; set; } = 40;
        public float SpacingRow { get; set; } = 10.0f;
        public float SpacingColumn { get; set; } = 10.0f;
        public float MarginText { get; set; } = 5.0f;
        public SkiaSharp.SKColor SeparatorColor { get; set; } = new SkiaSharp.SKColor(200, 200, 200);
        public SkiaSharp.SKColor FontColor { get; set; } = new SkiaSharp.SKColor(0, 0, 0);

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            try
            {
                e.Surface.Canvas.Clear(SkiaSharp.SKColors.White);
                if (Cells == null) return;

                var currentColumn = DisplayedColumn;
                var currentRow = DisplayedRow;
                Paint.Typeface = Font;
                Paint.TextSize = FontSize;

                var wc = currentColumn;
                float x = -DisplayedOffsetX;

                float mx = 0;
                float my = 0;

                if (RowsContent == null || ColumnsContent == null) return;
                if (currentRow >= RowsContent.Count || currentColumn >= ColumnsContent.Count) return;

                var ylist = new List<float>();
                bool ylistFirst = true;

                while (true)
                {
                    x += MarginText;
                    float y = -DisplayedOffsetY;
                    var wr = currentRow;
                    Paint.IsAntialias = true;
                    while (true)
                    {
                        var cell = Cells[wc, wr];
                        if (!cell.SizeSpecified)
                        {
                            cell.Width = Paint.MeasureText(cell.Content ?? "");
                            //cell.Height = (cell.Content.Count(a => a == '\n') + 1) * FontSize;
                            cell.Height = FontSize;
                            Cells[wc, wr] = cell;

                            var c = ColumnsContent[wc];
                            c.Width = Math.Max(c.Width, cell.Width);
                            c.WidthMargin = MarginText * 2 + SpacingColumn;
                            if (wr == 0) c.Header = cell.Content ?? "";
                            ColumnsContent[wc] = c;

                            var r = RowsContent[wr];
                            r.Height = Math.Max(r.Height, cell.Height);
                            r.HeightMargin = MarginText * 2 + SpacingRow;
                            RowsContent[wr] = r;
                        }

                        Paint.Color = FontColor;

                        y += SpacingRow / 2.0f;
                        y += MarginText;
                        e.Surface.Canvas.DrawText(cell.Content?.Split('\n')[0] ?? "", x, y + FontSize, Paint);
                        y += RowsContent[wr].Height;
                        y += MarginText;
                        y += SpacingRow / 2.0f;
                        if (ylistFirst) ylist.Add(y);
                        my = Math.Max(my, y);


                        wr++;

                        if (y > e.Info.Height) break;
                        if (RowsContent.Count <= wr) break;
                    }

                    ylistFirst = false;

                    x += ColumnsContent[wc].Width;
                    x += MarginText;
                    x += SpacingColumn / 2.0f;

                    Paint.Color = SeparatorColor;

                    e.Surface.Canvas.DrawLine(x, -DisplayedOffsetY, x, y, Paint);

                    mx = x;

                    x += MarginText;
                    x += SpacingColumn / 2.0f;

                    wc++;

                    if (x > e.Info.Width) break;
                    if (ColumnsContent.Count <= wc) break;
                }
                foreach (var y in ylist)
                {
                    Paint.Color = SeparatorColor;
                    e.Surface.Canvas.DrawLine(-DisplayedOffsetX, y, mx, y, Paint);
                }
                {
                    Paint.Color = SeparatorColor;
                    e.Surface.Canvas.DrawLine(-DisplayedOffsetX, -DisplayedOffsetY, mx, -DisplayedOffsetY, Paint);
                    e.Surface.Canvas.DrawLine(-DisplayedOffsetX, -DisplayedOffsetY, -DisplayedOffsetX, my, Paint);
                }
                ShiftColumnRow(e.Info.Width, e.Info.Height, mx, my);
            }
            catch { }
        }

        public Rows RowsContent=new Rows();
        public Columns ColumnsContent = new Columns();

        private int DisplayedRow = 0;
        private int DisplayedColumn = 0;
        private float DisplayedOffsetX = 0;
        private float DisplayedOffsetY = 0;

        public Cell[,] Cells;

        private float DisplayShiftX = 0;
        private float DisplayShiftY = 0;

        public SpreadSheetImageView() : base()
        {

            //Not working on Android and it's not my fault.
            //https://github.com/xamarin/Xamarin.Forms/issues/1495
            var g = new Xamarin.Forms.PanGestureRecognizer();
            g.TouchPoints = 1;
            g.PanUpdated += (s, e) =>
            {

                switch (e.StatusType)
                {
                    case Xamarin.Forms.GestureStatus.Started: break;
                    case Xamarin.Forms.GestureStatus.Running:
                        DisplayShiftX = (float)e.TotalX;
                        DisplayShiftY = (float)e.TotalY;
                        break;
                    case Xamarin.Forms.GestureStatus.Completed:
                    case Xamarin.Forms.GestureStatus.Canceled:
                        this.DisplayedOffsetX = (float)e.TotalX;
                        this.DisplayedOffsetY = (float)e.TotalY;
                        break;
                }

                this.InvalidateSurface();
            };
            this.GestureRecognizers.Add(g);
        }

        public void ShiftColumnRow(float width,float height,float mx,float my)
        {
            this.DisplayedOffsetX -= Math.Max(0, width - mx);
            this.DisplayedOffsetY -= Math.Max(0, height - my);

            if (this.DisplayedOffsetX < 0)
            {
                while (true)
                {
                    DisplayedColumn--;
                    if (DisplayedColumn <= 0)
                    {
                        DisplayedColumn = 0;
                        DisplayedOffsetX = 0;
                        break;
                    }
                    if (DisplayedOffsetX + ColumnsContent[DisplayedColumn].WidthCombined < 0)
                    {
                        DisplayedOffsetX += ColumnsContent[DisplayedColumn].WidthCombined;
                    }
                    else
                    {
                        DisplayedColumn++;
                        break;
                    }
                }
            }
            else
            {
                while (true)
                {
                    if (DisplayedColumn + 1 >= ColumnsContent.Count)
                    {
                        DisplayedColumn = ColumnsContent.Count - 1;
                        DisplayedOffsetX = 0;
                        break;
                    }
                    if (DisplayedOffsetX - ColumnsContent[DisplayedColumn].WidthCombined > 0)
                    {
                        DisplayedOffsetX -= ColumnsContent[DisplayedColumn].WidthCombined;
                    }
                    else break;
                    DisplayedColumn++;
                }
            }

            if (this.DisplayedOffsetY < 0)
            {
                while (true)
                {
                    DisplayedRow--;
                    if (DisplayedRow <= 0)
                    {
                        DisplayedRow = 0;
                        DisplayedOffsetY = 0;
                        break;
                    }
                    if (DisplayedOffsetY + RowsContent[DisplayedRow].HeightCombined < 0)
                    {
                        DisplayedOffsetY += RowsContent[DisplayedRow].HeightCombined;
                    }
                    else
                    {
                        DisplayedRow++;
                        break;
                    }
                }
            }
            else
            {
                while (true)
                {
                    if (DisplayedRow + 1 >= RowsContent.Count)
                    {
                        DisplayedRow = RowsContent.Count - 1;
                        DisplayedOffsetY = 0;
                        break;
                    }
                    if (DisplayedOffsetY - RowsContent[DisplayedRow].HeightCombined > 0)
                    {
                        DisplayedOffsetY -= RowsContent[DisplayedRow].HeightCombined;
                    }
                    else break;
                    DisplayedRow++;
                }
            }
        }


        public void Load(SpreadSheet.ISpreadSheetProvider spread)
        {
            var (c, r) = spread.GetSize();
            this.RowsContent = new Rows(new Row[r]);
            this.ColumnsContent = new Columns(new Column[c]);
            Cells = new Cell[c, r];
            var items = spread.GetCells().ToArray();
            foreach(var item in items)
            {
                Cells[item.RowColumn.Column, item.RowColumn.Row] = new Cell()
                {
                    SizeSpecified = false,
                    Content = item.Text
                };
            }

            this.DisplayedColumn = 0;
            this.DisplayedRow = 0;
            this.DisplayedOffsetX = 0;
            this.DisplayedOffsetY = 0;
            this.DisplayShiftX = 0;
            this.DisplayShiftY = 0;
        }

        public struct Cell
        {
            public float Width;
            public float Height;
            public bool SizeSpecified;
            public string Content;
        }

        public class Rows : List<Row>
        {
            public bool HeaderDisplayed = false;

            public Rows()
            {
            }

            public Rows(IEnumerable<Row> collection) : base(collection)
            {
            }

            public Rows(int capacity) : base(capacity)
            {
            }

            public IEnumerator<double> GetHeights()
            {
                float c = 0;
                foreach (var item in this)
                {
                    yield return c;
                    c += item.Height;
                }
                yield return c;
            }
        }

        public class Columns : List<Column>
        {
            public bool HeaderDisplayed = false;

            public Columns()
            {
            }

            public Columns(IEnumerable<Column> collection) : base(collection)
            {
            }

            public Columns(int capacity) : base(capacity)
            {
            }

            public IEnumerator<double> GetWidths()
            {
                float c = 0;
                foreach (var item in this)
                {
                    yield return c;
                    c += item.Width;
                }
                yield return c;
            }
        }

        public struct Row
        {
            public float Height;
            public float HeightMargin;
            public string Header;
            public float HeightCombined => Height + HeightMargin;
        }

        public struct Column
        {
            public float Width;
            public float WidthMargin;
            public string Header;
            public float WidthCombined => Width + WidthMargin;
        }
    }
}
