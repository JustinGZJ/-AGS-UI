using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;

namespace 比亚迪AGS_WPF.ViewModels;

public partial class PlotViewModel: ObservableObject
{
    [ObservableProperty]
    ObservableCollection<XyPlotViewModel> _xyPlotViewModels = new();

    public PlotViewModel()
    {
        Random random = new Random();
        for (int i = 0; i < 2; i++)
        {
            XyPlotViewModel xyPlotViewModel = new();
            xyPlotViewModel.SetTitleAndAxis($"Plot {i}", "Freq(Hz)", "Loss(dB)");
            // 添加Series1
            xyPlotViewModel.AddOrUpdateSeries("Series1", GenerateRandomDataPoints(random, 3), System.Drawing.Color.Red);
            // 添加Series2
            xyPlotViewModel.AddOrUpdateSeries("Series2", GenerateRandomDataPoints(random, 3), System.Drawing.Color.Blue);
            // 添加Series3
            xyPlotViewModel.AddOrUpdateSeries("Series3", GenerateRandomDataPoints(random, 3), System.Drawing.Color.Green);

            XyPlotViewModels.Add(xyPlotViewModel);
        }
    }

    private List<DataPoint> GenerateRandomDataPoints(Random random, int count)
    {
        List<DataPoint> dataPoints = new List<DataPoint>();
        for (int i = 0; i < count; i++)
        {
            double y = random.NextDouble() * 100; // Generate a random y value between 0 and 100
            dataPoints.Add(new DataPoint(i, y));
        }
        return dataPoints;
    }
}