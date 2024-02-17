using System.Collections.ObjectModel;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using 比亚迪AGS_WPF.ViewModels;

namespace 比亚迪AGS_WPF.Views;

public partial class ScriptsView : UserControl
{
    ObservableCollection<FileSystemItem> folderList = new();
    public ScriptsView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<ScriptsViewModel>();
        CodeTextEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("Python");
    }
}