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

        private PictureBox _plot;
        private PointD _origin;     // the axis origin as a complex number, starts at (0,0)
        private MathFunction _f;
        private ViewReckoner _vr;
        private Axes _axes;
        private int _noOfTicks = 20;
        private int _border = 10;

        // rubber band selection
        private bool _band;
        private Point _mouseDownAt;
        private Point _mouseMoveAt;

        // image planes
        private Bitmap _mainImage;
        private Bitmap _overlayImage;

        Shape _shape;

        public Plotter2d(PictureBox plot, double xExtent, double yExtent)
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

            // image planes
            _mainImage = new Bitmap(_plot.Width, _plot.Height);
            _overlayImage = new Bitmap(_plot.Width, _plot.Height);

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

        private PlotterGraphics MainGraphics()
        {
            return new PlotterGraphics(_vr, Graphics.FromImage(_mainImage));
        }

        private PlotterGraphics OverlayGraphics()
        {
            return new PlotterGraphics(_vr, Graphics.FromImage(_overlayImage));
        }

        private void ClearOverlay()
        {
            _overlayImage = new Bitmap(_plot.Width, _plot.Height);
        }

        public void DisplayImage()
        {
            Bitmap img = new Bitmap(_plot.Width, _plot.Height);
            Graphics g = Graphics.FromImage(img);
            g.DrawImage(_mainImage, new Rectangle(0, 0, _plot.Width, _plot.Height));
            g.DrawImage(_overlayImage, new Rectangle(0, 0, _plot.Width, _plot.Height));
            _plot.Image = img;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);
            g.DrawImage(_mainImage, 0, 0);
            g.DrawImage(_overlayImage, 0, 0);
        }

        public ViewReckoner View { get { return _vr; } }

        public void SetYExtent(RangeD ex)
        {
            // recalculate Y axis, update the view extent and redraw
            _axes.YAxis.Initialise(ex.Min, ex.Max, _origin.Y, _noOfTicks);
            _vr.SetYExtent(new RangeD(_axes.YAxis.Min, _axes.YAxis.Max));
            Replot();
        }

        public void Replot()
        {
            PlotterGraphics pg = MainGraphics();
            pg.Clear();
            _axes.Draw(pg);
            if (_f != null) _f.Draw(pg);
            pg.Graphics.Dispose();

            pg = OverlayGraphics();
            _shape.Draw(pg);
            pg.Graphics.Dispose();


            DisplayImage();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            PlotterGraphics pg = new PlotterGraphics(_vr, _plot.CreateGraphics());
            _mouseDownAt = e.Location;
            _mouseMoveAt = e.Location;

            if (Control.ModifierKeys == Keys.Shift)
            {
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
                ClearOverlay();
                PlotterGraphics pg = OverlayGraphics();
                pg.Graphics.DrawRectangle(new Pen(Color.Black, 1.0f),
                    _mouseDownAt.X, _mouseDownAt.Y,
                    e.Location.X - _mouseDownAt.X, e.Location.Y - _mouseDownAt.Y);
                DisplayImage();
            }
            else
            {
                if (_shape.Selected)
                {
                    ClearOverlay();
                    PlotterGraphics pg = OverlayGraphics();
                    Point offset = new Point(e.Location.X - _mouseMoveAt.X, e.Location.Y - _mouseMoveAt.Y);
                    _shape.Move(pg, offset);
                    DisplayImage();
                    _mouseMoveAt = e.Location;
                }
            }
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_band)
            {
                // clear the rubber band
                ClearOverlay();

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
                Replot();
                _band = false;
            }
            else
            {
                ClearOverlay();
                PlotterGraphics pg = OverlayGraphics();
                _shape.Deselect(pg);
                DisplayImage();
            }
        }
    }
}
