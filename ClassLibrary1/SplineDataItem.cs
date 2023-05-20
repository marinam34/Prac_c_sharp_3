using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary2
{
    public class SplineDataItem
    {
        public double node { get; set; }
        public double spline_value { get; set; }
        public double FistDerivative { get; private set; }
        public double SecondDerivative { get; private set; }
        public SplineDataItem(double node, double spline_value, double fistDerivative, double secondDerivative)
        {
            this.node = node;
            this.spline_value = spline_value;
            FistDerivative = fistDerivative;
            SecondDerivative = secondDerivative;
        }
        public override string ToString()
        {
            return $"node = {node},\nspline_value = {spline_value},\nFistDerivative = {FistDerivative},\nSecondDerivative = {SecondDerivative}";
        }
        public string ToString(string format)
        {
            return $"node = {string.Format(format, node)},\nspline_value = {string.Format(format, spline_value)},\nFistDerivative = {string.Format(format, FistDerivative)},\nSecondDerivative = {string.Format(format, SecondDerivative)}";
        }
    }
}
