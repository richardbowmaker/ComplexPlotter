using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows;


namespace ComplexPlotter
{
 
    class ViewReckoner
    {
        CoordReckoner _x;
        CoordReckoner _y;

        public ViewReckoner(
            int cx1, int cx2, 
            int cy1, int cy2, 
            double vx1, double vx2,
            double vy1, double vy2)
        {
            _x = new CoordReckoner(cx1, cx2, vx1, vx2);
            _y = new CoordReckoner(cy1, cy2, vy1, vy2);
        }


        public double VXSize {  get { return _x.VSize; } }
        public double VYSize {  get { return _y.VSize; } }
        public int CXSize { get { return _x.CSize; } }
        public int CYSize { get { return Math.Abs(_y.CSize); } }

        public RangeD VXRange {  get { return _x.VRange;  } }

        public int XVToC(double x)
        {
            return _x.VToC(x);
        }

        public int YVToC(double y)
        {
            return _y.VToC(y);
        }

        public double XCToV(int x)
        {
            return _x.CToV(x);
        }
        public double YCToV(int y)
        {
            return _y.CToV(y);
        }

        public Point VToC(double x, double y)
        {
            return new Point(_x.VToC(x), _y.VToC(y));
        }

        public Point VToC(PointD p)
        {
            return new Point(_x.VToC(p.X), _y.VToC(p.Y));
        }

        public PointD CToV(int x, int y)
        {
            return new PointD(_x.CToV(x), _y.CToV(y));
        }
        public PointD CToV(Point p)
        {
            return new PointD(_x.CToV(p.X), _y.CToV(p.Y));
        }

        public int VXLenToC(double xl)
        {
            return _x.VLenToC(xl);
        }
        public int VYLenToC(double yl)
        {
            return _y.VLenToC(yl);
        }

        public double CXLenToV(int xl)
        {
            return _x.CLenToV(xl);
        }
        public double CYLenToV(int yl)
        {
            return _y.CLenToV(yl);
        }

        public double VXMin { get { return _x.V1; } }
        public double VXMax {  get { return _x.V2;  } }

        public double VYMin { get { return _y.V1; } }
        public double VYMax { get { return _y.V2; } }

        public int CXMin { get { return _x.C1; } }
        public int CXMax { get { return _x.C2; } }

        public int CYMin { get { return _y.C1; } }
        public int CYMax { get { return _y.C2; } }

        public void SetXExtent(RangeD ex)
        {
            _x.Zoom(ex.Min, ex.Max);
        }
        public void SetYExtent(RangeD ex)
        {
            _y.Zoom(ex.Min, ex.Max);
        }

        // Zooms into region specified by control co-ords
        public void Zoom(Point p1, Point p2)
        {
            
        }
    }

    class CoordReckoner
    {
        // the extent of the display in pixels
        int _c1;    
        int _c2;
        
        // the extent of the display area in view units 
        double _v1;     
        double _v2;

        double _scale;  // view units per pixel

        public CoordReckoner(int c1, int c2, double v1, double v2)
        {
            _c1 = c1;
            _c2 = c2;
            _v1 = v1;
            _v2 = v2;
            Initialise();
        }

        private void Initialise()
        {
            if (_c1 == _c2)
                _scale = 1.0f;
            else
                _scale = (_v1 - _v2) / (_c1 - _c2);
        }

        public int C1 { get { return _c1; } }
        public int C2 {  get { return _c2; } }
        public double V1 { get { return _v1; } }
        public double V2 { get { return _v2; } }
        public double Scale {  get { return _scale; } }
        public RangeD VRange {  get { return new RangeD(_v1, _v2); } }

        public int VToC(double v)
        {
            return _c1 + (int)((v - _v1) / _scale);
        }

        public double CToV(int c)
        {
            return _v1 + ((double)(c - _c1) * _scale);
        }

        public int VLenToC(double v)
        {
            return (int)(v / _scale);
        }

        public double CLenToV(int c)
        {
            return (double) c * _scale;
        }

        // Zoom to view port
        public void Zoom(double v1, double v2)
        {
            _v1 = v1;
            _v2 = v2;
            Initialise();
        }

        // Zoom at point value by ratio, ratio = 0.5 => zoom in
        public void ZoomAt(double v, double r)
        {
            _v1 = v * (1 - r) + r * _v1;
            _v2 = v * (1 - r) + r * _v2;
            Initialise();
        }

        public void MoveTo(int v)
        {
            _v1 += v;
            _v2 += v;
            Initialise();
        }

