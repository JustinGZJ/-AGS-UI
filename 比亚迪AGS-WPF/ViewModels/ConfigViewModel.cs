using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 比亚迪AGS_WPF.BydMes;

namespace 比亚迪AGS_WPF.ViewModels
{
    public  class ConfigViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<MyDataClass> _myData;

        public ObservableCollection<MyDataClass> MyData
        {
            get { return _myData; }
            set
            {
                _myData = value;
                OnPropertyChanged(nameof(MyData));
            }
        }

        private readonly List<UserConfig> config;

        //public event PropertyChangedEventHandler? PropertyChanged;
        // 属性改变事件通知
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isEditing;

        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                _isEditing = value;
                OnPropertyChanged(nameof(IsEditing));
            }
        }

        public ICommand CellEditEndingCommand { get; }

        public ConfigViewModel(IConfiguration configuration)
        {
            MyData = new ObservableCollection<MyDataClass>();
            IConfigurationSection section = configuration.GetSection("UserConfig");
            config = configuration.GetSection("UserConfig").Get<List<UserConfig>>();

            
            // 初始化命令
            CellEditEndingCommand = new RelayCommand<DataGridCellEditEndingEventArgs>(OnCellEditEnding); 

            foreach (var userConfig in config)
            {
               var jj= kk(userConfig.Value);
                MyData.Add(new MyDataClass { Column1 = userConfig.Section, Column2 = userConfig.Key, Column3 =jj.SN, Column4=jj.AttachSN });
            }

        }

        private void OnCellEditEnding(DataGridCellEditEndingEventArgs? obj)
        {
            IsEditing = false;
            var args = obj as DataGridCellEditEndingEventArgs;
            if (args != null)
            {
                var editedItem = args.Row.Item as MyDataClass;
                if (editedItem != null)
                {
                    var cell = args.Column.GetCellContent(args.Row);
                    if (cell != null)
                    {
                        if (args.Column.DisplayIndex == 0)
                        {
                            // 更新Column1属性
                            editedItem.Column1 = ((TextBlock)cell).Text;
                        }
                        else if (args.Column.DisplayIndex == 1)
                        {
                            // 更新Column2属性
                            int newValue;
                            if (int.TryParse(((TextBlock)cell).Text, out newValue))
                            {
                                editedItem.Column2 = newValue.ToString();
                            }
                        }
                    }
                }
            }
        }


        // 处理CellEditEnding事件的方法
        private void OnCellEditEnding(object parameter, DataGridCellEditEndingEventArgs e)
        {
          
        }

        public UserCon kk(string json)
        {
           return JsonConvert.DeserializeObject<UserCon>(json);
        }

        public void SaveCpnfig()
        {
            var config = new
            {
                MySetting = "NewValue"
            };

            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText("Users.json", json);
        }
    }
 
    public class UserCon
    {
        public string SN { get; set; }
        public string AttachSN { get; set; }

    }

    public class MyDataClass
    {
        public string Column1 { get; set; }
    
        public string Column2 { get; set; }
    
        public string Column3 { get; set; }

        public string Column4 { get; set; }
    }
}
