﻿using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using 比亚迪AGS_WPF.BydMes;

namespace 比亚迪AGS_WPF.Services;

public class MesService
{
    public MesService(BydMesCom mes)
    {
        WeakReferenceMessenger.Default.Register<UserLoginMessage>(this, (r, m) =>
        {
            WeakReferenceMessenger.Default.Send<TestLog>(new TestLog()
            {
                Type = "MES",
                Log = $"用户登录：{m.CardNo}",
                Level = "INFO"
            });
            mes.用户验证(out bool 验证结果, out string mes反馈);

            string cleanText = RemoveHtmlTags(mes反馈);
            m.message.ReplyLine(验证结果 ? "Y" : "N" + ";" + cleanText);

            WeakReferenceMessenger.Default.Send<TestLog>(new TestLog()
            { Type = "MES", Log = $"MES反馈：{cleanText}", Level = "INFO" });
        });
        WeakReferenceMessenger.Default.Register<DataUploadMessage>(this, (r, m) =>
        {
            WeakReferenceMessenger.Default.Send<TestLog>(new TestLog()
            {
                Type = "MES",
                Log = $"数据上传：{m.Code},{m.Result},{m.Value}",
                Level = "INFO"
            });
            mes.条码上传(m.Result == "PASS", m.Code, "1.0", "1.0", m.Value, out bool 验证结果, out string mes反馈);

            string cleanText = RemoveHtmlTags(mes反馈);
            m.message.ReplyLine(验证结果 ? "Y" : "N" + ";" + cleanText);

            WeakReferenceMessenger.Default.Send<TestLog>(new TestLog()
            { Type = "MES", Log = $"cleanText：{cleanText}", Level = "INFO" });

        });
        WeakReferenceMessenger.Default.Register<CodeVerifyMessage>(this, (r, m) =>
        {
            WeakReferenceMessenger.Default.Send<TestLog>(new TestLog()
            {
                Type = "MES",
                Log = $"条码验证：{m.Code}",
                Level = "INFO"
            });
            mes.条码验证(m.Code, out bool 验证结果, out string mes反馈);

            string cleanText = RemoveHtmlTags(mes反馈);
            m.message.ReplyLine(验证结果 ? "Y" : "N" + ";" + cleanText);

            WeakReferenceMessenger.Default.Send<TestLog>(new TestLog()
            { Type = "MES", Log = $"MES反馈：{cleanText}", Level = "INFO" });
        });
        WeakReferenceMessenger.Default.Register<AssemblyMessage>(this, (r, m) =>
        {
            WeakReferenceMessenger.Default.Send<TestLog>(new TestLog()
            {
                Type = "MES",
                Log = $"离散装配：{m.Code},{string.Join(",", m.PartCodes)}",
                Level = "INFO"
            });
            mes.离散装配(m.Code, m.PartCodes, out bool 验证结果, out string mes反馈);

            string cleanText = RemoveHtmlTags(mes反馈);
            m.message.ReplyLine(验证结果 ? "Y" : "N" + ";" + cleanText);

            WeakReferenceMessenger.Default.Send<TestLog>(new TestLog()
            { Type = "MES", Log = $"MES反馈：{cleanText}", Level = "INFO" });
        });
        WeakReferenceMessenger.Default.Register<OrderBindingMessage>(this, (r, m) =>
        {

            WeakReferenceMessenger.Default.Send(new TestLog()
            {
                Type = "MES",
                Log = $"工单绑定：" + m.Order,
                Level = "INFO"
            });
            mes.工单绑定(m.Order, out bool 验证结果, out string mes反馈);

            string cleanText = RemoveHtmlTags(mes反馈);
            m.message.ReplyLine(验证结果 ? "Y" : "N" + ";" + cleanText);
            //m.message.ReplyLine(验证结果 ? "Y" : "N" + ";" + mes反馈);
            WeakReferenceMessenger.Default.Send<TestLog>(new TestLog()
            { Type = "MES", Log = $"MES反馈：{cleanText}", Level = "INFO" });
        });
    }

    static string RemoveHtmlTags(string text)
    {
        string pattern = "<table[^>]*>(.*?)</table>";
        string result = Regex.Match(text, pattern, RegexOptions.Singleline).Groups[1].Value;
        //只保留字符串<table></table>里面的内容
        Regex regex = new("<.*?>");
        string resultdeal = regex.Replace(result, "");
        //去除<>
        Regex regex1 = new(@"\s+");//去除字符串里面的空格和空行
        return regex1.Replace(resultdeal, "");
    }
}