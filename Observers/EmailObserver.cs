using JobAlert.Logger;
using JobAlert.Models;
using JobAlert.Services;

namespace JobAlert.Observers
{
    public class EmailObserver() : IObserver
    {
        private readonly FileConsoleLogger _logger = new();
        private readonly EmailService _emailService = new();
        private readonly List<JobOffer> _newOffersToNotify = [];
        private const int NotificationThreshold = 5;

        public void Update(JobOffer offer)
        {
            _newOffersToNotify.Add(offer);
            _logger.Info($"New job offer added: {offer.Title} from {offer.JobSite}");
        }

        public void SendOffers()
        {
            int totalOffers = _newOffersToNotify.Count;

            if (totalOffers == 0)
            {
                _logger.Warn("No new offers to send.");
                return;
            }

            var snapshot = new List<JobOffer>(_newOffersToNotify);

            for (int i = 0; i < totalOffers; i += NotificationThreshold)
            {
                var batchSize = Math.Min(NotificationThreshold, totalOffers - i);
                var batch = snapshot.GetRange(i, batchSize);
                SendEmail(batch);
            }

            _newOffersToNotify.Clear();
            _logger.Info($"Sent {totalOffers} new job offers.");
        }

        private void SendEmail(List<JobOffer> offersBatch)
        {
            if (offersBatch.Count == 0)
            {
                _logger.Warn("Attempted to send an empty batch of offers.");
                return;
            }

            string subject = "New Job Offers Detected from JobAlert";
            string body = "Here are today's new job offers:<br/><ul>";

            foreach (var offer in offersBatch)
            {
                body += $"<li><a href=\"https://{offer.JobSite}.com/{offer.Link}\">{offer.Title}</a></li>";
            }

            body += "</ul>";

            try
            {
                _emailService.SendEmail(subject, body);
                _logger.Info($"Bulk email sent successfully for {offersBatch.Count} offers.");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to send bulk email.", ex);
            }
        }
    }
}
