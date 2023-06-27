using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
using 比亚迪AGS_WPF.ViewModels;

namespace 比亚迪AGS_WPF.Views
{
    /// <summary>
    /// ConfigView.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigView 
    {
        public ConfigView()
        {
            InitializeComponent();
            this.DataContext = App.Current.Services.GetService<ConfigViewModel>();
           
        }

        private void myDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // 获取编辑后的值
            string newValue = ((TextBox)e.EditingElement).Text;

            // 获取编辑的行和列
            int rowIndex = e.Row.GetIndex();
            int columnIndex = e.Column.DisplayIndex;

            // 更新数据源
            MyDataClass editedItem = (MyDataClass)ConfigDataGrid.Items[rowIndex];
            if (columnIndex == 0)
            {
                editedItem.Column1 = newValue;
            }
            else
            {
                editedItem.Column2 = (newValue);
            }
        }
    }
}
