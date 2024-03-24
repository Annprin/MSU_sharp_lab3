using lab_code_3_3;


class Program
{
    public static void SquaredArray(double x, ref double y1, ref double y2) {

        y1 = x * x;
        y2 = x * x;

    }
    static double initial_approximation_func3(double x)
    {
        return x * x - 8 * x - 11;
    }
    static double initial_approximation_func2(double x)
    {
        return x * x * x - 8 * x - 11;
    }
    public static double initial_approximation_func1(double x) {
        return x * x * x + 2 * x * x + 3 * x + 4;
    }

    static void Main()
    {
        // Создание тестовых данных
       /* double[] xValues = { 1.0, 2.0, 3.0, 4.0, 5.0 };
        Array.Sort(xValues);
        xValues = xValues.Distinct().ToArray();*/

        // Создание объекта VDataArray
       /* V1DataArray vDataArray = new V1DataArray("key1", DateTime.Today, xValues, SquaredArray);*/

        // Создание объекта SplineData с небольшим числом узлов
        /*int m = 5;
        int maxIterations = 1000;

        Console.WriteLine($"using approximation function with number 1");
        SplineData splineData1 = new SplineData(vDataArray, m, maxIterations);
        splineData1.BuildSpline(splineData1, initial_approximation_func1);
        string filename1 = "C:\\Users\\KORCH\\source\\repos\\sharp_3\\lab_code_3_3\\spline_results1.txt";
        string format = "F2";
        splineData1.Save(filename1, format);
        Console.WriteLine(splineData1.ToLongString(format));

        Console.WriteLine($"using approximation function with number 2");
        SplineData splineData2 = new SplineData(vDataArray, m, maxIterations);
        splineData2.BuildSpline(splineData2, initial_approximation_func2);
        string filename2 = "C:\\Users\\KORCH\\source\\repos\\sharp_3\\lab_code_3_3\\spline_results2.txt";
        splineData2.Save(filename2, format);
        Console.WriteLine(splineData2.ToLongString(format));

        Console.WriteLine($"using approximation function with number 3");
        SplineData splineData3 = new SplineData(vDataArray, m, maxIterations);
        splineData3.BuildSpline(splineData3, initial_approximation_func3);
        string filename3 = "C:\\Users\\KORCH\\source\\repos\\sharp_3\\lab_code_3_3\\spline_results3.txt";
        splineData3.Save(filename3, format);
        Console.WriteLine(splineData3.ToLongString(format));*/
    }
}