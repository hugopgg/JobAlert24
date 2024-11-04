namespace JobAlert.Models
{
    public class JobSite(string name, string url, string job)
    {
        public string Name { get; set; } = name;
        public string Url { get; set; } = url;
        public string Job { get; set; } = job;
    }
}