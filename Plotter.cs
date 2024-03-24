using lab_code_3_3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Legends;

namespace lab_1_wpf
{
    public class Plotter
    {
        private readonly SplineData data;

        public PlotModel plotModel { get; set; }

        public Plotter(SplineData data)
        {
            this.data = data;
            plotModel = new PlotModel { Title = "Результаты сплайн-аппроксимации" };
            AddSeries();
        }

        private void AddSeries()
        {
            List<DataPoint> list = new List<DataPoint>();
            var points = data.dataArray.X;
            var values = data.dataArray[0];
            for (int i = 0; i < points.Length; i++)
            {
                list.Add(new DataPoint(points[i], values[i]));
            }
            AddSeries(list, OxyColors.Red, "Дискретные значения функции");
            AddSeries(data.ResultOnAddonGrid.Select(SplinePoint => new DataPoint(SplinePoint[0], SplinePoint[1])),
                      OxyColors.Blue, "Аппроксимирующий сплайн");
        }

        private void AddSeries(IEnumerable<DataPoint> points, OxyColor color, string title)
        {
            var lineSeries = new LineSeries
            {
                Title = title,
                MarkerType = MarkerType.Diamond,
                Color = OxyColors.Transparent,
                MarkerSize = 5,
                MarkerFill = color,
                MarkerStroke = color,
                MarkerStrokeThickness = 2.3
            };

            foreach (var point in points)
                lineSeries.Points.Add(point);

            var legend = new Legend { LegendPosition = LegendPosition.LeftTop, LegendPlacement = LegendPlacement.Inside };
            plotModel.Legends.Add(legend);
            plotModel.Series.Add(lineSeries);
        }
    }
}
