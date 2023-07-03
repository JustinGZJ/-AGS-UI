using System.Collections.Generic;
using System.ComponentModel;

namespace 比亚迪AGS_WPF.BydMes;
public class Endpoint
{
    public string EndpointUrl { get; set; }
    public string SecurityPolicyUri { get; set; }
}

public class MappedEndpoint
{
    public string RequestedUrl { get; set; }
    public Endpoint Endpoint { get; set; }
}
public class RootConfig 
{
    public string Title { get; set; }
    public string Version { get; set; }
    public int ServerPort { get; set; }
    public string PhoneNumber { get; set; }
    public List<MappedEndpoint> MappedEndpoints { get; set; }
    public BydMesConfig BydMesConfig { get; set; }
}

public class BydMesConfig
{
    public string Url { get; set; }
    public string Site { get; set;}
    public string UserName { get;set; }
    public string Password { get; set;}
    public string Resource { get; set;}
    public string Operation { get; set;}
    public string NcCode { get; set;}
    public int TimeOut { get; set;}
    [Editor(typeof(HandyControl.Controls.ListPropertyEditor), typeof(HandyControl.Controls.PropertyEditorBase))]
    public List<IdentifierObject> IdentifierObject { get; set; }
}

public class AssyData
{
    public string DataField { get; set; }
    public string DataAttr { get; set; }
}

public class IdentifierObject
{
    public string Identifier { get; set; }
    public string Revision { get; set; }
    public string Qty { get; set; }
    public AssyData AssyDataValues { get; set; }
}
