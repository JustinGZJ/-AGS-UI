using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using 比亚迪AGS_WPF.BydMes;

namespace 比亚迪AGS_WPF.ViewModels
{
    public  class ConfigViewModel:ObservableObject
    {
        public BydMesConfig MesConfig { get; private set; }

        public ConfigViewModel(IOptions<BydMesConfig> mes)
        {
            MesConfig = mes.Value;
        }
    }
}
