using System.Collections.ObjectModel;
using Microsoft.VisualBasic;

namespace lab_code_3_3
{
    public class V1MainCollection : ObservableCollection<V1Data>
    {
        public bool Contains(string key)
        {
            return this.Any(item => item.Key == key);
        }
        public double AverageField
        {
            get
            {
                // double sum = this
                //     .SelectMany(item => item)
                //     .Select(i => Math.Sqrt(i.Y1 * i.Y1 + i.Y2 * i.Y2))
                //     // .DefaultIfEmpty()
                //     .Average();
                // return double.IsNaN(sum) ? double.NaN : sum;
                var sum = (from i in Items
                                from j in i select Math.Sqrt(j.Y1 * j.Y1 + j.Y2 * j.Y2));
                if (!sum.Any()){
                    return double.NaN;
                }                
                var res = sum.Sum() / sum.Count();
                return res;
            }
        }
        public DataItem? MaxDifferenceItem
        {
            get
            {
                var query = this
                    .SelectMany(item => item)
                    .OrderBy(dataItem => Math.Abs(Math.Sqrt(dataItem.Y1 * dataItem.Y1 + dataItem.Y2 * dataItem.Y2) - AverageField));
    
                return query != null ? (DataItem?)query.Last() : null;
            }
        }

        public IEnumerable<double> DistinctX
        {
            get
            {
                var xValues = this
                    // .Where(item => item is V1DataList)
                    .SelectMany(item => item)
                    .Select(item => item.X);
                //     .SelectMany(item => ((V1DataList)item).DataItems.Select(dataItem => dataItem.X))
                    // .Distinct()
                var tmp = this
                    .SelectMany(item => item)
                    .Where(item => xValues.Count(x => x == item.X) > 1)
                    .Select(item => item.X)
                    .Distinct()
                    .OrderBy(x => x);
                return tmp.Any() ? tmp : null;
                // return xValues.Any() ? xValues : null;
            }
        }
        public bool Add(V1Data v1Data)
        {
            if (!Contains(v1Data.Key))
            {
                base.Add(v1Data);
                return true;
            }
            return false;
        }
        public V1MainCollection(int nV1DataArray, int nV1DataList)
        {
            for (int i = 0; i < nV1DataArray; i++)
            {
                V1DataArray v1DataArray = new V1DataArray($"Array {i}", DateTime.Now, 1+nV1DataArray, i, nV1DataArray, (double x, ref double y1, ref double y2) =>{y1 = x * 2; y2 = x * 3;});
                Add(v1DataArray);
            }

            for (int i = 0; i < nV1DataList; i++)
            {
                double[] x = {i+1.5, i+2.0, i+3.0};
                V1DataList v1DataList = new V1DataList($"List {i}", DateTime.Now, x,  (double x) =>{return new DataItem(x, x * 2.0, x * 3.0);});
                Add(v1DataList);
            }
        }
        public List<DataItem> Max
        {
            get
            {
                List<DataItem> xMax = new List<DataItem>();
                foreach (var item in this)
                {
                    xMax.Add(item.xMaxItem);
                }
                return xMax;
            }
        }
        public string ToLongString(string format)
        {
            string result = string.Empty;
            foreach (var item in this)
            {
                result += '\n' + item.ToLongString(format);
            }
            return result;
        }
        public override string ToString()
        {
            string result = string.Empty;
            foreach (var item in this)
            {
                result += item.ToString() + "\n";
            }
            return result;
        }
    }
}