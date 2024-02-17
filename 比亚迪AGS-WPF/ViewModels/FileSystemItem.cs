using System;
using System.Collections.ObjectModel;

namespace 比亚迪AGS_WPF.ViewModels
{
    public class FileSystemItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string? Path { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsFolder { get; set; }
        public ObservableCollection<FileSystemItem> SubItems { get; set; }
    }
    
}
