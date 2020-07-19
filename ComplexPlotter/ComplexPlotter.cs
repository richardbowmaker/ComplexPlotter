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
            //_coDomain = new Plotter2d(picCoDomain, 20, 20);


            Logger.TheListBox = lstLogger;
            Logger.Info("Complex plotter");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            _domain.Replot();
            _domain.DisplayImage();

            //Bitmap bm1 = new Bitmap(picCoDomain.Width, picCoDomain.Height);
            //Graphics g1 = Graphics.FromImage(bm1);
            //Pen p1 = new Pen(Color.Black, 1.0f);
            //g1.DrawLine(p1, 10, 10, 300, 300);

            //Bitmap bm2 = new Bitmap(picCoDomain.Width, picCoDomain.Height);
            //Graphics g2 = Graphics.FromImage(bm2);
            //Pen p2 = new Pen(Color.Red, 1.0f);
            //g2.DrawLine(p2, 100, 10, 20, 30);


            //Graphics gout = picCoDomain.CreateGraphics();
            //gout.DrawImage(bm1, new Point(0, 0));
            //gout.DrawImage(bm2, new Point(0, 0));





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
