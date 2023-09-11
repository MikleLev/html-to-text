﻿using HtmlAgilityPack;
using System;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        string htmlContent = @"
        <html>
            <head>
                <meta content='text/html; charset=unicode' http-equiv='Content-Type'>
            </head>
            <body>
                <h1>Main Header</h1>
                <p>This is a paragraph of text.</p>
                <ul>
                    <li>Bullet Point 1</li>
                    <li>Bullet Point 2</li>
                    <li>Bullet Point 3</li>
                </ul>
                <ol>
                    <li>Numbered Item 1</li>
                    <li>Numbered Item 2</li>
                    <li>Numbered Item 3</li>
                </ol>
                <p>Visit <a href='https://example.com'>Example</a> website.</p>
                <div>Highlighted Text</div>
                <p>1 тест На листе 3 вынесен узел А, откуда он? Вынести его или дать ссылку на него.</p>
                <div></div>
                <div>2 На листе 7 примечания выполнить над основной надписью (см. ГОСТ 2.316-2008). Обращаю Ваше внимание, что на всех листах примечания д.б. выполнены над основной надписью шириной 185 мм</div>
            </body>
        </html>";

        string txtContent = ConvertHtmlToTxt(htmlContent);
        Console.WriteLine(txtContent);
    }

    static string ConvertHtmlToTxt(string htmlContent)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        StringBuilder txtContent = new StringBuilder();

        ProcessNode(doc.DocumentNode, txtContent, false, null);

        return txtContent.ToString();
    }

    static void ProcessNode(HtmlNode node, StringBuilder txtContent, bool isList, string parentTag)
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
        else
        {
            int index = 1;
            foreach (var childNode in node.ChildNodes)
            {
                if (node.Name != "div")
                {
                    ProcessNode(childNode, txtContent, node.Name == "ul" || node.Name == "ol", node.Name);
                }
            }

            if (node.Name == "ul" && parentTag != "li")
            {
                foreach (var listItem in node.SelectNodes(".//li"))
                {
                    txtContent.Append("• ");
                    txtContent.AppendLine(listItem.InnerText.Trim());
                }
                txtContent.AppendLine();
            }
            else if (node.Name == "ol" && parentTag != "li")
            {
                foreach (var listItem in node.SelectNodes(".//li"))
                {
                    txtContent.AppendFormat("{0}. {1}", index, listItem.InnerText.Trim());
                    txtContent.AppendLine();
                    index++;
                }
                txtContent.AppendLine();
            }
        }
    }
}
