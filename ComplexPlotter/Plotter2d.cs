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
        private bool _band;
        private Point _mouseDownAt;
        private Point _mouseMoveAt;

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

            Point oc =_vr.VToC(_origin);
            _shape = new Grid(oc.X, oc.X + _vr.CXSize / 4, 4, oc.Y, oc.Y - _vr.CYSize / 4, 4);
            //_shape = new Line(oc, new Point(oc.X + _vr.CXSize / 4, oc.Y - _vr.CYSize / 4));

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
            PlotterGraphics pg = new PlotterGraphics(_vr, _plot.CreateGraphics());
            _mouseDownAt = e.Location;
            _mouseMoveAt = e.Location;

            if (Control.ModifierKeys == Keys.Shift)
            {
                _saveImage = new Bitmap(_plot.Width, _plot.Height);
                _plot.DrawToBitmap(_saveImage, new Rectangle(0, 0, _plot.Width, _plot.Height));
                _band = true;
            }
            else if (Control.ModifierKeys == Keys.None)
            {
                if (_shape.CursorIsOn(pg, e.Location, _vr.CXSize / 100, _vr.CYSize / 100))
                    _shape.Select(pg);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_band)
            {
                // clear previous band
                PlotterGraphics pg = new PlotterGraphics(_vr, _plot.CreateGraphics());
                pg.Graphics.DrawImage(_saveImage, 0, 0);
                pg.Graphics.DrawRectangle(new Pen(Color.Black, 1.0f),
                    _mouseDownAt.X, _mouseDownAt.Y,
                    e.Location.X - _mouseDownAt.X, e.Location.Y - _mouseDownAt.Y);
            }
            else
            {
                if (_shape.Selected)
                {
                    PlotterGraphics pg = new PlotterGraphics(_vr, _plot.CreateGraphics());
                    Point offset = new Point(e.Location.X - _mouseMoveAt.X, e.Location.Y - _mouseMoveAt.Y);
                    _shape.Move(pg, offset);
                    _mouseMoveAt = e.Location;
                    ReplotG(pg);
                }
            }
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_band)
            {
                PlotterGraphics pg = new PlotterGraphics(_vr, _plot.CreateGraphics());
                pg.Graphics.DrawImage(_saveImage, 0, 0);

                // re-calculate x axis
                int l = _mouseDownAt.X;
                int u = e.X;
                if (l > u) (l, u) = (u, l);
                _axes.XAxis.Initialise(_vr.XCToV(l), _vr.XCToV(u), _origin.X, _noOfTicks);
                _vr.SetXExtent(new RangeD(_axes.XAxis.Min, _axes.XAxis.Max));

                // re-calculate y axis
                l = _mouseDownAt.Y;
                u = e.Y;
                if (l > u) (l, u) = (u, l);
                _axes.YAxis.Initialise(_vr.YCToV(u), _vr.YCToV(l), _origin.Y, _noOfTicks);
                _vr.SetYExtent(new RangeD(_axes.YAxis.Min, _axes.YAxis.Max));

                // redraw
                ReplotG(pg);

                _band = false;
            }
            else
            {
                PlotterGraphics pg = new PlotterGraphics(_vr, _plot.CreateGraphics());
                _shape.Deselect(pg);
                ReplotG(pg);
            }
        }
    }
}
