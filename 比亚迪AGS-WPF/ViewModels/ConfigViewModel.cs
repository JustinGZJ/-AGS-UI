using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;

using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using 比亚迪AGS_WPF.BydMes;

namespace 比亚迪AGS_WPF.ViewModels
{
    public partial  class ConfigViewModel:ObservableObject
    {
        public RootConfig Config { get; }
        public string OpcUrl { get; set; }
        
        

        public BydMesConfig MesConfig { get; private set; }

        public ConfigViewModel()
        {
            Config = JsonConvert.DeserializeObject<RootConfig>(File.ReadAllText("AppSettings.json"));
            MesConfig = Config.BydMesConfig;
            OpcUrl= Config.MappedEndpoints[0].Endpoint.EndpointUrl;
        }



        [RelayCommand]
        public void Save()
        {
            string json = JsonConvert.SerializeObject(Config,Formatting.Indented);
            File.WriteAllText("AppSettings.json", json);
            HandyControl.Controls.MessageBox.Success("保存成功", "提示");
        }
    }
}
