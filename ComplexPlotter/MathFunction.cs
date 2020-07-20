using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace ComplexPlotter
{
    class ComplexMap
    {
        ComplexMap()
        {
            Discontinuity = false;
        }
        ComplexMap(Complex dval, Complex cval)
        {
            Discontinuity = false;
            DVal = dval;
            CVal = cval;
        }
        ComplexMap(bool discontinuity)
        {
            Discontinuity = discontinuity;
        }

        bool Discontinuity; // true implies a break in a series of ComplexPair values
        Complex DVal;       // Domain value
        Complex CVal;       // Codmain value
    }

 


    abstract class MathFunction : IDrawable
    {
        abstract public double f(double x);

        // rough calculate min and max values
        public RangeD Range(RangeD r)
        {
            double min = Double.MaxValue;
            double max = Double.MinValue;
            double x = r.Min;
            double xd = r.Size / 500.0;

            while (x < r.Max)
            {
                double v = f(x);
                min = Math.Min(min, v);
                max = Math.Max(max, v);
                x += xd;
            }
            return new RangeD(min, max);
        }

        public void Draw(PlotterGraphics pg)
        {
            double x = pg.VR.VXMin;
            double xd = pg.VR.VXSize / (double)pg.VR.CXSize;
            PointD p0 = null;

            while (x < pg.VR.VXMax)
            {
                PointD p1 = new PointD(x, f(x));
                if (p0 != null) pg.DrawLineV(p0, p1);
                p0 = p1;
                x += xd;
            }
        }
    }

    class SinX : MathFunction
    {
        public override double f(double x) { return Math.Sin(x); }
    }

    class Sinh : MathFunction
    {
        public override double f(double x) { return Math.Sinh(x); }
    }

    class X2 : MathFunction
    {
        public override double f(double x) { return x * x; }
    }

    class InverseX : MathFunction
    {
        public override double f(double x) { return x == 0.0 ? Double.MaxValue : 1.0 / x; }
    }

    class AddF : MathFunction
    {
        private MathFunction _f;
        private MathFunction _g;

        public AddF(MathFunction f, MathFunction g)
        {
            _f = f;
            _g = g;
        }

        public override double f(double x) { return _f.f(x) + _g.f(x); }
    }

    class MultF : MathFunction
    {
        private MathFunction _f;
        private MathFunction _g;

        public MultF(MathFunction f, MathFunction g)
        {
            _f = f;
            _g = g;
        }

        public override double f(double x) { return _f.f(x) * _g.f(x); }
    }

    class CompF : MathFunction
    {
        private MathFunction _f;
        private MathFunction _g;

        public CompF(MathFunction f, MathFunction g)
        {
            _f = f;
            _g = g;
        }

        public override double f(double x) { return _f.f(_g.f(x)); }
    }

    class PowerF : MathFunction
    {
        private double _p;

        public PowerF(double p)
        {
            _p = p;
        }

        public override double f(double x) { return Math.Pow(x, _p); }
    }

    class ConstantF : MathFunction
    {
        private double _k;

        public ConstantF(double k)
        {
            _k = k;
        }

        public override double f(double x) { return _k; }
    }

    class PolyF : MathFunction
    {
        MathFunction _f;

        public PolyF(List<double> cooeffs)
        {
            double p = 0.0;
            MathFunction f = new ConstantF(0.0);
            foreach (double c in cooeffs)
            {
                if (p == 0.0)
                {
                    f = new ConstantF(c);
                }
                else
                {
                    f = new AddF(f, new MultF(new PowerF(p), new ConstantF(c)));
                }
                p += 1.0;
            }
            _f = f;
        }

        public override double f(double x) { return _f.f(x); }
    }



}
