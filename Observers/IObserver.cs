using JobAlert.Models;

namespace JobAlert.Observers
{
    public interface IObserver
    {
        void Update(JobOffer jobOffer);
    }
}