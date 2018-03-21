using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarbleBoardGame
{
    public partial class MainForm : Form
    {
        private MarbleBoardControl mbControl;

        public MainForm()
        {
            InitializeComponent();

            AnalysisPane analysisPane = new AnalysisPane();
            analysisPane.Dock = DockStyle.Fill;

            NotationPane notationPane = new NotationPane();
            notationPane.Anchor = AnchorStyles.Bottom;

            mbControl = new MarbleBoardControl();
            mbControl.ClientSize = splitContainer1.Panel1.ClientSize;
            mbControl.AnalysisView = analysisPane;
            mbControl.NotationPane = notationPane;

            if (!DesignMode)
            {
                splitContainer1.Panel1.Controls.Add(mbControl);
                splitContainer1.Panel2.Controls.Add(analysisPane);
                
            }
        }

        public void HidePanels()
        {
            splitContainer1.Panel2Collapsed = true;
            if (this.Width > mbControl.Width)
            {
                this.Width = mbControl.Width;
            }
        }

        protected override void OnResizeBegin(EventArgs e)
        {
            mbControl.ResizeBegin();
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            mbControl.ResizeEnd();
        }

        private void splitContainer1_Panel1_Resize(object sender, EventArgs e)
        {
            if (mbControl == null || DesignMode)
            {
                return;
            }

            int minValue = Math.Min(splitContainer1.Panel1.Width, splitContainer1.Panel1.Height);
            if (mbControl.Width < minValue)
            {
                mbControl.Width = minValue;
            }
            if (mbControl.Height < minValue)
            {
                mbControl.Height = minValue;
            }


            mbControl.Size = minValue;
            mbControl.Resize();


        }

        private void rotateBoardBtn_Click(object sender, EventArgs e)
        {
            BoardView view = mbControl.GetBoard();
            view.Rotate();
        }

        private void diceSoundsCheckBox_CheckBoxCheckChanged(object sender, EventArgs e)
        {
            BoardView view = mbControl.GetBoard();
        }

        private void marbleSoundsCheckBox_CheckBoxCheckChanged(object sender, EventArgs e)
        {
            BoardView view = mbControl.GetBoard();
        }

        private void notationOption_Click(object sender, EventArgs e)
        {
            notationOption.Checked = !notationOption.Checked;
        }

        private void analysisOption_Click(object sender, EventArgs e)
        {
            analysisOption.Checked = !analysisOption.Checked;
        }
    }
}
