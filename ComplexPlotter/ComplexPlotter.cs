using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace ComplexPlotter
{
    public partial class ComplexPlotter : Form
    {
        public ComplexPlotter()
        {
            InitializeComponent();
        }

        private Plotter2d _domain;
        private Plotter2d _coDomain;

        private void ComplexPlotter_Load(object sender, EventArgs e)
        {
            _domain = new Plotter2d(picDomain, 20, 20);
            _coDomain = new Plotter2d(picCoDomain, 20, 20);


            Logger.TheListBox = lstLogger;
            Logger.Info("Complex plotter");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            _domain.Clear();
        }

        private bool IsNearLine(Complex c1, Complex c2, Complex p, double xtol, double ytol)
        {
            Complex xt = new Complex(xtol, 0);
            Complex yt = new Complex(0, ytol);

            // make c1 the left most point, smallest real value
            if (c1.Real > c2.Real)
            {
                Complex c3 = c1;
                c1 = c2;
                c2 = c3;
            }

            // move c1 to origin
            Complex c2p = Complex.Subtract(c2, c1);
            Complex pp = Complex.Subtract(p, c1);

            // rotate so that c2p lies on real axis
            Complex r = new Complex(c2p.Real / c2p.Magnitude, -c2p.Imaginary / c2p.Magnitude);
            c2p = Complex.Multiply(c2p, r);
            pp = Complex.Multiply(pp, r);
            xt = Complex.Multiply(xt, r);
            yt = Complex.Multiply(yt, r);

            // take worst case of tolerances in x and y directions
            double dxp = Math.Max(Math.Abs(xt.Real), Math.Abs(yt.Real));
            double dyp = Math.Max(Math.Abs(xt.Imaginary), Math.Abs(yt.Imaginary));

            // is within tolerance of line c1 to c2
            return pp.Real > -dxp && pp.Real < c2p.Real + dxp &&
                   pp.Imaginary > -dyp && pp.Imaginary < c2p.Imaginary + dyp;
        }

        private void butSinX_Click(object sender, EventArgs e)
        {
            //MathFunction f = new CompF(new SinX(), new InverseX());
            MathFunction f = new PolyF(new List<double> { 0.0, 5.0, 10.0, 3.0, 2.0, 1.0 });
            //f = new CompF(new InverseX(), f);

            _domain.f = f;
            RangeD r = f.Range(_domain.View.VXRange);
            double min = Math.Max(r.Min, -100.0);
            double max = Math.Min(r.Max, 100.0);
            min *= 1.1;
            max *= 1.1;
            _domain.SetYExtent(new RangeD(min, max));
        }

 
    }
}
