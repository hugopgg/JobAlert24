using JobAlert.Models;


namespace JobAlert.Observers
{
    public class ConsoleObserver : IObserver
    {
        public void Update(JobOffer jobOffer)
        {
            // Console.WriteLine($"Notification Console : New Offer: {jobOffer.Title} || JobSite : {jobOffer.JobSite} || Link: {jobOffer.Link}");
        }
    }
}
