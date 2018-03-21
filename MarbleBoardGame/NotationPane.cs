using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarbleBoardGame
{
    public partial class NotationPane : WindowPane
    {
        public NotationPane() : base()
        {
            this.Text = "Notation";
            moveList = new List<Move>();
        }

        private List<Move> moveList;

        public void AddMove(Move move)
        {
            moveList.Add(move);
            RefreshCrossThread();
        }

        protected void DrawMoveList(Graphics gfx, Rectangle clip)
        {
            const float padding = 15;
            Font font = new Font("Consolas", 10f);

            float curX = padding;
            float curY = padding;

            for (int m = 0; m < moveList.Count; m++)
            {
                Move move = moveList[m];

                int moveNumber = m + 1;
                string mStr = string.Concat(moveNumber, '.', ' ', move.GetNotation());
                float length = gfx.MeasureString(mStr, font).Width + 15;

                if (curX + length > clip.Width - padding)
                {
                    curX = padding;
                    curY += font.Height + 5;

                    if (curY >= clip.Height - padding)
                    {
                        break;
                    }
                }

                gfx.DrawString(mStr, font, Brushes.Black, new PointF(curX, curY));
                curX += length;
            }
        }

        protected override void PaintPane(Graphics gfx, Rectangle clip)
        {
            if (!DesignMode)
            {
                DrawMoveList(gfx, clip);
            }
        }
    }
}
