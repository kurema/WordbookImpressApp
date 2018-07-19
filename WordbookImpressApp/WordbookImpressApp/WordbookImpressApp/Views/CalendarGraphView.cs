using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp.Views.Forms;

namespace WordbookImpressApp.Views
{
    public class CalendarGraphView:SKCanvasView
    {
        private SkiaSharp.SKTypeface font;
        public SkiaSharp.SKTypeface Font => font ?? (font = SkiaSharp.SKTypeface.FromStream(typeof(CalendarGraphView).Assembly.GetManifestResourceStream("WordbookImpressApp.Fonts.NotoSansCJKjp_Regular.otf")));

        private SkiaSharp.SKPaint paint;
        public SkiaSharp.SKPaint Paint => paint ?? (paint = new SkiaSharp.SKPaint());

        private string[] dayOfWeekTexts = new[] { "日", "月", "火", "水", "木", "金", "土", };
        public string[] DayOfWeekTexts { get => dayOfWeekTexts; set { if (value.Length < 7) dayOfWeekTexts = value; } }

        public Dictionary<DateTime, SkiaSharp.SKColor> ColorTable { get; set; } = new Dictionary<DateTime, SkiaSharp.SKColor>();
        public SkiaSharp.SKColor DefaultColor { get; set; } = new SkiaSharp.SKColor(250, 250, 250, 255);
        public SkiaSharp.SKColor TextColor { get; set; } = new SkiaSharp.SKColor(0, 0, 0, 180);

        public float FontHeight { get; set; } = 0;
        public float Spacing { get; set; } = 2;
        public float TextMargin = 0;


        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            e.Surface.Canvas.Clear();

            var w = e.Info.Width;
            var h = e.Info.Height;

            float fh = FontHeight == 0 ? h / 10.0f : FontHeight;
            float spacing = Spacing;

            float textMargin = TextMargin == 0 ? h / 14 : TextMargin;

            float cellSize = (h - fh - textMargin) / 7.0f;

            var current = DateTime.Now.Date;
            var init = current;
            int weekCount = 0;

            Paint.Typeface = Font;
            Paint.IsAntialias = true;

            float maxw = 0;

            Paint.Color = TextColor;
            Paint.TextSize = fh;
            Paint.TextAlign = SkiaSharp.SKTextAlign.Right;
            for (int i = 0; i < 7; i++)
            {
                maxw = Math.Max(maxw, Paint.MeasureText(DayOfWeekTexts[i]));
            }
            for (int i = 0; i < 7; i++)
            {
                float x = (h - maxw) % cellSize + maxw - textMargin;
                float y = h - (7 - i) * cellSize + spacing / 2.0f;
                e.Surface.Canvas.DrawText(DayOfWeekTexts[i], x, y + cellSize / 2.0f + fh / 2.0f, Paint);
            }

            while (true)
            {
                if (ColorTable.ContainsKey(current))
                {
                    Paint.Color = ColorTable[current];
                }
                else
                {
                    Paint.Color = DefaultColor;
                }
                var dow = (int)current.DayOfWeek;

                float x = w - (1 + weekCount) * cellSize + spacing / 2.0f;
                float y = h - (7 - dow) * cellSize + spacing / 2.0f;
                if (x < maxw) break;
                e.Surface.Canvas.DrawRect(x, y, cellSize - spacing, cellSize - spacing, Paint);

                if (current.Day == 1)
                {
                    Paint.Color = new SkiaSharp.SKColor(0, 0, 0, 255);
                    Paint.TextSize = fh;
                    Paint.TextAlign = SkiaSharp.SKTextAlign.Left;
                    e.Surface.Canvas.DrawText(current.Month + "月", x, fh, Paint);
                }

                current = current.AddDays(-1);

                if (dow == 0) weekCount++;
            }
        }
    }
}
