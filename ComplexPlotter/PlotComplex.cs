using ComplexPlotter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComlexPlotter
{
    class PlotComplex : IPlotter
    {
        private Plotter2d _plot1;
        private Plotter2d _plot2;
        private ComplexMathFunction _fn;

        public PlotComplex(PictureBox plot1, PictureBox plot2)
        {
            _plot1 = new Plotter2d(plot1, 20.0, 20.0, this);
            _plot2 = new Plotter2d(plot2, 20.0, 20.0, null);

            //_plot1.Replot();
            //_plot2.Replot();
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
                    fz.Add(new PointDO(_fn.F(p.PointD)));
            }
            _plot2.PlotPoints(fz);
        }
    }
}
