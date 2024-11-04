using System.Collections.Generic;
using JobAlert.Models;
using JobAlert.Observers;
using JobAlert.Logger;

namespace JobAlert.Services
{
    public class NotificationService : ISubject
    {

        private static NotificationService? _instance;
        private static readonly object _lock = new();
        private readonly List<IObserver> _observers = [];
        private readonly FileConsoleLogger _logger = new();


        public static NotificationService GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new NotificationService();
                }
            }
            return _instance;
        }
        public void Subscribe(IObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                _logger.Info($"Observer subscribed: {observer.GetType().Name}");
            }
        }

        public void Unsubscribe(IObserver observer)
        {
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
                _logger.Info($"Observer unsubscribed: {observer.GetType().Name}");
            }
        }

        public void Notify(JobOffer jobOffer)
        {
            foreach (var observer in _observers)
            {
                observer.Update(jobOffer);
                _logger.Info($"Notified observer: {observer.GetType().Name} for job offer: {jobOffer.Title}");
            }
        }

        public void ExecuteNotifyAll()
        {
            foreach (var observer in _observers)
            {
                if (observer is EmailObserver emailObserver)
                {
                    emailObserver.SendOffers();
                    _logger.Info($"Executed notification for EmailObserver.");
                }
            }
        }
    }
}
