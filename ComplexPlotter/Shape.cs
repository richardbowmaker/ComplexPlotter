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
        public abstract List<ComplexO> Values();

        public virtual bool CursorIsOn(PlotterGraphics pg, Point p, int xtol, int ytol) { return false; }
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

        public Line(Point p0, Point p1)
        {
            _p0 = p0;
            _p1 = p1;
        }

        public override List<ComplexO> Values()
        {
            List<ComplexO> values = new List<ComplexO>();
            return values;
        }

        public override void Draw(PlotterGraphics pg)
        {
            if (Selected)
                pg.DrawLineC(_p0, _p1, pg.DoublePen());
            else
                pg.DrawLineC(_p0, _p1, pg.NarrowPen());
        }

        public override bool CursorIsOn(PlotterGraphics pg, Point p, int xtol, int ytol)
        {
            Complex pc = new Complex((double)p.X, (double)p.Y);
            Complex c0 = new Complex((double)_p0.X, (double)_p0.Y);
            Complex c1 = new Complex((double)_p1.X, (double)_p1.Y);
            Complex xt = new Complex((double)xtol, 0.0);
            Complex yt = new Complex(0.0, (double)ytol);

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
            return pp.Real > -dxp && pp.Real < c1p.Real + dxp &&
                   pp.Imaginary > -dyp && pp.Imaginary < c1p.Imaginary + dyp;
        }

        public override void Select(PlotterGraphics pg)
        {
            Selected = true;
            Draw(pg);
        }

        public override void Move(PlotterGraphics pg, Point offset)
        {
            _p0 = new Point(_p0.X + offset.X, _p0.Y + offset.Y);
            _p1 = new Point(_p1.X + offset.X, _p1.Y + offset.Y);
            Draw(pg);
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

        public override List<ComplexO> Values()
        {
            List<ComplexO> values = new List<ComplexO>();
            return values;
        }

        public override void Draw(PlotterGraphics g)
        {
            foreach (Line l in _lines) l.Draw(g);
        }

        public override bool CursorIsOn(PlotterGraphics pg, Point p, int xtol, int ytol)
        {
            foreach (Line l in _lines)
                if (l.CursorIsOn(pg, p, xtol, ytol)) return true;
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

            for (int i = 0; i <= xsteps; i++)
            {
                Add(new Point(x, ymin), new Point(x, ymax));
                x += xd;
            }

            for (int i = 0; i <= ysteps; i++)
            {
                Add(new Point(xmin, y), new Point(xmax, y));
                y += yd;
            }
        }
    }

}
