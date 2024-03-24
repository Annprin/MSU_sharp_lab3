using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace lab_1_wpf
{
    internal class ParseBoundaries : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double[] seg = (double[])value;
            return $"{seg[0]} {seg[1]}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                try
                {
                    return ParseString(str);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            return new double[] { 0, 0 };
        }

        private double[] ParseString(string str)
        {
            double[] edges = new double[2];
            string[] numbers = str.Split(' ');

            if (numbers.Length == 2 && double.TryParse(numbers[0], out edges[0]) && double.TryParse(numbers[1], out edges[1]))
                return edges;
            throw new Exception("Некорректный ввод");
        }
    }
}
