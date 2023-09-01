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


        private static void SaveFile(DataUploadMessage message, IEnumerable<TestItem> testItems)
        {
            // 保存文件
            var fileName = Path.Combine( 
                AppDomain.CurrentDomain.BaseDirectory ,
                "Data", 
                DateTime.Now.ToString("yyyy-MM-dd") + ".csv");
            // 检查目录是否存在,+ DateTime.Now.ToString("yyyy-MM") +"\\"
            if (!Directory.Exists(Path.Combine( AppDomain.CurrentDomain.BaseDirectory , "Data")))
            {
                Directory.CreateDirectory(Path.Combine( AppDomain.CurrentDomain.BaseDirectory , "Data"));
            }

            // 把数据放如dictionary
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("时间", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            dictionary.Add("站位", message.Station);
            dictionary.Add("条码", message.Code);
            dictionary.Add("结果", message.Result);
     
            foreach (var item in testItems)
            {
                dictionary.TryAdd(item.Name, item.Value);
            }

            // 把dictionary内的数据追加到文件，如果文件不存在则追加表头
            //检查文件是否存在
            var fileExists = File.Exists(fileName);
            var header = string.Join(",", dictionary.Keys);
            //添加表头到文件
            if (!fileExists)
            {
                File.WriteAllText(fileName, header + Environment.NewLine);
            }

            //将数据追加到文件
            File.AppendAllText(fileName, string.Join(",", dictionary.Values) + Environment.NewLine);
        }
    }
}