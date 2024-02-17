﻿using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using 比亚迪AGS_WPF.Services;
using 比亚迪AGS_WPF.ViewModels;

namespace 比亚迪AGS_WPF.Views
{
    /// <summary>
    /// EnquireView.xaml 的交互逻辑
    /// </summary>
    public partial class DataView 
    {
        ObservableCollection<FileSystemItem> folderList = new();
        public DataView()
        {
            InitializeComponent();
             ReadFile.FilterFilesWithExtension(AppPath.DataPath, folderList);
            myTreeView.ItemsSource = folderList;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is FileSystemItem selectedItem)
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
