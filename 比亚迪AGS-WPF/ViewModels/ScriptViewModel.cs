using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using 比亚迪AGS_WPF.Services;

namespace 比亚迪AGS_WPF.ViewModels
{
    public class ScriptsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FileSystemItem> _folderList;
        private FileSystemItem _selectedItem;
        private string _codeText;

        public ObservableCollection<FileSystemItem> FolderList
        {
            get { return _folderList; }
            set
            {
                _folderList = value;
                OnPropertyChanged();
            }
        }

        public FileSystemItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (_selectedItem != null && _selectedItem.Type == "py")
                {
                    CodeText = File.ReadAllText(_selectedItem.Path);
                }
            }
        }
        

        public string CodeText
        {
            get { return _codeText; }
            set
            {
                _codeText = value;
                OnPropertyChanged();
            }
        }

        public ScriptsViewModel()
        {
            FolderList = new ObservableCollection<FileSystemItem>();
            ReadFile.FilterFilesWithExtension(AppPath.ScriptsPath, FolderList, "py");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}