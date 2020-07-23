using ComplexPlotter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ComlexPlotter
{
    class PlotComplex : IPlotter
    {
        private Plotter2d _plot1;
        private Plotter2d _plot2;
        private ComplexMathFunction _fn;

        public PlotComplex(PictureBox plot1, PictureBox plot2)
        {
            _plot1 = new Plotter2d(plot1, 10.0, 10.0, this);
            _plot2 = new Plotter2d(plot2, 100.0, 100.0, null);


            ViewReckoner vr = _plot1.View;
            Point oc = vr.VToC(_plot1.Origin);
            //Shape mapper = new Line(oc, new Point(oc.X + vr.CXSize / 4, oc.Y - vr.CYSize / 4));
            Shape mapper = new Grid(oc.X + 10, oc.X + vr.CXSize / 4, 4, oc.Y + 10, oc.Y - vr.CYSize / 4, 4);
            //Shape mapper = new Circle(oc, vr.CXSize / 10);
            _plot1.SetMapper(mapper);

            //_fn = new ComplexZ2();
            //_fn = new ComplexZ();
            _fn = new ComplexSinInverseZ();
            //_fn = new ComplexSinZ2();
        }

        public void SetFunction(ComplexMathFunction fn)
        {
            _fn = fn;
        }

        public void PlotPoints(List<PointDO> points)
        {
            List<PointDO> fz = new List<PointDO>();
            foreach (PointDO p in points)
            {
                if (p.Discontinuity)
                    fz.Add(p);
                else
                    fz.Add(new PointDO(_fn.F(p.PointD), p.Color));
            }
            _plot2.PlotPoints(fz);
        }
    }
}
