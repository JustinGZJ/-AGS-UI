using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using 比亚迪AGS_WPF.Services;

namespace 比亚迪AGS_WPF.ViewModels
{
    public class TestLogViewModel : ObservableObject
    {
        public ObservableCollection<TestLog> TestLogs { get; set; } = new();

        /// <summary>
        /// 测试项目
        /// </summary>
        public ObservableCollection<TestItem> TestItems { get; } = new()
        {
        };

        public TestLogViewModel()
        {
            WeakReferenceMessenger.Default.Register<DataUploadMessage>(this, ((recipient, message) =>
            {
                var testItems = message.Value.Split('!').Where(x => x.Contains(",")).Select(x =>
                {
                    var m = x.Split(',');
                    return new TestItem()
                    {
                        Name = m[0],
                        Parameter = m[1],
                        Value = m[2],
                        Result = message.Result
                    };
                });
                
                // 保存文件
               // SaveFile(message, testItems);

                // 刷新界面
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TestItems.Clear();
                    foreach (var item in testItems)
                    {
                        TestItems.Add(item);
                    }
                });
            }));
            WeakReferenceMessenger.Default.Register<TestLog>(this, ((recipient, message) =>
            {
                var log = new TestLog()
                {
                    Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Type = message.Type,
                    Log = message.Log,
                    Level = message.Level
                };
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TestLogs.Add(log);
                    if(TestLogs.Count>=200)
                        TestLogs.RemoveAt(TestLogs.Count-1);
                });
            }));
        }

        
    }
}