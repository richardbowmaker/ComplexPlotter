using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace ComplexPlotter
{
    class Axes
    {
        public Axis XAxis;
        public Axis YAxis;

        private const int _tickSize = 3;
        private const int _fontSize = 7;

        public void Draw(PlotterGraphics g)
        {
            SolidBrush b = new SolidBrush(Color.Black);
            FontFamily fontFamily = new FontFamily("Lucida Console");
            Font f = new Font(fontFamily, _fontSize, FontStyle.Regular, GraphicsUnit.Point);
            StringFormat sf = new StringFormat(StringFormatFlags.DirectionVertical);
            SizeF st = g.Graphics.MeasureString("0", f);

            // draw x axis
            int oy = g.VR.YVToC(YAxis.Origin);
            g.DrawLineC(g.VR.CXMin, oy, g.VR.CXMax, oy);

            // draw x axis ticks and values
            for (int i = 0; i < XAxis.NoOfTicks; i++)
            {
                double tv = XAxis.Tick(i);
                int tc = g.VR.XVToC(tv);

                if (tv != XAxis.Origin)
                {
                    // tick mark
                    g.DrawLineC(tc, oy - _tickSize, tc, oy + _tickSize);

                    // value
                    string stv = tv.ToString();
                    if (stv.Length < 4)
                        // horizontal text
                        g.Graphics.DrawString(stv, f, b, new PointF(tc - (st.Width), oy + _tickSize));
                    else
                        // vertical 
                        g.Graphics.DrawString(stv, f, b, new PointF(tc - _fontSize, oy + _tickSize), sf);
                }
            }

            // draw y axis
            int ox = g.VR.XVToC(XAxis.Origin);
            g.DrawLineC(ox, g.VR.CYMin, ox, g.VR.CYMax);

            // draw y axis ticks
            for (int i = 0; i < YAxis.NoOfTicks; i++)
            {
                double tv = YAxis.Tick(i);
                int tc = g.VR.YVToC(tv);

                if (tv != YAxis.Origin)
                {
                    // tick mark
                    g.DrawLineC(ox - _tickSize, tc, ox + _tickSize, tc);

                    // value
                    g.Graphics.DrawString(tv.ToString(), f, b, new PointF(ox + _tickSize, tc - (st.Height / 4)));
                }
            }

            // origin circle
            if (XAxis.Origin == 0.0 && YAxis.Origin == 0.0)
            {
                g.DrawEllipseC(ox - _tickSize, oy - _tickSize, _tickSize * 2, _tickSize * 2);
            }
        }

    }

    class Axis
    {
        public double Interval { get; private set; }
        public double Min { get; private set; }
        public double Max { get; private set; }
        public int NoOfTicks { get; private set; }
        public double Origin { get; private set; }

        public Axis()
        {
        }

        public Axis(double min, double max, double origin, int noOfTicks)
        {
            Initialise(min, max, origin, noOfTicks);
        }

        // calculates the axis based on the requirements, the axis ticks will fall on
        // 1 x 10 ** n, 2 x 10 ** n or 5 x 10 ** n, according to which gives the best
        // fit based on the desired no. of ticks.
        // The properties Interval, Start, End, NoOfTicks and Origin give the adjusted
        // values.
        // If the origin is no longer in the min and max range it will be adjusted to
        // the second tick mark.
        public void Initialise(double min, double max, double origin, int noOfTicks)
        {
            MantissaExp10 min10 = Utilities.GetMantissaExponent10(min);
            MantissaExp10 max10 = Utilities.GetMantissaExponent10(max);

            // normalise the min and max so that they have the same exponent
            // i.e. the smallest exponent of the two
            long me = Math.Min(min10.Exponent, max10.Exponent);
            long de = min10.Exponent - me;
            if (de > 0)
            {
                min10.Mantissa *= Math.Pow(10, de);
                min10.Exponent -= de;
            }

            de = max10.Exponent - me;
            if (de > 0)
            {
                max10.Mantissa *= Math.Pow(10, de);
                max10.Exponent -= de;
            }

            // calculate the ideal interval of each tick
            double iiv = (max - min) / (double)noOfTicks;
            MantissaExp10 meiiv = Utilities.GetMantissaExponent10(iiv);

            // ticks will be adjusted to 1 x 10 ** n, 2 x 10 ** n, or 5 x 10 ** n,
            // whichever gives the closest match to the ideal tick interval
            int mriv = 1;
            if (meiiv.Mantissa > 1.0 && meiiv.Mantissa <= 2.0) mriv = 2;
            else if (meiiv.Mantissa > 2.0) mriv = 5;

            // calculate the actual interval
            double iv = mriv * Math.Pow(10, meiiv.Exponent);

            // find the start and end points of the axis
            // as integer multiples of the tick interval
            long st = (long)Math.Floor(min / iv);
            long et = (long)Math.Ceiling(max / iv);

            // set actual values
            Min = st * iv;
            Max = et * iv;
            Interval = iv;
            NoOfTicks = 1 + (int)(et - st);

            if (origin > Min && origin < Max)
                // existing origin is still in view
                Origin = origin;
            else
                Origin = Tick(1);
        }

        public double Tick(int n)
        {
            return Min + n * Interval;
        }

        public override string ToString()
        {
            return String.Format("{0}...{1} step {2} #{3}, Origin {4}", Min, Max, Interval, NoOfTicks, Origin);
        }

    }

}
