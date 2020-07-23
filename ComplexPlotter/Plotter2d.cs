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
using System.Data;

namespace ComplexPlotter
{
    interface IDrawable
    {
        void Draw(PlotterGraphics pg);
    }

    interface IPlotter
    {
        void PlotPoints(List<PointDO> points);
    }

    class ImagePlane
    {
        private Control _control;
        private Bitmap _image;
        private Graphics _graphics;

        public ImagePlane(Control c)
        {
            _control = c;
            _image = null;
            _graphics = null;
        }

        public void Clear()
        {
            _graphics = null;
            _image = null;
        }

        private void Initialise()
        {
            if (_graphics == null)
            {
                _image = new Bitmap(_control.Width, _control.Height);
                _graphics = System.Drawing.Graphics.FromImage(_image);
            }
        }

        public Graphics Graphics
        {
            get
            {
                Initialise();
                return _graphics;
            }
        }

        public Bitmap Image
        {
            get
            {
                Initialise();
                return _image;
            }
        }
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
        private ImagePlane _mainImage;
        private ImagePlane _bandImage;
        private ImagePlane _mapperImage;

        private Shape _mapper;

        IPlotter _plotter;

        public Plotter2d(PictureBox plot, double xExtent, double yExtent, IPlotter plotter)
        {
            double xmin = -xExtent / 2.0;
            double xmax = xExtent / 2.0;
            double ymin = -yExtent / 2.0;
            double ymax = yExtent / 2.0;

            _plot = plot;
            _plotter = plotter;
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

            // create image planes
            _mainImage = new ImagePlane(_plot);
            _bandImage = new ImagePlane(_plot);
            _mapperImage = new ImagePlane(_plot);

            Point oc =_vr.VToC(_origin);
            //_mapper = new Grid(oc.X, oc.X + _vr.CXSize / 4, 4, oc.Y, oc.Y - _vr.CYSize / 4, 4);
            //_mapper = new Line(oc, new Point(oc.X + _vr.CXSize / 4, oc.Y - _vr.CYSize / 4));

            Replot();
        }

        public MathFunction f {  set { _f = value; }  get { return _f; } }

        public PointD Origin {  get { return _origin; } }

        public void SetMapper(Shape mapper)
        {
            _mapper = mapper;
            _mapper.Draw(new PlotterGraphics(_vr, _mapperImage.Graphics));
            DisplayImage();
        }

        public void PlotPoints(List<PointDO> points)
        {
            _mainImage.Clear();
            PlotterGraphics pg = new PlotterGraphics(_vr, _mainImage.Graphics);
            _axes.Draw(pg);
            pg.PlotPoints(points);
            DisplayImage();
        }

        private double XSize { get { return _vr.VXSize; } }
        private double YSize { get { return _vr.VYSize; } }

        public void DisplayImage()
        {
            Bitmap image = new Bitmap(_plot.Width, _plot.Height);
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(_mainImage.Image, new Rectangle(0, 0, _plot.Width, _plot.Height));
            g.DrawImage(_bandImage.Image, new Rectangle(0, 0, _plot.Width, _plot.Height));
            g.DrawImage(_mapperImage.Image, new Rectangle(0, 0, _plot.Width, _plot.Height));
            _plot.Image = image;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {

            //DisplayImage();
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
            // main image plane
            _mainImage.Clear();
            _axes.Draw(new PlotterGraphics(_vr, _mainImage.Graphics));
            if (_f != null) _f.Draw(new PlotterGraphics(_vr, _mainImage.Graphics));

            // the mapper plane
            if (_mapper != null)
            {
                _mapperImage.Clear();
                _mapper.Draw(new PlotterGraphics(_vr, _mapperImage.Graphics));
            }

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
                if (_mapper != null && _mapper.CursorIsOn(pg, e.Location, _vr.CXSize / 100))
                    _mapper.Select(pg);
                else
                {
                    //if (_plotter != null)
                    //{
                    //    List<PointDO> ps = new List<PointDO>();
                    //    ps.Add(new PointDO(_vr.CToV(e.Location)));
                    //    _plotter.PlotPoints(ps);
                    //}
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_band)
            {
                // clear previous band
                _bandImage.Clear();

                // draw new band
                _bandImage.Graphics.
                    DrawRectangle(new Pen(Color.Black, 1.0f),
                        _mouseDownAt.X, _mouseDownAt.Y,
                        e.Location.X - _mouseDownAt.X, e.Location.Y - _mouseDownAt.Y);
                DisplayImage();
            }
            else
            {
                if (_mapper != null && _mapper.Selected)
                {
                    // clear previous mapper
                    _mapperImage.Clear();

                    // draw mapper in new position
                    PlotterGraphics pg = new PlotterGraphics(_vr, _mapperImage.Graphics);
                    Point offset = new Point(e.Location.X - _mouseMoveAt.X, e.Location.Y - _mouseMoveAt.Y);
                    _mapper.Move(pg, offset);
                    DisplayImage();
                    _mouseMoveAt = e.Location;

                    if (_plotter != null)
                        _plotter.PlotPoints(_mapper.Values(_vr));
                }
            }
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_band)
            {
                // clear the rubber band
                _bandImage.Clear();

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
                if (_mapper != null && _mapper.Selected)
                {
                    _mapperImage.Clear();
                    PlotterGraphics pg = new PlotterGraphics(_vr, _mapperImage.Graphics);
                    _mapper.Deselect(pg);
                    DisplayImage();

                    if (_plotter != null)
                        _plotter.PlotPoints(_mapper.Values(_vr));
                }
            }
        }
    }
}
