using JobAlert.Models;

namespace JobAlert.Observers
{
    public interface ISubject
    {
        void Subscribe(IObserver observer);
        void Unsubscribe(IObserver observer);
        void Notify(JobOffer jobOffer);
    }
}