﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using ComlexPlotter;
using System.Resources;

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

        // complex plotter
        private PlotComplex _complex;

        // complex animation
        private int _animationIndex;
        private List<PointDO> _animationDom;
        private List<PointDO> _animationCodom;

        private void ComplexPlotter_Load(object sender, EventArgs e)
        {
            //_domain = new Plotter2d(picDomain, 20, 20, null);
            //_coDomain = new Plotter2d(picCoDomain, 20, 20, null);

            _complex = new PlotComplex(picDomain, picCoDomain, txtCoords);


    
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

        private void butCircle_Click(object sender, EventArgs e)
        {

            const int n = 100;
            const double r = 0.0666666666;
            _animationDom = new List<PointDO>();
            _animationCodom = new List<PointDO>();
            _animationIndex = 5;

            ComplexMathFunction fn = new ComplexPolynomial(new List<double>() { 1.0, 0.0, 0.0, 0.0, 15.0, 1.0 });

            PointD p1 = fn.F(new PointD(1.0, 0.0));
            p1 = fn.F(new PointD(2.0, 0.0));
 

            //           ComplexMathFunction fn = new ComplexZ();

            double xmin = Double.MaxValue;
            double xmax = Double.MinValue;
            double ymin = Double.MaxValue;
            double ymax = Double.MinValue;

            double arg = 0;
            for (int i = 0; i <= n; i++)
            {
                PointDO p = new PointDO(
                    ((double)r) * Math.Cos(arg),
                    ((double)r) * Math.Sin(arg));
                _animationDom.Add(p);
                PointDO c = new PointDO(fn.F(p.PointD));
                _animationCodom.Add(c);
                arg += (2.0 * Math.PI) / (double)n;

                xmin = Math.Min(xmin, c.X);
                xmax = Math.Max(xmax, c.X);
                ymin = Math.Min(ymin, c.Y);
                ymax = Math.Max(ymax, c.Y);
            }
            plotTimer.Enabled = true;
            plotTimer.Interval = 20;
        }

        private void plotTimer_Tick(object sender, EventArgs e)
        {
            if (++_animationIndex < _animationDom.Count)
            {
                List<PointDO> dom = new List<PointDO>();
                List<PointDO> codom = new List<PointDO>();

                for (int i = 0; i < _animationIndex; ++i)
                {
                    dom.Add(_animationDom[i]);
                    codom.Add(_animationCodom[i]);
                }
                _complex.Animate(dom, codom);
            }
        }

        private void butDomCodomSame_Click(object sender, EventArgs e)
        {
      
        }
    }
}
