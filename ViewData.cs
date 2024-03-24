using lab_code_3_3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static lab_code_3_3.V1DataList;

namespace lab_1_wpf
{
    public class FuncCollection
    {
        public static List<FValues> FuncList { get; set; } = new List<FValues>() { F1, F2 };
        public static void F1(double x, ref double y1, ref double y2)
        {
            y1 = x * x * x;
            y2 = x;
        }
        public static void F2(double x, ref double y1, ref double y2)
        {
            y1 = x * x;
            y2 = x;
        }
        public static double startF(double x) => x + 1; //Начальное приближение
    }
    public class ViewData : IDataErrorInfo
    {
        public int MaxIterations { get; set; }
        public double DiscrepancyRate { get; set; }
        public int UniformNum { get; set; }
        public int SmoothingSplineNum { get; set; }
        public int FuncId { get; set; }
        public bool IsGridUniform { get; set; }
        public int NodesNum { get; set; }
        public double[] GridBoundaries { get; set; }
        public V1DataArray? DataArrayObj;
        public SplineData? SplineDataObj;
        public Plotter? plotModel { get; set; }

        public ViewData()
        {
            GridBoundaries = new double[2] { 1, 8 };
            NodesNum = 50;
            IsGridUniform = true;
            FuncId = 0;
            SmoothingSplineNum = 50;
            UniformNum = 100;
            DiscrepancyRate = 1e-7;
            MaxIterations = 100;
            DataArrayObj = null;
            SplineDataObj = null;
        }
        public void Calculation()
        {
            Func<double, double> fInit = FuncCollection.startF;
            SplineData.BuildSpline(SplineDataObj, fInit, IsGridUniform);
            plotModel = new Plotter(SplineDataObj);
        }

        public void InitializeData()
        {
            if (SmoothingSplineNum <= 1)
            {
                throw new Exception("Invalid number of nodes for smoothing spline");
            }
            if (UniformNum <= 1)
            {
                throw new Exception("Invalid number of nodes for uniform grid");
            }
            SplineDataObj = new SplineData(DataArrayObj, SmoothingSplineNum, MaxIterations, UniformNum);
        }

        public void UpData()
        {
            GridBoundaries[0] = DataArrayObj.X[0];
            GridBoundaries[1] = DataArrayObj.X[DataArrayObj.X.Length-1];
            NodesNum = DataArrayObj.X.Length;
        }
        public void InitializeThroughControl()
        {
            FValues f = FuncCollection.FuncList[FuncId];

            if (GridBoundaries[1] == 0 || GridBoundaries[1] <= GridBoundaries[0])
            {
                throw new Exception("Invalid segment boundaries");
            }

            if (NodesNum <= 1)
            {
                throw new Exception("Invalid number of nodes");
            }
            DataArrayObj = new V1DataArray("Test", DateTime.Now, NodesNum, GridBoundaries[0], GridBoundaries[1], f, IsGridUniform);
        }

        public bool Save(string filename)
        {
            return V1DataArray.Save(filename,ref DataArrayObj);
        }

        public bool Load(string filename)
        {
            DataArrayObj = new V1DataArray("", new DateTime());
            return V1DataArray.Load(filename, ref DataArrayObj);
        }

        public void Print(string text)
        {
            Console.WriteLine(text);
        }
        public string Error { get { return "Incorrect data."; } }
        public string this[string propertyName]
        {
            get
            {
                string? error = null;
                switch (propertyName)
                {
                    case "NodesNum":
                        if (NodesNum < 3)
                            error = "Число узлов сетки, на которой заданы дискретные значения функции, должно быть больше или равно 3";
                        break;
                    case "UniformNum":
                        if (UniformNum < 3)
                            error = "Число узлов равномерной сетки, на которой вычисляются значения сглаживающего сплайна, должно быть больше или равно 3";
                        break;
                    case "LeftBorder":
                        if (GridBoundaries[0] >= GridBoundaries[1])
                            error = "Левый конец отрезка должен быть меньше правого конца отрезка";
                        break;
                    case "RightBorder":
                        if (GridBoundaries[0] >= GridBoundaries[1])
                            error = "Левый конец отрезка должен быть меньше правого конца отрезка";
                        break;
                    case "SmoothingSplineNum":
                        if (SmoothingSplineNum < 2 || SmoothingSplineNum > NodesNum)
                            error = "Число узлов сглаживающего сплайна должно быть больше или равно 2 и меньше или равно числу заданных дискретных значений функции";
                        break;
                    default:
                        break;
                }
                return error;
            }
        }
    }
}
