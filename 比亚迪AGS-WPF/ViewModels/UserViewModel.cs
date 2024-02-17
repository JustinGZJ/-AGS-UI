using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace 比亚迪AGS_WPF.ViewModels;

public class User
{
    public string CardNumber { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string UserLevel { get; set; }
}


public partial class UserViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<User> _users;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteUserCommand))]
    [NotifyCanExecuteChangedFor(nameof(UpdateUserCommand))]
    private User _selectedUser;
    public UserViewModel()
    {
        var users = ConfigHelper.LoadConfig<List<User>>("user.json");
        this.Users= new ObservableCollection<User>(users);
    }

    [RelayCommand(CanExecute = nameof(CanAddUser))]
    private void AddUser()
    {
        Users.Add(new User {CardNumber = "0", UserName = "New User", Password = "Password", UserLevel = "user"});
    }

    private bool CanAddUser()
    {
        return true;
    }
    [RelayCommand(CanExecute = nameof(CanDeleteUser))]
    private void DeleteUser()
    {
        Users.Remove(SelectedUser);
    }

    private bool CanDeleteUser()
    {
        return SelectedUser != null;
    }
   [RelayCommand(CanExecute = nameof(CanUpdateUser))]
    private void UpdateUser()
    {
        // 更新用户逻辑
    }

    private bool CanUpdateUser()
    {
        return SelectedUser != null;
    }
    [RelayCommand]
    private void SaveUser()
    {   
        ConfigHelper.SaveConfig(this.Users.ToList(), "user.json");
    }

    public string[] UserLevels => new[] {"USER", "ADMIN","QE","PE"};
    
}