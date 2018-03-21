using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MarbleBoardGame
{
    //Analysis
    //Game move list
    //Clock
    public partial class WindowPane : UserControl
    {
        private BufferedGraphicsContext GraphicManager;
        private BufferedGraphics ManagedBackBuffer;
        private WindowHeader windowHeader;

        public WindowPane()
        {
            InitializeComponent();      

            ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);

            ToolStripRadioButtonMenuItem top = new ToolStripRadioButtonMenuItem("Top");
            ContextMenuStrip.Items.Add(top);

            ToolStripRadioButtonMenuItem bottom = new ToolStripRadioButtonMenuItem("Bottom");
            ContextMenuStrip.Items.Add(bottom);

            this.windowHeader = new WindowHeader(ContextMenuStrip, PointToScreen);
            windowHeader.OnCloseClicked += windowHeader_OnCloseClicked;

            this.Text = "WindowPane";

            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            GraphicManager = BufferedGraphicsManager.Current;
            GraphicManager.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            ManagedBackBuffer = GraphicManager.Allocate(this.CreateGraphics(), ClientRectangle);
        }

        private void windowHeader_OnCloseClicked(object sender, EventArgs e)
        {
            if (this.ParentForm is MainForm)
            {
                MainForm mainForm = this.ParentForm as MainForm;
                mainForm.HidePanels();
            }
        }

        public string Text { get { return windowHeader.Text; } set { windowHeader.Text = value; } }

        protected void RefreshCrossThread()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate() { this.Refresh(); }));
            }
            else
            {
                this.Refresh();
            }
        }

        protected void DrawBackground(Graphics gfx)
        {
            Rectangle c = ClientRectangle;

            Color start = Color.FromArgb(236, 233, 230);
            Color end = SystemColors.Control;

            int centerX = c.X + (c.Width / 2);
            LinearGradientBrush brush = new LinearGradientBrush(new Point(centerX, c.Y), new Point(centerX, c.Bottom), start, end);
            gfx.FillRectangle(brush, c);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawBackground(ManagedBackBuffer.Graphics);
            windowHeader.Draw(ManagedBackBuffer.Graphics, Location.X, Location.Y, Width);

            Rectangle pane = new Rectangle(0, 16, Width, Height - 16);
            PaintPane(ManagedBackBuffer.Graphics, pane);

            ManagedBackBuffer.Render(e.Graphics);
        }

        protected virtual void PaintPane(Graphics gfx, Rectangle clip)
        {

        }

        protected override void OnResize(EventArgs e)
        {
            if (ManagedBackBuffer != null)
            {
                ManagedBackBuffer.Dispose();
            }

            GraphicManager.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            ManagedBackBuffer = GraphicManager.Allocate(this.CreateGraphics(), ClientRectangle);
            this.Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            windowHeader.Update(e);
            this.Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (DesignMode)
            {
                return;
            }

            windowHeader.Update(e);
            this.Refresh();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (DesignMode)
            {
                return;
            }

            windowHeader.Update(e);
            this.Refresh();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (DesignMode)
            {
                return;
            }

            windowHeader.MouseLeave();
            this.Refresh();
        }
    }
}
