﻿using System;
using System.Collections.Generic;
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
        public PointD() { X = 0; Y = 0; }
        public PointD(double x, double y) { X = x; Y = y; }
        public PointD(Complex z) { X = z.Real; Y = z.Imaginary; }

        public double X;
        public double Y;

        public override string ToString()
        {
            return String.Format("({0},{1})", X, Y);
        }

        public Complex ToComplex()
        {
            return new Complex(X, Y);
        }
    }

    class PointDO
    {
        private PointD _value;

        public PointDO()
        {
            _value = null;
        }

        public PointDO(PointD value)
        {
            _value = value;
        }

        public PointDO(double x, double y)
        {
            _value = new PointD(x, y);
        }

        public bool Discontinuity { get { return _value == null; } }

        public PointD PointD { get { return _value; } }

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


    }
}
