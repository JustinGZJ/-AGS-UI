using System.ComponentModel;

namespace 比亚迪AGS_WPF.Config;
public class RootConfig 
{
    [DisplayName("软件名称")]
    public string Title { get; set; }
    [DisplayName("软件版本")]
    public string Version { get; set; }
    [DisplayName("联系电话")]
    public string PhoneNumber { get; set; }
    [DisplayName("公司名称")]
    public string Company { get; set; }
    [DisplayName("数据路径")]
    public string? DataPath { get; set; }= "data";
    [DisplayName("Python路径")]
    public string PythonPath { get; set; }
    [DisplayName("当前脚本")]
    public string CurrentScript { get; set; }
    [DisplayName("Python脚本路径")]
    public string? PythonScriptHome { get; set; }
    [DisplayName("MQTT服务器地址")]
    public string MqttBrokerAddress { get; set; } = "127.0.0.1";
    [DisplayName("MQTT服务器端口")]
    public int MqttBrokerPort { get; set; } = 1883;
    [DisplayName("MQTT主题")]
    public string MqttTopic { get; set; } = "inst/AGS";
    [DisplayName("当前用户")]
    public string CurrentUser { get; set; }="admin";
}