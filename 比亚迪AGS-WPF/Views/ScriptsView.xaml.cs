using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using 比亚迪AGS_WPF.Services;
using 比亚迪AGS_WPF.ViewModels;

namespace 比亚迪AGS_WPF.Views;

public partial class ScriptsView : UserControl
{
    ObservableCollection<FileSystemItem> folderList = new();
    public ScriptsView()
    {
        InitializeComponent();
        DataContext = new ScriptsViewModel();
         CodeTextEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("Python");
    }
}