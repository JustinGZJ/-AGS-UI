using System;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot.Axes;

namespace 比亚迪AGS_WPF.ViewModels
{
    public partial class XyPlotViewModel : ObservableRecipient
    {
        [ObservableProperty]
        PlotModel _plotModel = new ();
        
       public XyPlotViewModel()
        {
            
        }
        // 设置图表标题和X轴和Y轴的标题
        public void SetTitleAndAxis(string title, string xTitle, string yTitle)
        {
            PlotModel.Title = title;
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = xTitle });
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = yTitle });
            
        }
        
        public void AddOrUpdateSeries(string seriesTitle, List<DataPoint> dataPoints,Color color)
        {
            var lineSeries = PlotModel.Series.FirstOrDefault(s => s.Title == seriesTitle) as LineSeries;
            if (lineSeries == null)
            {
                // 如果不存在，则添加新的Series
                lineSeries = new LineSeries
                {
                    Title = seriesTitle,
                    Color = OxyColor.FromArgb(color.A, color.R, color.G, color.B)
                };
         
                
                
                PlotModel.Series.Add(lineSeries);
            }

            // 更新数据点
            lineSeries.Points.Clear();
            lineSeries.Points.AddRange(dataPoints);
           
            // 重新绘制图表
            PlotModel.InvalidatePlot(true);
        }
    }
}