using HtmlAgilityPack;
using JobAlert.Logger;
using JobAlert.Models;
using Newtonsoft.Json;

namespace JobAlert.Services
{
    public class ScraperService
    {
        // private static ScraperService? _instance;
        // private static readonly object _lock = new();
        private static readonly HttpClient client = new();
        private readonly Dictionary<string, XPath> _xpath;
        private readonly NotificationService _notificationService;
        private readonly FileConsoleLogger _logger = new();
        private List<JobOffer> _existingOffers = [];

        private const string OUTPUT_DIR = "Ressources/output";
        private const string BASE_OFFERS_PATH = OUTPUT_DIR + "/base_job_offers.json";
        private const string NEW_OFFERS_PATH = OUTPUT_DIR + "/new_job_offers.json";

        public ScraperService(Dictionary<string, XPath> xpath, NotificationService notificationService)
        {
            _xpath = xpath;
            _notificationService = notificationService;
            CreateOutputDir();
            CreateHeaders();
        }

        // public static ScraperService GetInstance(Dictionary<string, XPath> xpath, NotificationService notificationService)
        // {
        //     if (_instance == null)
        //     {
        //         lock (_lock)
        //         {
        //             _instance ??= new ScraperService(xpath, notificationService);
        //         }
        //     }
        //     return _instance;
        // }

        public async Task<List<JobOffer>> Scrape(string url, string siteName)
        {
            var jobOffers = new List<JobOffer>();

            try
            {
                if (!_xpath.TryGetValue(siteName, out XPath? xPathConfig))
                {
                    _logger.Warn($"No XPath configuration found for site: {siteName}");
                    return jobOffers;
                }

                var html = await GetHtml(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var jobNodes = doc.DocumentNode.SelectNodes(xPathConfig.JobNodeXPath);

                if (jobNodes != null)
                {
                    foreach (var node in jobNodes)
                    {
                        var titleNode = node.SelectSingleNode(xPathConfig.TitleXPath);
                        var urlNode = node.SelectSingleNode(xPathConfig.UrlXPath);

                        if (titleNode != null)
                        {
                            string jobTitle = titleNode.InnerText.Trim();
                            string jobUrl = urlNode != null ? urlNode.GetAttributeValue("href", string.Empty) : url;
                            var newOffer = new JobOffer(jobTitle, jobUrl, siteName);
                            jobOffers.Add(newOffer);
                        }
                    }
                }
                else
                {
                    _logger.Warn($"No nodes found for the given structure. Check the XPath expression for site: {siteName}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error while scraping {url}", ex);
            }

            return jobOffers;
        }

        public async Task FirstScraping(List<JobSite> jobSites)
        {
            var jobOffers = new List<JobOffer>();
            LoadExistingOffers();

            foreach (var site in jobSites)
            {
                var offersFromSite = await Scrape(site.Url, site.Name);
                jobOffers.AddRange(offersFromSite);
                _logger.Info($"Scraping completed for site: {site.Name}, offers found: {offersFromSite.Count}");
            }

            SaveOffers(jobOffers, BASE_OFFERS_PATH);
        }


        public async Task DailyScraping(List<JobSite> jobSites)
        {
            var newOfferList = new List<JobOffer>();
            LoadExistingOffers();

            foreach (var site in jobSites)
            {
                var offers = await Scrape(site.Url, site.Name);

                foreach (var offer in offers)
                {
                    if (!_existingOffers.Any(o => o.Title == offer.Title))
                    {
                        newOfferList.Add(offer);
                        AddNewOffer(offer);
                    }
                }
            }

            if (newOfferList.Count > 0)
            {
                SaveOffers(newOfferList, NEW_OFFERS_PATH, append: true);
                SaveOffers(newOfferList, BASE_OFFERS_PATH, append: true);

                foreach (var offer in newOfferList)
                {
                    _notificationService.Notify(offer);
                }
                _logger.Info($"{newOfferList.Count} new offers found and notified.");

            }
            else
            {
                _logger.Info("No new offers found during daily scraping.");
            }

            _notificationService.ExecuteNotifyAll();
        }



        private static async Task<string> GetHtml(string url)
        {
            return await client.GetStringAsync(url);
        }



        private void AddNewOffer(JobOffer newOffer)
        {
            _existingOffers.Add(newOffer);
        }

        private void LoadExistingOffers()
        {
            if (!File.Exists(BASE_OFFERS_PATH))
            {
                _existingOffers = [];
                return;
            }

            try
            {
                var json = File.ReadAllText(BASE_OFFERS_PATH);
                _existingOffers = JsonConvert.DeserializeObject<List<JobOffer>>(json) ?? [];
            }
            catch (Exception ex)
            {
                _logger.Error($"Error loading existing offers", ex);
                _existingOffers = [];
            }
        }

        private static void SaveOffers(List<JobOffer> offers, string filePath, bool append = false)
        {
            if (append && File.Exists(filePath))
            {
                var existingOffers = JsonConvert.DeserializeObject<List<JobOffer>>(File.ReadAllText(filePath)) ?? [];
                existingOffers.AddRange(offers);
                offers = existingOffers;
            }

            var json = JsonConvert.SerializeObject(offers, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        private static void CreateHeaders()
        {
            // client.DefaultRequestHeaders.Clear();
            // client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            // client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            // client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            // client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,lt;q=0.8,et;q=0.7,de;q=0.6");
        }

        private static void CreateOutputDir()
        {
            if (!Directory.Exists(OUTPUT_DIR))
            {
                Directory.CreateDirectory(OUTPUT_DIR);
            }
        }
    }
}
