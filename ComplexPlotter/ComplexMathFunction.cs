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
}
