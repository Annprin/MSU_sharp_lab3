namespace lab_code_3_3
{
    public delegate void FValues(double x, ref double y1, ref double y2);
    public delegate DataItem FDI(double x);
    public class V1DataList : V1Data
    {
        public List<DataItem> DataItems { get; }
        public override IEnumerator<DataItem> GetEnumerator()
        {
            return DataItems.GetEnumerator();
        }
        public V1DataList() : base("First", DateTime.Now)
        {
            DataItems = new List<DataItem>();
            for (int i = 0; i < 5; ++i)
            {
                DataItem res = new DataItem(i+1, i+1, i+1);
                DataItems.Add(res);
            }
        }
        public V1DataList(string key, DateTime date) : base(key, date)
        {
            DataItems = new List<DataItem>();
        }

        public V1DataList(string key, DateTime date, double[] x, FDI F) : base(key, date)
        {
            DataItems = new List<DataItem>();
            bool f = true;
            for (int i = 0; i < x.Length; ++i)
            {
                DataItem res = F(x[i]);
                f = true;
                foreach (var item in DataItems)
                {
                    if (item.X == x[i])
                    {
                        f = false;
                        break;
                    }
                }
                if (f)
                    DataItems.Add(res);
            }
        }
        public override DataItem xMaxItem
        {
            get
            {
                bool f = false;
                DataItem xMax = new DataItem(0, 0, 0);
                foreach (var dataItem in DataItems)
                {
                    if (!f)
                    {
                        xMax = dataItem;
                        f = true;
                    }
                    else
                    {
                        if (dataItem.X > xMax.X)
                            xMax = dataItem;
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
                for (int i = 0; i < DataItems.Count - 1; i++)
                {
                    for (int j = i + 1; j < DataItems.Count; j++)
                    {
                        double distance = Math.Abs(DataItems[i].X - DataItems[j].X);
                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                        }
                    }
                }
                return maxDistance;
            }
        }

        public static explicit operator V1DataArray(V1DataList source)
        {
            V1DataArray dataArray = new V1DataArray(source.Key, source.Date);
            dataArray.X = source.DataItems.Select(item => item.X).ToArray();
            dataArray.Y = new double[2][];
            dataArray.Y[0] = source.DataItems.Select(item => item.Y1).ToArray();
            dataArray.Y[1] = source.DataItems.Select(item => item.Y2).ToArray();
            return dataArray;
        }

        public override string ToString()
        {
            return $"Type: {GetType()}, {base.ToString()}, DataItems Count: {DataItems.Count}\n";
        }
        public override string ToLongString(string format)
        {
            string result = ToString() + Environment.NewLine;
            foreach (var dataItem in DataItems)
            {
                result += dataItem.ToLongString(format);
            }
            return result;
        }
    }

}