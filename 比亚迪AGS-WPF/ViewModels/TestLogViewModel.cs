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
    public class TestLogViewModel : ObservableRecipient
    {
        /// <summary>
        /// 日志
        /// </summary>
        public ObservableCollection<TestLog> TestLogs { get; set; } = new();

        /// <summary>
        /// 测试项目
        /// </summary>
        public ObservableCollection<TestItem> TestItems { get; } = new();

        public TestLogViewModel()
        {
            WeakReferenceMessenger.Default.Register<TestItemsChangeMessage>(this, ((recipient, messages) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    #region 更新测试项目,如果不存在则添加，如果存在则更新
                    foreach (var message in messages.Value)
                    {
                        var item = TestItems.FirstOrDefault(x => x.Name == message.Name);
                        if (item == null)
                        {
                            TestItems.Add(message);
                        }
                        else
                        {
                            item.Value = message.Value;
                            item.Result = message.Result;
                        }
                    }
                    // 通知更新testitems
                    OnPropertyChanged(nameof(TestItems));
                    #endregion

                    // TestItems.Add(message);
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
                    if (TestLogs.Count >= 200)
                        TestLogs.RemoveAt(TestLogs.Count - 1);
                });
            }));
        }
    }
}