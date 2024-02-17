using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using 比亚迪AGS_WPF.DataObject;
using 比亚迪AGS_WPF.Utils;

namespace 比亚迪AGS_WPF.ViewModels;

public class TestItemsViewModel:ObservableRecipient,IRecipient<ScriptResponse>
{
    /// <summary>
    ///     测试项目
    /// </summary>
    public ObservableCollection<TestItem> TestItems { get; } = new();

    public TestItemsViewModel()
    {
        IsActive = true;
    }

    public void Receive(ScriptResponse message)
    {
        //根据message的类型确定如何反序列化result, 更新testitems还是testlogs
        if (message.Topic == "Measure")
        {
            var measure = message.Result.Cask<Measure>();
            Application.Current.Dispatcher.Invoke(() =>
            {
                var testItems = measure?.TestItems;
                //  TestItems 存在则修改不存在则添加
                if (testItems == null) return;
                foreach (var testItem in testItems)
                {
                    var item =TestItems.FirstOrDefault(x => (x.Name == testItem.Name)&&(x.Category==testItem.Category));
                    if (item != null)
                    {
                        item.Value = testItem.Value;
                        item.Result = testItem.Result;
                    }
                    else
                    {
                        TestItems.Add(testItem);
                    }
                }
     
            });
            if (measure != null) TestLogHelper.SaveMeasureLog(measure);
        }
    }
}