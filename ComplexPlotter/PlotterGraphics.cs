using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ComplexPlotter
{
    class PlotterGraphics
    {
        private ViewReckoner _vr;
        private Graphics _graphics;
        private Pen _narrowPen;
        private Pen _doublePen;
        private SolidBrush _solidWhiteBrush;

        public PlotterGraphics(ViewReckoner vr, Graphics g)
        {
            _vr = vr;
            _graphics = g;
        }

        public Graphics Graphics { get { return _graphics; } }
        public ViewReckoner VR { get { return _vr; } }

        public void DrawLineV(PointD p0, PointD p1)
        {
            Point c0 = _vr.VToC(p0);
            Point c1 = _vr.VToC(p1);
            if (_vr.CInView(c0) && _vr.CInView(c1))
                _graphics.DrawLine(NarrowPen(), _vr.VToC(p0), _vr.VToC(p1));
        }

        public void DrawLineC(Point p0, Point p1)
        {
            _graphics.DrawLine(NarrowPen(), p0, p1);
        }

        public void DrawLineC(Point p0, Point p1, Pen p)
        {
            _graphics.DrawLine(p, p0, p1);
        }
        public void DrawLineC(Point p0, Point p1, Color c)
        {
            Pen p = new Pen(c, 1.0f);
            _graphics.DrawLine(p, p0, p1);
        }

        public void DrawLineC(int x0, int y0, int x1, int y1)
        {
            _graphics.DrawLine(NarrowPen(), x0, y0, x1, y1);
        }

        public void DrawLineC(int x0, int y0, int x1, int y1, Pen p)
        {
            _graphics.DrawLine(p, x0, y0, x1, y1);
        }

        public void DrawEllipseC(int x0, int y0, int x1, int y1)
        {
            _graphics.DrawEllipse(NarrowPen(), x0, y0, x1, y1);
        }
        public void DrawCircleC(Point centre, int radius)
        {
            _graphics.DrawEllipse(NarrowPen(),
                centre.X - radius, centre.Y - radius, 2 * radius, 2 * radius);
        }

        public void Clear()
        {
            _graphics.Clear(Color.White);
        }

        public Pen NarrowPen()
        {
            if (_narrowPen == null)
                _narrowPen = new Pen(Color.Black, 1.0f);
            return _narrowPen;
        }

        public Pen DoublePen()
        {
            if (_doublePen == null)
                _doublePen = new Pen(Color.Black, 2.0f);
            return _doublePen;
        }

        public SolidBrush SolidWhiteBrush()
        {
            if (_solidWhiteBrush == null)
                _solidWhiteBrush = new SolidBrush(Color.White);
            return _solidWhiteBrush;
        }

        public void PlotPoints(List<PointDO> points)
        {
            PointDO p0 = new PointDO();
            Pen pen = null;

            foreach (PointDO p1 in points)
            {
                if (!p0.Discontinuity)
                {
                    // does pen need to change colour
                    if (pen == null || pen.Color != p0.Color)
                        pen = new Pen(p0.Color, 1.0f);

                    if (!p1.Discontinuity)
                    {
                        Point c0 = _vr.VToC(p0.PointD);
                        Point c1 = _vr.VToC(p1.PointD);
                        if (_vr.CInView(c0) && _vr.CInView(c1))
                            _graphics.DrawLine(pen, c0, c1);
                    }
                }
                p0 = p1;
            }
        }
    }
}
