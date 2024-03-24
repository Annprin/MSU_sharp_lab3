using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace lab_code_3_3
{
    public class SplineDataItem
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double SplineY { get; set; }

        public SplineDataItem(double x, double y, double splineY)
        {
            X = x;
            Y = y;
            SplineY = splineY;
        }

        public string ToString(string format)
        {
            return $"X: {X.ToString(format)}, Y: {Y.ToString(format)}, SplineY: {SplineY.ToString(format)}";
        }

        public override string ToString()
        {
            return ToString("F2");
        }
    }
}
