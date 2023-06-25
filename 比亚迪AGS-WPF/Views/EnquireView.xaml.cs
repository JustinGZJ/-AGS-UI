using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using 比亚迪AGS_WPF.Services;
using 比亚迪AGS_WPF.ViewModels;

namespace 比亚迪AGS_WPF.Views
{
    /// <summary>
    /// EnquireView.xaml 的交互逻辑
    /// </summary>
    public partial class EnquireView : Window
    {
        public EnquireView()
        {
            InitializeComponent();
            this.DataContext = App.Current.Services.GetService<EnquireViewModel>();
            string folderPath = "./Data";
            ObservableCollection<FileSystemItem> folderList = ReadFile.GetCsvFiles(folderPath);

            myTreeView.ItemsSource = folderList;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            FileSystemItem selectedItem = e.NewValue as FileSystemItem;
            if (selectedItem != null)
            {
                if (selectedItem.IsFolder)
                {
                    // 如果选择的是文件夹，则不进行任何操作
                }
                else if (selectedItem.Type == "csv")
                {
                    // 如果选择的是 CSV 文件，则使用 ReadCsvFile 方法读取文件的内容
                    DataTable  dataList =new DataTable() ;
                    DataTable dataTable = ReadFile.ReadCsvFile(selectedItem.Path); ; // 获取数据表格
                    CSVDataGrid.DataContext = dataTable.DefaultView; // 将数据表格的 DefaultView 设置为 DataGrid 控件的 DataContext 属性
                   // CSVDataGrid.ItemsSource = (System.Collections.IEnumerable)dataList;
                }
                else
                {
                    // 如果选择的是其他类型的文件，则可以根据需要进行相应的处理
                }
            }
        }
    }
}
