using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Resources;
using System.Numerics;

namespace ComplexPlotter
{
    interface IDrawable
    {
        void Draw(PlotterGraphics g);
    }

    class Plotter2d
    {

        private Control _plot;
        private PointD _origin;     // the axis origin as a complex number, starts at (0,0)
        private MathFunction _f;
        private ViewReckoner _vr;
        private Axes _axes;
        private int _noOfTicks = 20;
        private int _border = 10;

        // rubber band selection
        private Bitmap _saveImage;
        Point? _bandStart;

        Shape _shape;

        public Plotter2d(Control plot, double xExtent, double yExtent)
        {
            double xmin = -xExtent / 2.0;
            double xmax = xExtent / 2.0;
            double ymin = -yExtent / 2.0;
            double ymax = yExtent / 2.0;

            _plot = plot;
            _origin = new PointD(0.0, 0.0); 
            _vr = new ViewReckoner(
                _border, plot.Width - _border, plot.Height - _border, _border, 
                xmin, xmax, ymin, ymax);
            _plot.BackColor = Color.White;
            _plot.Paint += new PaintEventHandler(OnPaint);

            // create the axis
            _axes = new Axes();
            _axes.XAxis = new Axis(xmin, xmax, _origin.X, _noOfTicks);
            _axes.YAxis = new Axis(ymin, ymax, _origin.Y, _noOfTicks);

            // rubber band
            _plot.MouseDown += new MouseEventHandler(OnMouseDown);
            _plot.MouseUp += new MouseEventHandler(OnMouseUp);
            _plot.MouseMove += new MouseEventHandler(OnMouseMove);
            _bandStart = null;

            _shape = new Grid(0.0, xmax / 2.0, 4, 0.0, ymax / 2.0, 4);
            //_shape = new Line(new PointD(1.0, 1.0), new PointD(2.0, 6.0));

            //_plot.Refresh();
            Replot();
        }

        public MathFunction f {  set { _f = value; }  get { return _f; } }

        private double XSize { get { return _vr.VXSize; } }
        private double YSize { get { return _vr.VYSize; } }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            PlotterGraphics pg = new PlotterGraphics(_vr, e.Graphics);
            ReplotG(pg);
        }

        public ViewReckoner View { get { return _vr; } }

        public void SetXExtent(RangeD ex)
        {
            _vr.SetXExtent(ex);
            Replot();
        }
        public void SetYExtent(RangeD ex)
        {
            // recalculate Y axis, update the view extent and redraw
            _axes.YAxis.Initialise(ex.Min, ex.Max, _origin.Y, _noOfTicks);
            _vr.SetYExtent(new RangeD(_axes.YAxis.Min, _axes.YAxis.Max));
            Replot();
        }

        public void Clear()
        {
            PlotterGraphics pg = new PlotterGraphics(_vr, _plot.CreateGraphics());
            ClearG(pg);
        }

        private void ClearG(PlotterGraphics pg)
        {
            pg.Clear();
        }

        public void Replot()
        {
            PlotterGraphics pg = new PlotterGraphics(_vr, _plot.CreateGraphics());
            ReplotG(pg);
        }

        public void ReplotG(PlotterGraphics pg)
        {
            ClearG(pg);
            _axes.Draw(pg);
            _shape.Draw(pg);
            if (_f != null) _f.Draw(pg);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift)
            {
                _saveImage = new Bitmap(_plot.Width, _plot.Height);
                _plot.DrawToBitmap(_saveImage, new Rectangle(0, 0, _plot.Width, _plot.Height));
                _bandStart = e.Location;
            }
            else if (Control.ModifierKeys == Keys.None)
            {

                // if (_shape.DragStart(_vr.CToV(e.Location), _vr.CXLenToV(5), _vr.CYLenToV(5)))
                if (_shape.CanSelect(_vr.CToV(e.Location), _vr.VXSize / 100, _vr.VYSize / 100))
                {
                    Logger.Info("Close");
                }
                else
                {
                    Logger.Info("-");
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_bandStart.HasValue)
            {
                // clear previous band
                Graphics g = _plot.CreateGraphics();
                g.DrawImage(_saveImage, 0, 0);
                g.DrawRectangle(new Pen(Color.Black, 1.0f),
                    _bandStart.Value.X, _bandStart.Value.Y,
                    e.Location.X - _bandStart.Value.X, e.Location.Y - _bandStart.Value.Y);
            }
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_bandStart.HasValue)
            {
                PlotterGraphics pg = new PlotterGraphics(_vr, _plot.CreateGraphics());
                pg.Graphics.DrawImage(_saveImage, 0, 0);

                // re-calculate x axis
                int l = _bandStart.Value.X;
                int u = e.X;
                if (l > u) (l, u) = (u, l);
                _axes.XAxis.Initialise(_vr.XCToV(l), _vr.XCToV(u), _origin.X, _noOfTicks);
                _vr.SetXExtent(new RangeD(_axes.XAxis.Min, _axes.XAxis.Max));

                // re-calculate y axis
                l = _bandStart.Value.Y;
                u = e.Y;
                if (l > u) (l, u) = (u, l);
                _axes.YAxis.Initialise(_vr.YCToV(u), _vr.YCToV(l), _origin.Y, _noOfTicks);
                _vr.SetYExtent(new RangeD(_axes.YAxis.Min, _axes.YAxis.Max));

                // redraw
                ReplotG(pg);

                _bandStart = null;
            }
        }
    }
}
