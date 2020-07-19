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

        public Graphics Graphics {  get { return _graphics;  } }
        public ViewReckoner VR {  get { return _vr; } }

        public void DrawLineV(PointD p0, PointD p1)
        {
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

        public void Clear()
        {
            _graphics.FillRectangle(SolidWhiteBrush(), _vr.CXMin, _vr.CYMax, _vr.CXMax, _vr.CYMin);
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
    }
}
