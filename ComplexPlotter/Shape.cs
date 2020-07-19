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
        public abstract bool CanSelect(PointD p, double xtol, double ytol);

        public abstract void Draw(PlotterGraphics g);
    }

    class Line : Shape
    {
        private PointD _p0;
        private PointD _p1;

        public Line(PointD p0, PointD p1)
        {
            _p0 = p0;
            _p1 = p1;
        }

        public override List<ComplexO> Values()
        {
            List<ComplexO> values = new List<ComplexO>();
            values.Add(new ComplexO(_p0));
            values.Add(new ComplexO(_p1));
            return values;
        }

        public override void Draw(PlotterGraphics g)
        {
            g.DrawLineV(_p0, _p1);
        }

        public override bool CanSelect(PointD p, double xtol, double ytol)
        {
            return IsNear(p, xtol, ytol);
        }

        private bool IsNear(PointD p, double xtol, double ytol)
        {
            Complex pc = new Complex(p.X, p.Y);
            Complex c0 = new Complex(_p0.X, _p0.Y);
            Complex c1 = new Complex(_p1.X, _p1.Y);
            Complex xt = new Complex(xtol, 0);
            Complex yt = new Complex(0, ytol);

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
    }


    // a set of disconnected lines
    class Lines : Shape
    {
        private List<Line> _lines;

        public Lines()
        {
            _lines = new List<Line>();
        }

        public Lines(PointD p1, PointD p2)
        {
            _lines = new List<Line>();
            Add(p1, p2);
        }

        public void Add(PointD p1, PointD p2)
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

        public override bool CanSelect(PointD p, double xtol, double ytol)
        {
            foreach (Line l in _lines)
                if (l.CanSelect(p, xtol, ytol)) return true;
            return false;
        }
    }

    class Grid : Lines
    {
        public Grid(
            double xmin, double xmax, int xsteps,
            double ymin, double ymax, int ysteps)
        {
            double xd = (xmax - xmin) / (double)xsteps;
            double x = xmin;
            for (int i = 0; i <= xsteps; i++)
            {
                Add(new PointD(x, ymin), new PointD(x, ymax));
                x += xd;
            }

            double yd = (ymax - ymin) / (double)ysteps;
            double y = ymin;
            for (int i = 0; i <= ysteps; i++)
            {
                Add(new PointD(xmin, y), new PointD(xmax, y));
                y += yd;
            }
        }
    }

}
