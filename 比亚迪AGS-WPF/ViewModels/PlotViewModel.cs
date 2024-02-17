using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using OxyPlot;
using 比亚迪AGS_WPF.DataObject;
using 比亚迪AGS_WPF.Utils;

namespace 比亚迪AGS_WPF.ViewModels;

public partial class PlotViewModel : ObservableRecipient, IRecipient<ScriptResponse>
{
    [ObservableProperty] ObservableCollection<XyPlotViewModel> _xyPlotViewModels = new();

    public PlotViewModel()
    {
        IsActive = true;
    }

    public void Receive(ScriptResponse message)
    {
        if (message.Topic == "Measure")
        {
            // 使用ui线程更新
            Application.Current.Dispatcher.Invoke(() =>
            {
                XyPlotViewModels.Clear();
                var measure = message.Result.Cask<Measure>();
                var groups = measure.TestItems.GroupBy(x => x.Category);
                foreach (var group in groups)
                {
                    if (!group.All(x => double.TryParse(x.Name, out double _)))
                        return;
                    var xyPlotViewModel = new XyPlotViewModel();
                    xyPlotViewModel.SetTitleAndAxis(group.Key, "Freq", "dB");

                    var dataPoints = group.Select(x => new
                    {
                        Name = double.Parse(x.Name),
                        Value = (double)x.Value!,
                        Upper = (double)x.Upper!,
                        Lower = (double)x.Lower!
                    }).ToList();
                    xyPlotViewModel.AddOrUpdateSeries("Value", 
                        dataPoints.Select(x => new DataPoint(x.Name, x.Value)).ToList(),
                        System.Drawing.Color.Blue);
                    xyPlotViewModel.AddOrUpdateSeries("Upper", 
                        dataPoints.Select(x => new DataPoint(x.Name, x.Upper)).ToList(),
                        System.Drawing.Color.Red);
                    xyPlotViewModel.AddOrUpdateSeries("Lower",
                        dataPoints.Select(x => new DataPoint(x.Name, x.Lower)).ToList(),
                        System.Drawing.Color.OrangeRed);
                    XyPlotViewModels.Add(xyPlotViewModel);
                }
            });
        }
    }
}