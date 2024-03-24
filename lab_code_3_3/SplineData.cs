using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace lab_code_3_3
{
    public class SplineData
    {
        [DllImport("C:\\Users\\KORCH\\source\\repos\\sharp_3\\Build\\x64\\Debug\\DLL1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DllSpline(int nX,
            double[] X,
            int nY,
            double[] Y,
            int m,
            double[] values_on_grid,
            double[] splineValues,
            ref int stop_reason,
            int maxIterations,
            ref int ActualNumberOfIterations,
            double[] addGrid,
            double[] addSplineData,
            int addSize);

        public V1DataArray dataArray { get; set; }
        public int m { get; set; }
        public double[] splineValues { get; set; }
        public int maxIterations { get; set; }
        public int stopReason { get; set; }
        public double minResidual { get; set; }
        public List<SplineDataItem> splineResults { get; set; }
        public int ActualNumberOfIterations { get; set; }
        public int UniformNum { get; set; }
        public List<double[]> ResultOnAddonGrid { get; set; }

        public SplineData(V1DataArray dataArray, int m, int maxIterations, int UniformNum)
        {
            this.dataArray = dataArray;
            this.m = m;
            this.maxIterations = maxIterations;
            this.UniformNum = UniformNum;
            splineValues = new double[this.dataArray.X.Length];
            splineResults = new List<SplineDataItem>();
            ResultOnAddonGrid = new List<double[]>();
        }

        public static double[] uniformGrid(double left_border, double right_border, int num_of_dots)
        {
            double step = (right_border - left_border) / (num_of_dots - 1);
            double[] ret = new double[num_of_dots];
            for (int i = 0; i < num_of_dots; i++)
            {
                ret[i] = left_border + step * i;
            }
            return ret;
        }

        public static double[] NonUniformGrid(double leftBorder, double rightBorder, int numOfDots)
        {
            Random random = new Random();
            double[] ret = new double[numOfDots];
            ret[0] = leftBorder;
            ret[numOfDots - 1] = rightBorder;

            for (int i = 1; i < numOfDots - 1; i++)
            {
                double range = (rightBorder - ret[i - 1]) / (numOfDots - i);
                double randomValue = random.NextDouble() * range;
                double nextValue = ret[i - 1] + randomValue;
                ret[i] = nextValue;
            }
            Array.Sort(ret);
            return ret;
        }

        public static void BuildSpline(SplineData data, Func<double, double> initial_approximation_func, bool isUniform = true)
        {
            int StopReasonLocal = 0;

            double step = (data.dataArray[0][data.dataArray[0].Length - 1] - data.dataArray[0][0]) / (data.m - 1);
            double[] values_on_grid;
            if (isUniform == true)
            {
                values_on_grid = uniformGrid(data.dataArray[0][0], data.dataArray[0][data.dataArray[0].Length - 1], data.m);
            }
            else
            {
                values_on_grid = NonUniformGrid(data.dataArray[0][0], data.dataArray[0][data.dataArray[0].Length - 1], data.m);
            }
            for (int i = 0; i < values_on_grid.Length; ++i)
            {
                values_on_grid[i] = initial_approximation_func(values_on_grid[i]);
            }
            int iterations = 0;
            double[] adduniformGrid;
            adduniformGrid = uniformGrid(data.dataArray.X[0], data.dataArray.X[data.dataArray.X.Length - 1], data.UniformNum);
            double[] valuesOnAdduniformGrid = new double[data.UniformNum];
            DllSpline(data.dataArray.X.Length,
            data.dataArray.X,
            data.dataArray[0].Length,
                data.dataArray[0],
                data.m,
                values_on_grid,
                data.splineValues,
                ref StopReasonLocal,
                data.maxIterations,
                ref iterations,
                adduniformGrid,
                valuesOnAdduniformGrid,
                data.UniformNum);
            data.minResidual = 0;
            for (int i = 0; i < data.splineValues.Length; ++i)
            {
                data.minResidual += (data.splineValues[i] - data.dataArray[0][i]) * (data.splineValues[i] - data.dataArray[0][i]);
                data.splineResults = data.splineResults.Append(new SplineDataItem(data.dataArray.X[i], data.dataArray[0][i], data.splineValues[i])).ToList();
            }
            data.minResidual = Math.Sqrt(data.minResidual);
            data.stopReason = StopReasonLocal;
            data.ActualNumberOfIterations = iterations;
            
            for (int i = 0; i < valuesOnAdduniformGrid.Length; ++i)
            {
                data.ResultOnAddonGrid = data.ResultOnAddonGrid.Append(new double[]{ adduniformGrid[i], valuesOnAdduniformGrid[i]}).ToList();
            }
        }

        public string ToLongString(string format)
        {
            var output = new StringBuilder();
            var reason = new string[7];
            reason[1] = "specified number of iterations has been exceeded";
            reason[2] = "specified trust region size has been reached";
            reason[3] = "specified residual norm has been reached";
            reason[4] = "the specified row norm of the Jacobian matrix has been reached";
            reason[5] = "specified trial step size has been reached";
            reason[6] = "the specified difference between the norm of the function and the error has been reached";
            output.AppendLine($"VDataArray: {dataArray.ToLongString(format)}");
            output.AppendLine();
            output.AppendLine("Spline approximation results:");
            foreach (var item in splineResults) output.AppendLine(item.ToString(format));
            output.AppendLine();
            output.AppendLine($"Minimal residual value: {minResidual.ToString()}");
            output.AppendLine($"Stop reason: {reason[stopReason]}");
            output.AppendLine($"Actual number of iterations: {ActualNumberOfIterations}");
            return output.ToString();
        }

        public bool Save(string filename, string format)
        {
            try
            {
                using (StreamWriter fwrite = new StreamWriter(filename))
                {
                    fwrite.WriteLine(ToLongString(format));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
    }
}
