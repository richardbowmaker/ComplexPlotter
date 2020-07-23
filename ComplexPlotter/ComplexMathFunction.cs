using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPlotter
{
    abstract class ComplexMathFunction
    {
        public virtual double PlotMax { get { return double.MaxValue; } }
        public virtual double PlotMin { get { return double.MinValue; } }
        public virtual List<PointD> Singularities {  get { return new List<PointD>(); } }

        public abstract PointD F(PointD p);
    }


    class ComplexZ2 : ComplexMathFunction
    {
        public override PointD F(PointD p)
        {
            return new PointD(Complex.Multiply(p.ToComplex(), p.ToComplex()));
        }
    }


    class ComplexZ : ComplexMathFunction
    {
        public override PointD F(PointD p)
        {
            return p;
        }
    }

    class ComplexSinZ : ComplexMathFunction
    {
        public override PointD F(PointD p)
        {
            return new PointD(Complex.Sin(p.ToComplex()));
        }
    }

    class ComplexSinInverseZ : ComplexMathFunction
    {
        public override PointD F(PointD p)
        {
            if (!p.IsZero)
            {
                Complex c = Complex.Divide(new Complex(1.0, 1.0), p.ToComplex());
                return new PointD(Complex.Sin(c));
            }
            else
                return new PointD();
        }
    }

    class ComplexSinZ2 : ComplexMathFunction
    {
        public override PointD F(PointD p)
        {
            if (!p.IsZero)
            {
                Complex c = Complex.Multiply(p.ToComplex(), p.ToComplex());
                return new PointD(Complex.Sin(c));
            }
            else
                return new PointD();
        }
    }
}
