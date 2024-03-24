using System;
using System.Collections;
using System.Collections.Generic;

namespace lab_code_3_3
{
    public abstract class V1Data : IEnumerable<DataItem>
    {
        public string Key { get; set; }
        public abstract IEnumerator<DataItem> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public abstract DataItem xMaxItem { get; }
        public DateTime Date { get; set; }
        public abstract double MaxDistance { get; }
        public V1Data(string key, DateTime date)
        {
            Key = key;
            Date = date;
        }
        public abstract string ToLongString(string format);
        public override string ToString()
        {
            return $"Key: {Key}, Date: {Date.ToString()}, MaxDistance: {MaxDistance}";
        }
    }
}