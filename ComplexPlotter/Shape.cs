using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ComplexPlotter
{
    abstract class Shape
    {
        public abstract List<PointDO> Values(ViewReckoner vr);

        public virtual bool CursorIsOn(PlotterGraphics pg, Point p, int tol) { return false; }
        public virtual void Select(PlotterGraphics pg) { }

        public virtual void Deselect(PlotterGraphics pg)
        {
            if (Selected)
            {
                Selected = false;
                Draw(pg);
            }
        }
        public virtual void Move(PlotterGraphics pg, Point offset) { }

        public bool Selected { get; protected set; }

        public virtual void Draw(PlotterGraphics g) { }
    }

    // line shape, in screen co-ordinates
    class Line : Shape
    {
        private Point _p0;
        private Point _p1;
        private SelectMode _mode;
        private Color _color;

        private enum SelectMode
        {
            P0,
            P1,
            Line
        }

        public Line(Point p0, Point p1)
        {
            _p0 = p0;
            _p1 = p1;
            _mode = SelectMode.Line;
            _color = Color.Blue;
        }
        public Line(Point p0, Point p1, Color color)
        {
            _p0 = p0;
            _p1 = p1;
            _mode = SelectMode.Line;
            _color = color;
        }

        public override List<PointDO> Values(ViewReckoner vr)
        {
            const int n = 50;

            List<PointDO> values = new List<PointDO>();

            double x0 = vr.XCToV(_p0.X);
            double x1 = vr.XCToV(_p1.X);
            double y0 = vr.YCToV(_p0.Y);
            double y1 = vr.YCToV(_p1.Y);

            double xd = (x1 - x0) / (double)n;
            double yd = (y1 - y0) / (double) n;

            for (int i = 0; i <= n; ++i)
                values.Add(new PointDO(x0 + (double) i * xd, y0 + (double) i * yd, _color));

            return values;
        }

        public override void Draw(PlotterGraphics pg)
        {
            pg.DrawLineC(_p0, _p1, _color);
            if (Selected && _mode == SelectMode.P0)
                pg.DrawCircleC(_p0, 5);
            if (Selected && _mode == SelectMode.P1)
                pg.DrawCircleC(_p1, 5);
        }

        public override bool CursorIsOn(PlotterGraphics pg, Point p, int tol)
        {
            Complex pc = new Complex((double)p.X, (double)p.Y);
            Complex c0 = new Complex((double)_p0.X, (double)_p0.Y);
            Complex c1 = new Complex((double)_p1.X, (double)_p1.Y);
            Complex xt = new Complex((double)tol, 0.0);
            Complex yt = new Complex(0.0, (double)tol);

            // has the p0 end been selected
            Complex ce = Complex.Subtract(pc, c0);
            if (ce.Magnitude < tol)
            {
                _mode = SelectMode.P0;
                return true;
            }

            // has the p1 end been selected
            ce = Complex.Subtract(pc, c1);
            if (ce.Magnitude < tol)
            {
                _mode = SelectMode.P1;
                return true;
            }

            // make c0 the left most point, i.e. c0 has smallest real value
            if (c0.Real > c1.Real)
            {
                Complex c = c0; c0 = c1; c1 = c;
            }

            // move origin to c0
            Complex c1p = Complex.Subtract(c1, c0);
            Complex pp = Complex.Subtract(pc, c0);

            // rotate so that c1p lies on real axis
            Complex r = new Complex(c1p.Real / c1p.Magnitude, -c1p.Imaginary / c1p.Magnitude);
            c1p = Complex.Multiply(c1p, r);
            pp = Complex.Multiply(pp, r);
            xt = Complex.Multiply(xt, r);
            yt = Complex.Multiply(yt, r);

            // take worst case of tolerances in x and y directions
            double dxp = Math.Max(Math.Abs(xt.Real), Math.Abs(yt.Real));
            double dyp = Math.Max(Math.Abs(xt.Imaginary), Math.Abs(yt.Imaginary));

            // is pp within tolerance of the line c0 (origin) to c1p
            if (pp.Real > -dxp && pp.Real < c1p.Real + dxp &&
                   pp.Imaginary > -dyp && pp.Imaginary < c1p.Imaginary + dyp)
            {
                _mode = SelectMode.Line;
                return true;
            }
            else
                return false;
        }

        public override void Select(PlotterGraphics pg)
        {
            Selected = true;
            Draw(pg);
        }

        public override void Move(PlotterGraphics pg, Point offset)
        {
            if (Selected)
            {
                switch (_mode)
                {
                    case SelectMode.Line:
                        _p0 = new Point(_p0.X + offset.X, _p0.Y + offset.Y);
                        _p1 = new Point(_p1.X + offset.X, _p1.Y + offset.Y);
                        break;
                    case SelectMode.P0:
                        _p0 = new Point(_p0.X + offset.X, _p0.Y + offset.Y);
                        break;
                    case SelectMode.P1:
                        _p1 = new Point(_p1.X + offset.X, _p1.Y + offset.Y);
                        break;
                }
                Draw(pg);
            }
        }
    }

    // a set of disconnected lines
    class Lines : Shape
    {
        private List<Line> _lines;

        public Lines()
        {
            _lines = new List<Line>();
        }

        public Lines(Point p1, Point p2)
        {
            _lines = new List<Line>();
            Add(p1, p2);
        }

        public void Add(Point p1, Point p2)
        {
            _lines.Add(new Line(p1, p2));
        }

        public void Add(Point p1, Point p2, Color color)
        {
            _lines.Add(new Line(p1, p2, color));
        }

        public override List<PointDO> Values(ViewReckoner vr)
        {
            List<PointDO> values = new List<PointDO>();
            foreach (Line l in _lines)
            {
                List<PointDO> vs = l.Values(vr);
                values.AddRange(vs);
                values.Add(new PointDO()); // discontinuity
            }
            return values;
        }

        public override void Draw(PlotterGraphics g)
        {
            foreach (Line l in _lines) l.Draw(g);
        }

        public override bool CursorIsOn(PlotterGraphics pg, Point p, int tol)
        {
            foreach (Line l in _lines)
                if (l.CursorIsOn(pg, p, tol)) return true;
            return false;
        }

        public override void Select(PlotterGraphics pg)
        {
            foreach (Line l in _lines) l.Select(pg);
            Selected = true;
        }
        public override void Deselect(PlotterGraphics pg)
        {
            foreach (Line l in _lines) l.Deselect(pg);
            Selected = false;
        }

        public override void Move(PlotterGraphics pg, Point offset)
        {
            foreach (Line l in _lines) l.Move(pg, offset);
        }
    }

    class Grid : Lines
    {
        public Grid(
            int xmin, int xmax, int xsteps,
            int ymin, int ymax, int ysteps)
        {
            int xd = (xmax - xmin) / xsteps;
            int x = xmin;
            xmax = xmin + xsteps * xd;

            int yd = (ymax - ymin) / ysteps;
            int y = ymin;
            ymax = ymin + ysteps * yd;

            List<Color> cs = Utilities.ColorList();
            int c = 0;

            for (int i = 0; i <= xsteps; i++)
            {
                Add(new Point(x, ymin), new Point(x, ymax), cs[c]);
                x += xd;
                c = (c + 1) % cs.Count;
            }

            for (int i = 0; i <= ysteps; i++)
            {
                Add(new Point(xmin, y), new Point(xmax, y), cs[c]);
                y += yd;
                c = (c + 1) % cs.Count;

            }
        }
    }


    class Circle : Shape
    {
        private Point _c0;
        private int _r;

        public Circle(Point c0, int r)
        {
            _c0 = c0;
            _r = r;
        }

        public override List<PointDO> Values(ViewReckoner vr)
        {
            const int n = 100;
            List<PointDO> values = new List<PointDO>();

            double arg = 0;
            for (int i = 0; i <= n; i++)
            {
                PointD p = new PointD(
                    (double)_c0.X + ((double) _r) * Math.Cos(arg), 
                    (double)_c0.Y + ((double) _r) * Math.Sin(arg));
                values.Add(new PointDO(vr.CToV(p)));
                arg += (2.0 * Math.PI) / (double) n;
            }
            return values;
        }

        public override void Draw(PlotterGraphics pg)
        {
            pg.DrawCircleC(_c0, _r);
        }

        public override bool CursorIsOn(PlotterGraphics pg, Point p, int tol)
        {
            // p relative to _c0
            Complex pc = Complex.Subtract(new Complex(p.X, p.Y), new Complex(_c0.X, _c0.Y));
            return pc.Magnitude < _r + tol && pc.Magnitude > _r - tol;
        }

        public override void Select(PlotterGraphics pg)
        {
            Selected = true;
            Draw(pg);
        }

        public override void Move(PlotterGraphics pg, Point offset)
        {
            if (Selected)
            {
                _c0.Offset(offset);
                Draw(pg);
            }
        }
    }


}