        public double VSize { get { return _v2 - _v1;  } }
        public int CSize { get { return _c2 - _c1;  } }

        public bool InView(double v)
        {
            return v >= _v1 && v <= _v2;
        }

        public static bool Test()
        {
            CoordReckoner cr = new CoordReckoner(100, 200, 400.0, 600.0);

            Logger.Assert(cr.C1 == 100, "1. C1 invalid");
            Logger.Assert(cr.C2 == 200, "2. C2 invalid");
            Logger.Assert(cr.V1 == 400.0, "3. V1 invalid");
            Logger.Assert(cr.V2 == 600.0, "4. V2 invalid");
            Logger.Assert(cr.Scale == 2.0, "5. Scale invalid");

            Logger.Assert(cr.VToC(400.0) == 100, "6. VToC(400.0) failed");
            Logger.Assert(cr.VToC(600.0) == 200, "7. VToC(600.0) failed");
            Logger.Assert(cr.VToC(500.0) == 150, "8. VToC(500.0) failed");
            Logger.Assert(cr.CToV(100) == 400.0, "9. CToV(100) failed");
            Logger.Assert(cr.CToV(200) == 600.0, "10. CToV(200) failed");
            Logger.Assert(cr.CToV(150) == 500.0, "11. CToV(150) failed");

            Logger.Assert(cr.VLenToC(20.0) == 10, "12. VLenToC(20.0)");
            Logger.Assert(cr.CLenToV(40) == 80.0, "13. CLenToV(40)");

            Logger.Assert(cr.VSize == 200.0, "14. VSize failed");
            Logger.Assert(cr.CSize == 100, "15. CSize failed");

            cr.Zoom(450.0, 550.0);
            Logger.Assert(cr.VSize == 100.0, "16. VSize failed");
            Logger.Assert(cr.CSize == 100, "17. CSize failed");
            Logger.Assert(cr.C1 == 100, "18. C1 invalid");
            Logger.Assert(cr.C2 == 200, "19. C2 invalid");
            Logger.Assert(cr.V1 == 450.0, "20. V1 invalid");
            Logger.Assert(cr.V2 == 550.0, "21. V2 invalid");
            Logger.Assert(cr.Scale == 1.0, "22. Scale invalid");

            Logger.Assert(cr.VToC(450.0) == 100, "23. VToC(450.0) failed");
            Logger.Assert(cr.VToC(550.0) == 200, "24. VToC(550.0) failed");
            Logger.Assert(cr.VToC(500.0) == 150, "25. VToC(500.0) failed");
            Logger.Assert(cr.CToV(100) == 450.0, "26. CToV(100) failed");
            Logger.Assert(cr.CToV(200) == 550.0, "27. CToV(200) failed");
            Logger.Assert(cr.CToV(150) == 500.0, "28. CToV(150) failed");

            CoordReckoner cr1 = new CoordReckoner(100, 200, 20.0, 40.0);
            Logger.Assert(cr1.VSize == 20.0, "29. VSize failed");
            Logger.Assert(cr1.CSize == 100, "30. CSize failed");
            cr1.ZoomAt(25.0, 0.5);
            Logger.Assert(cr1.VSize == 10.0, "40. VSize failed");
            Logger.Assert(cr1.CSize == 100, "41. CSize failed");
            Logger.Assert(cr1.C1 == 100, "42. C1 invalid");
            Logger.Assert(cr1.C2 == 200, "43. C2 invalid");
            Logger.Assert(cr1.V1 == 22.5, "44. V1 invalid");
            Logger.Assert(cr1.V2 == 32.5, "45. V2 invalid");
            Logger.Assert(cr1.Scale == 0.1, "46. Scale invalid");

            // y axis, inversion 
            CoordReckoner cr2 = new CoordReckoner(200, 100, 20.0, 40.0);
            Logger.Assert(cr2.VToC(20.0) == 200, "47. VToC(450.0) failed");
            Logger.Assert(cr2.VToC(40.0) == 100, "48. VToC(550.0) failed");
            Logger.Assert(cr2.VToC(30.0) == 150, "49. VToC(500.0) failed");
            Logger.Assert(cr2.CToV(200) == 20.0, "50. CToV(100) failed");
            Logger.Assert(cr2.CToV(100) == 40.0, "51. CToV(200) failed");
            Logger.Assert(cr2.CToV(150) == 30.0, "52. CToV(150) failed");

            return false;
        }
    }
}
