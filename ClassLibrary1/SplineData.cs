using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary2
{
    public class SplineData
    {
        public RawData raw_data { get; set; }
        public int unodes_num { get; set; }
        public double leftSecondDerivative { get; set; }
        public double rightSecondDerivative { get; set; }
        public List<SplineDataItem> splineDataItems { get; set; }
        public double integralValue { get; set; }
        public SplineData(RawData raw_data, double leftSecondDerivative, double rightSecondDerivative, int n)
        {
            this.raw_data = raw_data;
            this.leftSecondDerivative = leftSecondDerivative;
            this.rightSecondDerivative = rightSecondDerivative;
            this.unodes_num = n;
            splineDataItems = new List<SplineDataItem>();
        }
        public void DoSplines()
        {
            double[] grid = { raw_data.begin, raw_data.end };
            double[] derivative = { leftSecondDerivative, rightSecondDerivative };
            double[] left_int_bound = { raw_data.begin };
            double[] right_int_bound = { raw_data.end };
            double[] calculated_values = new double[1 * 3 * unodes_num];
            double[] calculated_integrals = new double[1];
            int status = 0;
            double step = (raw_data.end - raw_data.begin) / (unodes_num - 1);
            CalculateMKL(raw_data.node_number, grid, raw_data.nodes, raw_data.values, raw_data.grid_type, unodes_num, derivative,
                left_int_bound, right_int_bound, calculated_integrals, calculated_values, ref status);
            if (status != 0)
            {
                throw new Exception($"Статус ошибки, произошедшей при вычислении сплайна в МКЛ, = {status}\n");
            }
            for (int i = 0; i < calculated_values.Length / 3; i++)
            {
                splineDataItems.Add(new SplineDataItem(raw_data.begin + i * step, calculated_values[i * 3], calculated_values[i * 3 + 1], calculated_values[i * 3 + 2]));
            }
            integralValue = calculated_integrals[0];

            [DllImport("C:\\prak\\с#_1\\Solution1\\x64\\Debug\\Dll1.dll", CallingConvention = CallingConvention.Cdecl)]
            static extern void CalculateMKL(int nodes_count, double[] grid_uniform, double[] grid_nonuniform, double[] funcion_values, bool grid_type, int unodes_num, double[] derivatives_bounds, double[] left_bound, double[] right_bound, double[] calculated_integrals, double[] calculated_values, ref int status);
        }
    }
}
