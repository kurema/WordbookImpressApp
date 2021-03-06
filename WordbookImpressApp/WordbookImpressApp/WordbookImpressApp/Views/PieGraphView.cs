﻿using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp.Views.Forms;

namespace WordbookImpressApp.Views
{
    public class PieGraphView : SkiaSharp.Views.Forms.SKCanvasView
    {
        private SkiaSharp.SKTypeface font;
        public SkiaSharp.SKTypeface Font => font ?? (font = SkiaSharp.SKTypeface.FromStream(typeof(PieGraphView).Assembly.GetManifestResourceStream("WordbookImpressApp.Fonts.NotoSansCJKjp_Regular.otf")));

        public struct PieItem
        {
            public float Rate;
            public SkiaSharp.SKColor Color;
        }

        public List<PieItem> Members = new List<PieItem>();

        private SkiaSharp.SKPaint paint;
        public SkiaSharp.SKPaint Paint => paint ?? (paint = new SkiaSharp.SKPaint());

        public float InnerRate = 0.3f;
        public float OuterRate = 0.8f;

        public string Title="";
        public float TitleFontSize=0.15f;

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            try
            {
                var w = e.Info.Width;
                var h = e.Info.Height;
                float cx = w / 2.0f;
                float cy = h / 2.0f;

                float Radius = Math.Min(cx, cy);

                Paint.Typeface = Font;

                float currentRate = 0;
                foreach (var item in Members)
                {
                    paint.IsAntialias = true;
                    {
                        var path = new SkiaSharp.SKPath();
                        if (item.Rate % 1 == 0f && item.Rate > 0.5f)
                        {
                            path.AddCircle(cx, cy, Radius * OuterRate);
                        }
                        else
                        {
                            path.ArcTo(new SkiaSharp.SKRect(cx - Radius * OuterRate, cy - Radius * OuterRate, cx + Radius * OuterRate, cy + Radius * OuterRate), 360 * (currentRate - 0.25f), 360 * item.Rate, false);
                            path.LineTo(cx, cy);
                            //path.LineTo(cx + Radius * (float)Math.Sin(Math.PI * 2 * currentRate) * OuterRate, cy - Radius * (float)Math.Cos(Math.PI * 2 * currentRate) * OuterRate);
                            path.Close();
                        }
                        Paint.Color = item.Color;
                        Paint.IsStroke = false;
                        e.Surface.Canvas.DrawPath(path, Paint);
                    }
                    {
                        var path = new SkiaSharp.SKPath();
                        path.MoveTo(cx + Radius * (float)Math.Sin(Math.PI * 2 * currentRate) * OuterRate, cy - Radius * (float)Math.Cos(Math.PI * 2 * currentRate) * OuterRate);
                        path.LineTo(cx, cy);
                        Paint.Color = new SkiaSharp.SKColor(255, 255, 255, 0);
                        Paint.StrokeWidth = Radius * 0.02f;
                        Paint.IsStroke = true;
                        Paint.BlendMode = SkiaSharp.SKBlendMode.Clear;
                        e.Surface.Canvas.DrawPath(path, Paint);
                        Paint.BlendMode = SkiaSharp.SKBlendMode.SrcOver;
                    }

                    currentRate += item.Rate;
                }
                {
                    Paint.Color = new SkiaSharp.SKColor(255, 255, 255);
                    Paint.IsStroke = false;
                    Paint.BlendMode = SkiaSharp.SKBlendMode.Clear;
                    e.Surface.Canvas.DrawCircle(cx, cy, Radius * InnerRate, Paint);
                    Paint.BlendMode = SkiaSharp.SKBlendMode.SrcOver;
                }
                {
                    var path = new SkiaSharp.SKPath();
                    path.MoveTo(cx + Radius * (float)Math.Sin(Math.PI * 2 * currentRate) * OuterRate, cy - Radius * (float)Math.Cos(Math.PI * 2 * currentRate) * OuterRate);
                    path.LineTo(cx, cy);
                    Paint.Color = new SkiaSharp.SKColor(255, 255, 255, 0);
                    Paint.StrokeWidth = Radius * 0.02f;
                    Paint.IsStroke = true;
                    Paint.BlendMode = SkiaSharp.SKBlendMode.Clear;
                    e.Surface.Canvas.DrawPath(path, Paint);
                    Paint.BlendMode = SkiaSharp.SKBlendMode.SrcOver;
                }
                {
                    Paint.TextSize = TitleFontSize * Radius;
                    Paint.Color = new SkiaSharp.SKColor(0, 0, 0);
                    Paint.IsStroke = false;
                    Paint.TextAlign = SkiaSharp.SKTextAlign.Center;
                    e.Surface.Canvas.DrawText(Title, cx, cy + Paint.FontMetrics.AverageCharacterWidth / 2.0f, Paint);
                }

            }
            catch { }
            base.OnPaintSurface(e);
        }
    }
}
