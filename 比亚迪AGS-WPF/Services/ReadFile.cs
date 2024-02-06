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

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                bool isFirstRow = true;
                while (!parser.EndOfData)
                {
                    string[]? fields = parser.ReadFields();

                    if (isFirstRow)
                    {
                        if (fields != null)
                            foreach (string field in fields)
                            {
                                dataTable.Columns.Add(field);
                            }

                        isFirstRow = false;
                    }
                    else
                    {
                        while (fields != null && dataTable.Columns.Count < fields.Length)
                        {
                            dataTable.Columns.Add("");
                        }

                        dataTable.Rows.Add(fields);
                    }
                }
            }

            // 设置 Locale 属性以避免在 UI 界面上显示乱码
            dataTable.Locale = CultureInfo.InvariantCulture;
            return dataTable;
        }


        public static void FilterFilesWithExtension(string folderPath, ObservableCollection<FileSystemItem> folderList,string filter = "csv")
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(folderPath);
            foreach (string fileName in fileEntries)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Extension == ($".{filter}"))
                {
                    folderList.Add(new FileSystemItem()
                    {
                        Name = fileInfo.Name,
                        Path = fileInfo.FullName,
                        Type = filter,
                    });
                }
            }
            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(folderPath);
            foreach (string subdirectory in subdirectoryEntries)
            {
               var item =(new FileSystemItem()
                {
                    Name = new DirectoryInfo(subdirectory).Name,
                    Path = subdirectory,
                    Type = "folder",
                    IsFolder = true,
                    SubItems = new ObservableCollection<FileSystemItem>()
                });
                folderList.Add(item);
                FilterFilesWithExtension(subdirectory, item.SubItems,filter);
            }
            
        }
    }
}