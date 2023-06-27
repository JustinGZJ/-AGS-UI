using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace 比亚迪AGS_WPF.BydMes;

public class BydMesCom
{
    private readonly ILogger<BydMesCom> _logger;
    private readonly BydMesConfig config;
    private string URL;
    private int PORT;
    private string Site;
    private string UserName;
    private string Password;
    private string Resource;
    private string Operation;
    private string NcCode;
    private int TimeOut;

    public BydMesCom(IOptions<BydMesConfig>  configOptions, IOptions<List<UserConfig>> User_config, ILogger<BydMesCom> logger)
    {
        _logger = logger;
        config=configOptions.Value;
        Global.user_config = User_config.Value;
        URL = config.Url;
        Site = config.Site;
        UserName = config.UserName;
        Password = config.Password;
        Resource = config.Resource;
        Operation = config.Operation;
        NcCode = config.NcCode;
        TimeOut = config.TimeOut;
    }

    public void 用户验证(string userName,string password,out bool 验证结果, out string MES反馈)
    {
        MES反馈 = GetHtmlByPost(URL,
            "&message=<PRODUCTION_REQUEST><USER><SITE>" + Site + "</SITE><NAME>" + userName + "</NAME><PWD>" +
            password + "</PWD></USER></PRODUCTION_REQUEST>", TimeOut);
        验证结果 = CutResult(MES反馈);
    }

    public void 条码验证(string 产品条码, out bool 验证结果, out string MES反馈)
    {
        MES反馈 = GetHtmlByPost(URL,
            "&message=<PRODUCTION_REQUEST><START><SFC_LIST><SFC><SITE>" + Site + "</SITE><ACTIVITY>XML</ACTIVITY><ID>" +
            产品条码 + "</ID><RESOURCE>" + Resource + "</RESOURCE><OPERATION>" + Operation + "</OPERATION><USER>" +
            UserName +
            "</USER><QTY></QTY><DATE_TIME></DATE_TIME><COMPLEX>N</COMPLEX></SFC></SFC_LIST></START></PRODUCTION_REQUEST>!erpautogy03!1234567@byd",
            TimeOut);
        验证结果 = CutResult(MES反馈);
    }

    public void 离散装配(string 产品条码, string[] 部件码, out bool 验证结果, out string MES反馈)
    {
        var indentifierStr = "";
        for (int i = 0; i < 部件码.Length; i++)
        {
           var indentifier = config.IdentifierObject[i];
           var assyDataStr = $@"<IDENTIFIER_OBJECT>
                <IDENTIFIER>{indentifier.Identifier}</IDENTIFIER>
                <REVISION>{indentifier.Revision}</REVISION>
                <QTY>{indentifier.Qty}</QTY>
                <ASSY_DATA_VALUES>
                <ASSY_DATA>
                <DATA_FIELD>{indentifier.AssyDataValues.DataField}</DATA_FIELD>
                <DATA_ATTR>{部件码[i]}</DATA_ATTR>
                </ASSY_DATA>
                </ASSY_DATA_VALUES>
                </IDENTIFIER_OBJECT>{Environment.NewLine}";
           indentifierStr += assyDataStr;
        }

        string param = $@"&message=<PRODUCTION_REQUEST><ASSEMBLE_COMPONENTS>
<USER>{UserName}</USER>
<SITE>{Site}</SITE>
<PARENT_SFC>{产品条码}</PARENT_SFC>
<OPERATION>{Operation}</OPERATION>
<RESOURCE>{Resource}</RESOURCE>
<CHECK_OPER>True</CHECK_OPER>
<EVENT>baseFinished:AssemblyPoint</EVENT>
<IDENTIFIER_LIST>
{indentifierStr}
</IDENTIFIER_LIST>
</ASSEMBLE_COMPONENTS>
</PRODUCTION_REQUEST>!erpautogy03!1234567@byd";
        MES反馈 = GetHtmlByPost( URL,
            param,
            TimeOut);
        验证结果 = CutResult(MES反馈);
    }

