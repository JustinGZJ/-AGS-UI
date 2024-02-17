using System;
using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using 比亚迪AGS_WPF.Config;
using 比亚迪AGS_WPF.Services;

namespace 比亚迪AGS_WPF.ViewModels;

public partial class ScriptsViewModel : ObservableObject
{
    private ObservableCollection<FileSystemItem> _folderList;
    private FileSystemItem? _selectedItem;

    [ObservableProperty] private string _codeText;
    [ObservableProperty] private string _codeOutput;
    [ObservableProperty] private string _functionName = "main";

    public ObservableCollection<FileSystemItem> FolderList
    {
        get => _folderList;
        set => SetProperty(ref _folderList, value);
    }
    public FileSystemItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value) && _selectedItem?.Type == "py")
            {
                CodeText = File.ReadAllText(_selectedItem.Path);
            }
            // 通知运行按钮状态改变
            RunScriptCommand.NotifyCanExecuteChanged();
            // 通知保存按钮状态改变
            SaveScriptCommand.NotifyCanExecuteChanged();
        }
    }

    public ScriptsViewModel(RootConfig config)
    {
        FolderList = new ObservableCollection<FileSystemItem>();
        ReadFile.FilterFilesWithExtension(config.PythonScriptHome, FolderList, "py");
    }

    // 保存代码
    [RelayCommand(CanExecute = nameof(CanRunScript))]
    public void SaveScript()
    {
        File.WriteAllText(SelectedItem?.Path, CodeText);
    }

    // 运行代码
    [RelayCommand(CanExecute = nameof(CanRunScript))]
    public void RunScript()
    {
        SaveScript();
        var scriptExecutor = App.Current.Services.GetService<IScriptExecutor>();
       
        try
        {
            // 获取文件的相对路径
            var result = scriptExecutor?.Execute(_selectedItem?.Path, FunctionName);
            CodeOutput = result?.ToString() ?? "null";
        }
        catch (Exception e)
        {
            CodeOutput = e.Message;
        }
    }

    bool CanRunScript()
    {
        return SelectedItem != null;
    }
}