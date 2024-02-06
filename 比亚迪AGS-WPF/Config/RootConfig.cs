using System.Collections.Generic;
using System.ComponentModel;

namespace 比亚迪AGS_WPF.Config;
public class RootConfig 
{
    public string Title { get; set; }
    public string Version { get; set; }
    public string PhoneNumber { get; set; }
    public string Company { get; set; }

    public string PythonPath { get; set; }

    public string MqttBrokerAddress { get; set; } = "127.0.0.1";

    public int MqttBrokerPort { get; set; } = 1883;
}