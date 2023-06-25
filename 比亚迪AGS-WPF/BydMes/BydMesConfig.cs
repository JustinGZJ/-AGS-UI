using System.Collections.Generic;

namespace 比亚迪AGS_WPF.BydMes;

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
