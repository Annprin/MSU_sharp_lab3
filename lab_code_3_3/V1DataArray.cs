using System.Text.Json;

namespace lab_code_3_3
{
    public class V1DataArray : V1Data
    {
        public double[] X { get; set; }
        public double[][] Y { get; set; }
        public static bool Save(string filename, ref V1DataArray array)
        {
            try
            {
                using (StreamWriter fs = new StreamWriter(filename))
                {
                    string s = JsonSerializer.Serialize(array.Key);
                    fs.WriteLine(s);
                    s = JsonSerializer.Serialize(array.Date);
                    fs.WriteLine(s);
                    s = JsonSerializer.Serialize(array.X);
                    fs.WriteLine(s);
                    s = JsonSerializer.Serialize(array.Y[0]);
                    fs.WriteLine(s);
                    s = JsonSerializer.Serialize(array.Y[1]);
                    fs.WriteLine(s);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                Console.WriteLine("Saving done");
            }
            return true;
        }
        public static bool Load(string filename, ref V1DataArray array)
        {
            try
            {
                using (StreamReader fs = new StreamReader(filename))
                {
                    string s = fs.ReadLine();
                    array.Key = JsonSerializer.Deserialize<string>(s);
                    s = fs.ReadLine();
                    array.Date = JsonSerializer.Deserialize<DateTime>(s);
                    s = fs.ReadLine();
                    array.X = JsonSerializer.Deserialize<double[]>(s);
                    s = fs.ReadLine();
                    var y1 = JsonSerializer.Deserialize<double[]>(s);
                    s = fs.ReadLine();
                    var y2 = JsonSerializer.Deserialize<double[]>(s);
                    array.Y = new double[2][] { y1, y2 };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                Console.WriteLine("Reading done");
            }
            return true;
        }
        public override IEnumerator<DataItem> GetEnumerator()
        {
            for (int i = 0; i < X.Length; i++)
            {
                double x = X[i];
                double y1 = Y[0][i];
                double y2 = Y[1][i];
                yield return new DataItem(x, y1, y2);
            }
        }
        public V1DataArray(string key, DateTime date) : base(key, date)
        {
            X = new double[0];
            Y = new double[2][];
            Y[0] = new double[0];
            Y[1] = new double[0];
        }
        public V1DataArray(string key, DateTime date, double[] x, FValues F) : base(key, date)
        {
            X = x;
            Y = new double[2][];
            Y[0] = new double[x.Length];
            Y[1] = new double[x.Length];
            for (int i = 0; i < x.Length; ++i)
            {
                F(x[i], ref Y[0][i], ref Y[1][i]);
            }
        }
        public V1DataArray(string key, DateTime date, int nX, double xL, double xR, FValues F, bool isUniform = true) : base(key, date)
        {
            X = new double[nX];
            Y = new double[2][];
            Y[0] = new double[nX];
            Y[1] = new double[nX];
            if (isUniform)
            {
                double step = (xR - xL) / (nX - 1);

                for (int i = 0; i < nX; ++i)
                {
                    X[i] = xL + step * i;
                    F(X[i], ref Y[0][i], ref Y[1][i]);
                }
            }
            else
            {
                Random rand = new Random();
                double previousPoint = xL;
                for (int i = 0; i < nX; ++i)
                {
                    double maxStep = (xR - previousPoint) / (nX - i);
                    double step = rand.NextDouble() * maxStep;
                    previousPoint += step;
                    X[i] = previousPoint;
                    F(X[i], ref Y[0][i], ref Y[1][i]);
                }
            }
        }

        public double[] this[int index]
        {
            get
            {
                if (index == 0 || index == 1)
                {
                    return Y[index];
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Index must be 0 or 1.");
                }
            }
        }

        public V1DataList ToList
        {
            get
            {
                V1DataList dataList = new V1DataList(Key, Date);
                for (int i = 0; i < X.Length; i++)
                {
                    dataList.DataItems.Add(new DataItem(X[i], Y[0][i], Y[1][i]));
                }
                return dataList;
            }
        }

        public override DataItem xMaxItem
        {
            get
            {
                bool f = false;
                DataItem xMax = new DataItem(0, 0, 0);
                for (int i = 0; i < X.Length; i++)
                {
                    if (!f)
                    {
                        xMax.X = X[i];
                        xMax.Y1 = Y[0][i];
                        xMax.Y2 = Y[1][i];
                        f = true;
                    }
                    else
                    {
                        if (X[i] > xMax.X)
                        {
                            xMax.X = X[i];
                            xMax.Y1 = Y[0][i];
                            xMax.Y2 = Y[1][i];
                        }
                    }
                }
                return xMax;
            }
        }
        public override double MaxDistance
        {
            get
            {
                double maxDistance = 0.0;
                for (int i = 0; i < X.Length - 1; i++)
                {
                    for (int j = i + 1; j < X.Length; j++)
                    {
                        double distance = Math.Abs(X[i] - X[j]);
                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                        }
                    }
                }
                return maxDistance;
            }
        }

        public override string ToString()
        {
            return $"Type: {GetType()}, {base.ToString()}, DataItems Count: {X.Length}\n";
        }

        public override string ToLongString(string format)
        {
            string result = ToString() + Environment.NewLine;
            for (int i = 0; i < X.Length; i++)
            {
                result += $"X: {X[i].ToString(format)}, Y1: {Y[0][i].ToString(format)}, Y2: {Y[1][i].ToString(format)}{Environment.NewLine}";
            }
            return result;
        }
    }
}