using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 比亚迪AGS_WPF.ViewModels
{
    public  class EnquireViewModel
    {
        public EnquireViewModel()
        {

        }
        
    }


    public class FileSystemItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsFolder { get; set; }
        public ObservableCollection<FileSystemItem> SubItems { get; set; }
        //public ObservableCollection<string> DataList { get; set; }
    }

    public class MyData
    {
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
    }
}
