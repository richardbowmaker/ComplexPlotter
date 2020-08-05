using ComplexPlotter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ComlexPlotter
{
    class PlotComplex : IPlotter
    {
        private Plotter2d _domPlot;
        private Plotter2d _codomPlot;
        private TextBox _coords;
        private ComplexMathFunction _fn;

        public PlotComplex(PictureBox dom, PictureBox codom, TextBox coords)
        {
            _domPlot = new Plotter2d(dom, 10.0, 10.0, this);
            _codomPlot = new Plotter2d(codom, 600.0, 600.0, this);
            _coords = coords;

            ViewReckoner vr = _domPlot.View;
            Point oc = vr.VToC(_domPlot.Origin);
            //Shape mapper = new Line(oc, new Point(oc.X + vr.CXSize / 4, oc.Y - vr.CYSize / 4));
            //Shape mapper = new Grid(oc.X + 10, oc.X + vr.CXSize / 4, 4, oc.Y + 10, oc.Y - vr.CYSize / 4, 4);
            //Shape mapper = new Circle(oc, vr.CXSize / 10);
            //_plot1.SetMapper(mapper);

            //_fn = new ComplexZ2();
            _fn = new ComplexZ();
            //_fn = new ComplexSinInverseZ();
            //_fn = new ComplexSinZ2();
        }

        public void SetFunction(ComplexMathFunction fn)
        {
            _fn = fn;
        }

        public void Animate(List<PointDO> domain, List<PointDO> codomain)
        {
            _domPlot.Replot();
            _codomPlot.Replot();
            _domPlot.PlotPoints(domain);
            _codomPlot.PlotPoints(codomain);

            //List<PointDO> fz = ToCodomain(points);

            //for (int i = 2; i < fz.Count; ++i)
            //{
            //    List<PointDO> dom = new List<PointDO>();
            //    List<PointDO> codom = new List<PointDO>();
            //    for (int j = 0; j < i; ++j)
            //    {
            //        dom.Add(points[j]);
            //        codom.Add(fz[j]);
            //    }
            //    _domPlot.PlotPoints(dom);
            //    _codomPlot.PlotPoints(codom);
            //}
            //Thread.Yield();
            //Thread.Sleep(20);
        }

        private List<PointDO> ToCodomain(List<PointDO> points)
        {
            List<PointDO> fz = new List<PointDO>();
            foreach (PointDO p in points)
            {
                if (p.Discontinuity)
                    fz.Add(p);
                else
                    fz.Add(new PointDO(_fn.F(p.PointD), p.Color));
            }
            return fz;
        }

        public void PlotPoints(List<PointDO> points)
        {
            _codomPlot.PlotPoints(ToCodomain(points));
        }

        public void AtPoint(PointD point)
        {
            _coords.Text = point.X.ToString() + " " + point.Y.ToString() + "i";
        }
    }
}
