using HtmlAgilityPack;
using System;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        string htmlContent = "<html><body><table><tr><th>Header 1</th><th>Header 2</th></tr><tr><td>Data 1</td><td>Data 2</td></tr></table></body></html>";
        string txtContent = ConvertHtmlToTxt(htmlContent);
        Console.WriteLine(txtContent);
    }

    static string ConvertHtmlToTxt(string htmlContent)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        StringBuilder txtContent = new StringBuilder();

        foreach (var table in doc.DocumentNode.SelectNodes("//table"))
        {
            foreach (var row in table.SelectNodes("tr"))
            {
                foreach (var cell in row.SelectNodes("th|td"))
                {
                    string cellText = cell.InnerText.Trim();
                    txtContent.Append(cellText + "\t"); // Добавляем табуляцию между ячейками
                }
                txtContent.AppendLine(); // Переходим на новую строку после каждой строки таблицы
            }
            txtContent.AppendLine(); // Добавляем пустую строку после каждой таблицы
        }

        return txtContent.ToString();
    }
}
