using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using 比亚迪AGS_WPF.ViewModels;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;

namespace 比亚迪AGS_WPF.Services
{

   

   public static class ReadFile 
   {
        public static DataTable ReadCsvFile(string filePath)
        {
            DataTable dataTable = new DataTable();

            using (TextFieldParser parser = new TextFieldParser(filePath, Encoding.GetEncoding("GB2312")))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                bool isFirstRow = true;
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    if (isFirstRow)
                    {
                        foreach (string field in fields)
                        {
                            dataTable.Columns.Add(field);
                        }
                        isFirstRow = false;
                    }
                    else
                    {
                        dataTable.Rows.Add(fields);
                    }
                }
            }
            // 设置 Locale 属性以避免在 UI 界面上显示乱码
            dataTable.Locale = CultureInfo.InvariantCulture;
            return dataTable;
        }

      

        public static List<MyData> ReadCsvFile1(string filePath)
        {
            List<MyData> dataList = new List<MyData>();
           // File.ReadAllLines(filePath, Encoding.Default);//str1[2].Split(',');
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    MyData data = new MyData()
                    {
                        Column1 = values[0],
                        Column2 = values[1],
                        Column3 = values[2]
                    };

                    dataList.Add(data);
                }
            }

            return dataList;
        }


        public static ObservableCollection<FileSystemItem>  GetCsvFiles(string folderPath)
        {
            ObservableCollection<FileSystemItem> folderList = new ObservableCollection<FileSystemItem>();
            List<string> filess = new List<string>();

            string path = @"X:\XXX\XX";
            DirectoryInfo root = new DirectoryInfo(folderPath);
            string[] files = Directory.GetFiles(folderPath, "*.csv");
            foreach (FileInfo f in root.GetFiles())
            {
                 filess.Add(f.Name);
                filess.Add(f.FullName);
                //string fullName = f.FullName;
            }

            DirectoryInfo folder = new DirectoryInfo(folderPath);
            
            foreach (FileInfo file in folder.GetFiles("*.csv"))
            {
                List<MyData> dataList = ReadCsvFile1(file.FullName);

                folderList.Add(new FileSystemItem()
                {
                    Name = file.Name,
                    Path= file.FullName,
                    Type = "csv",
                });
            }

            return folderList;
        }
    }
   
}
