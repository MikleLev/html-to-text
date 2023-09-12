using HtmlAgilityPack;
using System;
using System.IO;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        // Устанавливаем кодировку консоли в UTF-8
        Console.OutputEncoding = Encoding.UTF8;

        string filePath = "C:\\Users\\Миша\\source\\repos\\html-to-text\\example.txt"; // Укажите путь к файлу HTML

        if (File.Exists(filePath))
        {
            string htmlContent = File.ReadAllText(filePath);

            string txtContent = ConvertHtmlToTxt(htmlContent);
            Console.WriteLine(txtContent);
        }
        else
        {
            Console.WriteLine("HTML file not found.");
        }
    }

    static string ConvertHtmlToTxt(string htmlContent)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        StringBuilder txtContent = new StringBuilder();

        ProcessNode(doc.DocumentNode, txtContent);

        return txtContent.ToString();
    }

    static void ProcessNode(HtmlNode node, StringBuilder txtContent)
    {
        if (node.NodeType == HtmlNodeType.Text)
        {
            string text = node.InnerText.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                txtContent.AppendLine(text);
            }
        }
        else if (node.Name == "a")
        {
            string linkText = node.InnerText.Trim();
            string linkUrl = node.GetAttributeValue("href", "");
            txtContent.AppendFormat("{0} ({1})", linkText, linkUrl);
            txtContent.AppendLine();
        }
        else if (node.Name == "div")
        {
            txtContent.AppendLine(node.InnerText.Trim());
        }
        else if (node.Name == "table" && node.HasClass("whitelisted-table"))
        {
            txtContent.AppendLine(ConvertTable(node));
        }
        else
        {
            foreach (var childNode in node.ChildNodes)
            {
                ProcessNode(childNode, txtContent);
            }

            if (node.Name == "ul" || node.Name == "ol")
            {
                txtContent.AppendLine();
            }
        }
    }

    static string ConvertTable(HtmlNode node)
    {
        var txtTable = new StringBuilder();
        var trNodes = node.SelectNodes(".//tr");

        if (trNodes != null)
        {
            var headerRow = trNodes[0];
            var headers = headerRow.SelectNodes(".//th");
            var columnCount = headers.Count;

            foreach (var header in headers)
            {
                txtTable.Append(header.InnerText.Trim() + "\t");
            }
            txtTable.AppendLine();

            for (int i = 1; i < trNodes.Count; i++)
            {
                var row = trNodes[i];
                var columns = row.SelectNodes(".//td");
                if (columns.Count != columnCount)
                {
                    continue; // Skip rows with inconsistent column count
                }
                foreach (var column in columns)
                {
                    txtTable.Append(column.InnerText.Trim() + "\t");
                }
                txtTable.AppendLine();
            }
        }

        return txtTable.ToString();
    }
}
