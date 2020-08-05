using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPlotter
{
    abstract class ComplexMathFunction
    {
        public virtual double PlotMax { get { return double.MaxValue; } }
        public virtual double PlotMin { get { return double.MinValue; } }
        public virtual List<PointD> Singularities { get { return new List<PointD>(); } }

        public abstract PointD F(PointD p);

        public override string ToString()
        {
            return "";
        }

    }


    class ComplexZ2 : ComplexMathFunction
    {
        public override PointD F(PointD p)
        {
            return new PointD(Complex.Multiply(p.ToComplex(), p.ToComplex()));
        }

        public override string ToString()
        {
            return "z^2";
        }
    }


    class ComplexZ : ComplexMathFunction
    {
        public override PointD F(PointD p)
        {
            return p;
        }

        public override string ToString()
        {
            return "z";
        }
    }

    class ComplexSinZ : ComplexMathFunction
    {
        public override PointD F(PointD p)
        {
            return new PointD(Complex.Sin(p.ToComplex()));
        }

        public override string ToString()
        {
            return "sin(z)";
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

        public override string ToString()
        {
            return "sin(1/z)";
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

        public override string ToString()
        {
            return "sin(z^2)";
        }
    }

    class ComplexAdd : ComplexMathFunction
    {
        private ComplexMathFunction _f1;
        private ComplexMathFunction _f2;

        public ComplexAdd(ComplexMathFunction f1, ComplexMathFunction f2)
        {
            _f1 = f1;
            _f2 = f2;
        }

        public override PointD F(PointD p)
        {
            return _f1.F(p).Add(_f2.F(p));
        }

        public override string ToString()
        {
            return String.Format("({0}) + ({1})", _f1.ToString(), _f2.ToString());
        }
    }

    class ComplexPower : ComplexMathFunction
    {
        private double _p;

        public ComplexPower(double p)
        {
            _p = p;
        }

        public override PointD F(PointD p)
        {
            return p.Power(_p);
        }

        public override string ToString()
        {
            return String.Format("^{0}", _p);
        }
    }

    class ComplexMult : ComplexMathFunction
    {
        private double _p;

        public ComplexMult(double p)
        {
            _p = p;
        }

        public override PointD F(PointD p)
        {
            return p.Mult(new PointD(_p, 0.0));
        }

        public override string ToString()
        {
            return String.Format("X {0}", _p);
        }
    }

    class ComplexConstant : ComplexMathFunction
    {
        private PointD _c;

        public ComplexConstant(PointD c)
        {
            _c = c;
        }
        public ComplexConstant(double c)
        {
            _c = new PointD(c, 0.0);
        }

        public override PointD F(PointD p)
        {
            return _c;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}i", _c.X, _c.Y);
        }
    }

    class ComplexPolynomialTerm : ComplexMathFunction
    {
        private Complex _power;
        private Complex _coeff;

        public ComplexPolynomialTerm(double power, double coeff)
        {
            _power = new Complex(power, 0.0);
            _coeff = new Complex(coeff, 0.0);
        }

        public override PointD F(PointD p)
        {
            Complex c = p.ToComplex();
            c = Complex.Pow(c, _power);
            c = Complex.Multiply(c, _coeff);
            return new PointD(c);
        }

        public override string ToString()
        {
            return String.Format("{0}x^{1}", _coeff.Real, _power.Real);
        }
    }

    class ComplexPolynomial : ComplexMathFunction
    {
        private ComplexMathFunction _f;
        private string _s;

        public ComplexPolynomial(List<double> coeffs)
        {
            int p = coeffs.Count() - 1;
            _f = null;
            _s = "";

            foreach (double coeff in coeffs)
            {
                ComplexPolynomialTerm f1 = new ComplexPolynomialTerm(p, coeff);
                if (_f == null)
                    _f = f1;
                else
                    _f = new ComplexAdd(_f, f1);

                if (coeff != 0.0)
                {
                    if (_s.Length > 0) _s += " + ";

                    if (coeff == 1.0)
                    {
                        if (p > 0)
                            _s += String.Format("x^{0}", p);
                        else
                            _s += "1";
                    }
                    else
                    {
                        if (p > 0)
                            _s += String.Format("{0}x^{1}", coeff, p);
                        else
                            _s += String.Format("{0}x", coeff);
                    }
                }

                --p;
            }
        }

        public override PointD F(PointD p)
        {
            return _f.F(p);
        }
    }


}
