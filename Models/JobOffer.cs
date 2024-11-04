

namespace JobAlert.Models
{
    public class JobOffer(string title, string link, string jobSite)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Title { get; set; } = title;
        public string Link { get; set; } = link;
        public string JobSite { get; set; } = jobSite;
    }
}
