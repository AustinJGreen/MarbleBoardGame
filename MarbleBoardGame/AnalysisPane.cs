using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarbleBoardGame
{
    public class AnalysisPane : WindowPane
    {
        public AnalysisPane() : base()
        {
            this.Text = "Analysis";
            variations = null;
        }

        private void Lerp(Vector4 from, Vector4 next, double amount)
        {
            for (int t = 0; t < 4; t++)
            {
                variations[0].Eval[t] = from[t] + ((next[t] - from[t]) * amount);
            }
        }

        private Task Lerp(Variation from, Variation next, int timeMs)
        {
            return Task.Run(delegate(){

                Stopwatch sw = Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < timeMs)
                {              
                    double elapsedPercent = sw.ElapsedMilliseconds / (double)timeMs;
                    Lerp(from.Eval, next.Eval, elapsedPercent);
                    this.RefreshCrossThread();
                }
            });
        }

        private Variation[] variations;
        public Variation[] Variations
        {
            get
            {
                return variations;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                if (variations != null)
                {
                    Task t = this.Lerp(variations[0], value[0], 1000);
                    t.ContinueWith(delegate(Task task)
                    {
                        variations = value;
                        this.RefreshCrossThread();
                    });

                }
                else
                {
                    variations = value;
                    this.RefreshCrossThread();
                }
            }
        }

        public Algorithm Algorithm { get; set; }

        protected void DrawRoundedRect(Graphics gfx, Brush brush, int x, int y, int width, int height, int curveLength)
        {
            Point[] pts=  new Point[]
            {
                new Point(x, y + height),
                new Point(x, y + curveLength),
                new Point(x + curveLength, y + curveLength),
                new Point(x + curveLength, y),
                new Point(x + width - curveLength, y),
                new Point(x + width - curveLength, y + curveLength),
                new Point(x + width, y + curveLength),
                new Point(x + width, y + height),
                new Point(x, y + height)
            };

            gfx.FillPolygon(brush, pts);

            const float p = 1f;
            gfx.FillPie(brush, x, y,  (curveLength * 2) + p,  (curveLength * 2) + p, 180f, 90f);
            gfx.FillPie(brush, x + width - ((curveLength * 2) + p), y, (curveLength * 2) + p, (curveLength * 2) + p, 270f, 90f);
        }

        protected void DrawGraphLines(Graphics gfx, int x, int y, int width, int height)
        {
            int lines = 8;
            int heightEa = height / lines;

            Pen lightPen = new Pen(Color.DarkGray, 1f);
            SolidBrush brush = new SolidBrush(Color.DarkGray);
            Font font = new Font("Consolas", 10f);
            for (int l = 0; l < lines; l++)
            {
                gfx.DrawLine(lightPen, x, y + (heightEa * (l + 1)), x + width, y + (heightEa * (l + 1)));

                if (l != lines - 1) //Dont draw zero line
                {
                    double evalLvl = Math.Round((5.0 / lines) * (lines - l - 1), 3);
                    gfx.DrawString(evalLvl.ToString(), font, brush, new PointF(x + 10, y + (heightEa * (l + 1)) - 16));
                }
            }

            Pen borderPen = new Pen(Color.Black, 2f);
            gfx.DrawRectangle(borderPen, x + (borderPen.Width / 2), y, width - (borderPen.Width * 1.5f), height);
        }

        protected void DrawColumn(Graphics gfx, int x, int y, int width, int height, Color color)
        {
            LinearGradientBrush brush = new LinearGradientBrush(new Point(x, y), new Point(x + width, y + height), color, Color.Black);
            DrawRoundedRect(gfx, brush, x, y, width, height, width / 4);
        }

        protected void DrawColumnData(Graphics gfx, int x, int y, int width, int height, double eval)
        {

        }

        protected void DrawGraph(Graphics gfx, Rectangle clip)
        {
            int uiSpacing = 50;
            int spacing = (int)(clip.Width * 0.1);

            int padding = 10;
            
            int width = (clip.Width - (padding * 2) - (spacing * (4 - 1))) / 4;
            width -= (uiSpacing / 4);

            DrawGraphLines(gfx, clip.X, clip.Y, clip.Width, clip.Height);

            Color[] colors = new Color[] { 
                Color.FromArgb(255, 204, 0),
                Color.FromArgb(204, 0, 0),
                Color.FromArgb(0, 204, 0),
                Color.FromArgb(0, 0, 204)
            };

            if (variations != null)
            {
                for (int t = 0; t < 4; t++)
                {
                    double teamEval = variations[0].Eval[t];
                    double height = (clip.Height - (padding * 2)) / (5 / teamEval);

                    int y = (clip.Y + clip.Height) - (padding + (int)height);
                    if (height > (width / 4) + 1)
                    {
                        DrawColumn(gfx, clip.X + uiSpacing + padding + (width * t) + (spacing * t), y, width, (int)height, colors[t]);
                    }
                }
            }
        }

        protected float DrawLabel(Graphics gfx, string text, float x, float y, Brush brush = null, Brush bgBrush = null)
        {
            Font font = new Font("Arial", 9f, FontStyle.Bold);

            const float padding = 2;
            SizeF sz = gfx.MeasureString(text, font);

            if (x + sz.Width > this.Width - 10)
            {
                return x;
            }

            RectangleF rect = new RectangleF(x - padding, y - padding, sz.Width + (padding * 2), sz.Height + (padding * 2));

            if (bgBrush == null)
            {
                bgBrush = new LinearGradientBrush(rect, SystemColors.ControlLight, SystemColors.ControlDark, LinearGradientMode.ForwardDiagonal);
            }

            gfx.FillRectangle(bgBrush, rect);

            SolidBrush gradientPenBrush = new SolidBrush(Color.FromArgb(145 - 20, 143 - 20, 138 - 20));
            gfx.DrawString(text, font, brush ?? gradientPenBrush, new PointF(x, y));
            return x + sz.Width + padding;
        }

        protected void DrawVariation(Graphics gfx, Variation variation, int team, float y, Brush brush)
        {
            Font font = new Font("Times New Roman", 9f);

            string moveStr = variation.Move.GetNotation();
            string evalStr = string.Concat(" ", Math.Round(variation.Eval[team], 2).ToString());
            SizeF evalSz = gfx.MeasureString(evalStr, font);

            const string sep = ".";

            int lineWidth = Width - 40;

            StringBuilder variationStr = new StringBuilder();
            variationStr.Append(moveStr);

            SizeF sz = gfx.MeasureString(variationStr.ToString(), font);
            while (sz.Width < lineWidth - (evalSz.Width * 2))
            {
                variationStr.Append(sep);
                sz = gfx.MeasureString(variationStr.ToString(), font);
            }

            gfx.DrawString(variationStr.ToString(), font, brush, new PointF(30, y));
            gfx.DrawString(evalStr, font, brush, new PointF(lineWidth - evalSz.Width, y));
        }

        protected void DrawEvalData(Graphics gfx, Rectangle clip)
        {
            Pen borderPen = new Pen(Color.Black, 2f);
            gfx.DrawRectangle(borderPen, clip.X + (borderPen.Width / 2), clip.Y, clip.Width - (borderPen.Width * 1.5f), clip.Height);

            if (Algorithm != null)
            {
                float next = clip.X;
                next = DrawLabel(gfx, Algorithm.GetType().Name, next + 10, clip.Y + 10);
                next = DrawLabel(gfx, string.Format("{0}k Nodes", Math.Round(Algorithm.Nodes / 1000.0, 1)), next + 10, clip.Y + 10);
                next = DrawLabel(gfx, string.Format("{0}k Nps", Math.Round(Algorithm.NodesPerSecond / 1000.0, 1)), next + 10, clip.Y + 10);

                LinearGradientBrush fade = new LinearGradientBrush(clip, Color.Gray, Color.DarkGray, LinearGradientMode.Vertical);
                Pen fadePen = new Pen(fade, 2f);
                gfx.DrawRectangle(fadePen, clip.X + 8, clip.Y + 36, clip.Width - 16, clip.Height - 44);

                Brush backgroundBrush = null;

                if (variations != null)
                {
                    int dir = variations[0].Eval.GetDirection();
                    if (dir != -1)
                    {
                        switch (dir)
                        {
                            case Board.YELLOW:
                                backgroundBrush = new LinearGradientBrush(clip, Color.FromArgb(255, 221, 0), Color.FromArgb(255, 187, 0), LinearGradientMode.ForwardDiagonal);
                                break;
                            case Board.RED:
                                backgroundBrush = new LinearGradientBrush(clip, Color.FromArgb(255, 0, 0), Color.FromArgb(150, 25, 25), LinearGradientMode.ForwardDiagonal);
                                break;
                            case Board.GREEN:
                                backgroundBrush = new LinearGradientBrush(clip, Color.FromArgb(0, 255, 0), Color.FromArgb(25, 150, 25), LinearGradientMode.ForwardDiagonal);
                                break;
                            case Board.BLUE:
                                backgroundBrush = new LinearGradientBrush(clip, Color.FromArgb(0, 0, 255), Color.FromArgb(25, 25, 150), LinearGradientMode.ForwardDiagonal);
                                break;
                        }

                        double score = Math.Round(variations[0].Eval.GetMagnitude((sbyte)dir), 2);
                        next = DrawLabel(gfx, score.ToString(), next + 10, clip.Y + 10, SystemBrushes.Control, backgroundBrush);
                    }

                    //Draw each variation
                    float y = clip.Y + 50;

                    for (int j = 0; j < variations.Length; j++)
                    {
                        DrawVariation(gfx, variations[j], Algorithm.Player, y, Brushes.Gray);
                        y += 20;
                    }
                }
            }
        }

        protected override void PaintPane(Graphics gfx, Rectangle clip)
        {
            if (!DesignMode)
            {
                Rectangle graphClip = new Rectangle(clip.X, clip.Y, clip.Width, clip.Height / 2);
                DrawGraph(gfx, graphClip);

                Rectangle evalDataClip = new Rectangle(clip.X, clip.Y + (clip.Height / 2), clip.Width, clip.Height / 2);
                DrawEvalData(gfx, evalDataClip);
            }
        }
    }
}
