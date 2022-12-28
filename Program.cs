using HtmlAgilityPack;
using System.Diagnostics;
using System.Text.RegularExpressions;

Console.WriteLine("enter the path of the template to be sanitized and press enter");
var path = Console.ReadLine();

if (!Directory.Exists(path))
    return;

var files = Directory
    .EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
    .Where(s => ".html".Equals(Path.GetExtension(s).ToLowerInvariant()));

foreach (var file in files)
{
    string text = File.ReadAllText(file);
    text = Regex.Replace(text, "/inc/", "inc/", RegexOptions.IgnoreCase);
    text = Regex.Replace(text, "href=\"/\"", "href=\"index.html\"", RegexOptions.IgnoreCase);

    var htmlDoc = new HtmlDocument();
    htmlDoc.LoadHtml(text);

    if (file.EndsWith("index.html"))
    {
        var nodes = htmlDoc.DocumentNode
            .SelectNodes("//a")
            .Where(a => a.GetAttributeValue("href", string.Empty).StartsWith("/") && a.GetAttributeValue("href", string.Empty).EndsWith(".html"));

        foreach (var node in nodes)
        {
            var attributeValue = node.GetAttributeValue("href", string.Empty);
            attributeValue = attributeValue.Substring(1);
            node.SetAttributeValue("href", attributeValue);
        }
    }

    htmlDoc.Save(file);
    Console.WriteLine(file);
}

Console.WriteLine(path);
Console.WriteLine("ok, i'm done");

try
{
    // Open folder
    Process.Start("explorer.exe", path);
}
catch { }

Console.ReadLine();