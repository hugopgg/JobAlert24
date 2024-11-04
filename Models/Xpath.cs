namespace JobAlert.Models
{
    public class XPath(string jobNodeXPath, string titleXPath, string urlXPath)
    {
        public string JobNodeXPath { get; set; } = jobNodeXPath;
        public string TitleXPath { get; set; } = titleXPath;
        public string UrlXPath { get; set; } = urlXPath;
    }
}