    public void 工单绑定(string 工单, out bool 验证结果, out string MES反馈)
    {
        MES反馈 = GetHtmlByPost(URL,
    $@"&message=<PRODUCTION_REQUEST>
< RESOURCE_BANDING_SHOPORDER >
< SITE > {Site} </ SITE >
< NAME > {UserName} </ NAME >
< PWD > {Password} </ PWD >
< RESOURCE > {Resource} </ RESOURCE >
< SHOPORDER > {工单} </ SHOPORDER >
</ RESOURCE_BANDING_SHOPORDER >
</ PRODUCTION_REQUEST > ",
    TimeOut);
        验证结果 = CutResult(MES反馈);
    }

    public void 条码上传(
        bool 测试结果,
        string 产品条码,
        string 文件版本,
        string 软件版本,
        string 测试项,
        out bool 验证结果,
        out string MES反馈)
    {
        string str = !测试结果 ? ErrorValidate(产品条码, 文件版本, 软件版本, 测试项) : PassValidate(产品条码, 文件版本, 软件版本, 测试项);
        MES反馈 = str;
        验证结果 = CutResult(MES反馈);
    }

    private string PassValidate(string 产品条码, string 文件版本, string 软件版本, string 测试项) =>
        GetHtmlByPost(URL,
            "&message=PASS<PRODUCTION_REQUEST><COMPLETE><SFC_LIST><SFC><SITE>" + Site +
            "</SITE><ACTIVITY>XML</ACTIVITY><ID>" + 产品条码 + "</ID><RESOURCE>" + Resource + "</RESOURCE><OPERATION>" +
            Operation + "</OPERATION><USER>" + UserName +
            "</USER><QTY>1</QTY><DATE_TIME></DATE_TIME><DATE_STARTED></DATE_STARTED></SFC></SFC_LIST></COMPLETE></PRODUCTION_REQUEST>!erpautogy03!1234567@byd!PASS," +
            文件版本 + "," + 软件版本 + 测试项, TimeOut);

    private string ErrorValidate(string 产品条码, string 文件版本, string 软件版本, string 测试项) =>
        GetHtmlByPost( URL,
            "&message=ERROR<PRODUCTION_REQUEST><NC_LOG_COMPLETE><SITE>" + Site + "</SITE><OWNER TYPE=\"USER\">" +
            UserName + "</OWNER><NC_CONTEXT>" + 产品条码 +
            "</NC_CONTEXT><QTY></QTY><IDENTIFIER></IDENTIFIER><FAILURE_ID></FAILURE_ID><DEFECT_COUNT>1</DEFECT_COUNT><COMMENTS></COMMENTS><DATE_TIME></DATE_TIME><RESOURCE>" +
            Resource + "</RESOURCE><OPERATION>" + Operation +
            "</OPERATION><ROOT_CAUSE_OPER></ROOT_CAUSE_OPER><NC_CODE>" + NcCode +
            "</NC_CODE><ACTIVITY>XML</ACTIVITY></NC_LOG_COMPLETE></PRODUCTION_REQUEST>!erpautogy03!1234567@byd!ERROR," +
            文件版本 + "," + 软件版本 + 测试项, TimeOut);

    private bool CutResult(string html) => html.Contains("</b>Y</td>");

    private string GetHtmlByPost(string URL, string Param, int TimeOut)
    {
        string str;
        _logger.LogInformation("POST:"+URL+Param);

        try
        {
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(Param);
            HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(URL);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Accept = "*/*";
            httpWebRequest.UserAgent = "Mozilla/4.0(compatible;MSIE 6.0;Windows NT 5.1;SV1;Maxthon;.NET CLR 1.1.4322)";
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = (long) bytes.Length;
            httpWebRequest.Timeout = TimeOut;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response = (HttpWebResponse) httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("GB2312"));
            str = streamReader.ReadToEnd();
            streamReader.Close();
            httpWebRequest.Abort();
            response.Close();
        }
        catch (Exception ex)
        {
            str = ex.Message;
            _logger.LogError(ex.Message);
        }
        var result = str.Replace("&lt;", "<").Replace("&gt;", ">");
        _logger.LogInformation(result);
        return result;
    }
}