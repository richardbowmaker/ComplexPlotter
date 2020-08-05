using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPlotter
{
    public struct MantissaExp10
    {
        public MantissaExp10(double m, long e)
        {
            Mantissa = m;
            Exponent = e;
        }
        public double Mantissa;
        public long Exponent;
    }

    class PointD
    {
        public PointD() { X = 0.0; Y = 0.0; }
        public PointD(double x, double y) { X = x; Y = y; }
        public PointD(Complex z) { X = z.Real; Y = z.Imaginary; }

        public double X;
        public double Y;

        public bool IsZero {  get { return X == 0.0 && Y == 0.0; } }
        public override string ToString()
        {
            return String.Format("({0},{1})", X, Y);
        }

        public Complex ToComplex()
        {
            return new Complex(X, Y);
        }

        public PointD Add(PointD p)
        {
            return new PointD(X + p.X, Y + p.Y);
        }
        public PointD Subtract(PointD p)
        {
            return new PointD(X - p.X, Y - p.Y);
        }

        public PointD Mult(PointD p)
        {
            Complex c = Complex.Multiply(ToComplex(), p.ToComplex());
            return new PointD(c);
        }

        public PointD Power(PointD p)
        {
            Complex c = Complex.Pow(ToComplex(), p.ToComplex());
            return new PointD(c);
        }

        public PointD Power(double p)
        {
            Complex c = Complex.Pow(ToComplex(), new Complex(p, 0));
            return new PointD(c);
        }
    }

    class PointDO
    {
        private PointD _value;
        private Color _color;

        public PointDO()
        {
            _value = null;
            _color = Color.Black;
        }

        public PointDO(PointD value)
        {
            _value = value;
            _color = Color.Black;
        }

        public PointDO(double x, double y)
        {
            _value = new PointD(x, y);
            _color = Color.Black;
        }

        public PointDO(PointD value, Color color)
        {
            _value = value;
            _color = color;
        }

        public PointDO(double x, double y, Color color)
        {
            _value = new PointD(x, y);
            _color = color;
        }

        public double X {  get { return _value.X; } }
        public double Y {  get { return _value.Y; } }

        public bool Discontinuity { get { return _value == null; } }

        public PointD PointD { get { return _value; } }

        public Color Color {  get { return _color; } }

        public override string ToString()
        {
            if (Discontinuity)
                return "(-,-)";
            else
                return String.Format("({0},{1})", _value.X, _value.Y);
        }
    }

    class RangeD
    {
        public RangeD() { Min = 0; Max = 0; }
        public RangeD(double min, double max) { Min = min; Max = max; }

        public double Min;
        public double Max;

        public double Size { get { return Max - Min; } }

        public override string ToString()
        {
            return String.Format("({0}:{1})", Max, Min);
        }
    }

    class Utilities
    {
 
        public static MantissaExp10 GetMantissaExponent10(double d)
        {
            double exp = Math.Floor(Math.Log10(Math.Abs(d)));
            double man = d / Math.Pow(10, exp);
            return new MantissaExp10(man, (long)exp);
        }

        public static List<Color> ColorList()
        {
            List<Color> cs = new List<Color>();
            cs.Add(Color.Aqua);
            cs.Add(Color.Beige);
            cs.Add(Color.Brown);
            cs.Add(Color.Crimson);
            cs.Add(Color.Red);
            cs.Add(Color.ForestGreen);
            cs.Add(Color.DarkSalmon);
            cs.Add(Color.MediumTurquoise);
            cs.Add(Color.Olive);
            cs.Add(Color.Magenta);
            cs.Add(Color.Tan);
            cs.Add(Color.YellowGreen);
            cs.Add(Color.Orchid);
            cs.Add(Color.MidnightBlue);
            cs.Add(Color.SkyBlue);
            return cs;
        }


    }
}
