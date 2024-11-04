using JobAlert.Models;
using JobAlert.Services;
using JobAlert.Observers;
using JobAlert.Managers;
using JobAlert.Logger;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace JobAlert
{
    class Program
    {


        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // deps injection

                services.AddSingleton<FileConsoleLogger>();
                services.AddSingleton<IObserver, EmailObserver>();
                services.AddSingleton<JobSiteManager>();
                services.AddSingleton<XPathManager>();
                services.AddSingleton<NotificationService>(provider =>
                {
                    var notificationService = new NotificationService();
                    var emailObserver = provider.GetRequiredService<IObserver>();
                    notificationService.Subscribe(emailObserver);
                    return notificationService;
                });

                services.AddTransient<ScraperService>(provider =>
                {
                    var notificationService = provider.GetRequiredService<NotificationService>();
                    var xPathManager = provider.GetRequiredService<XPathManager>();
                    var xpaths = xPathManager.LoadXpaths();
                    return new ScraperService(xpaths, notificationService);

                });

                services.AddHostedService<ScraperBackgroundService>(provider =>
                {
                    var scraperService = provider.GetRequiredService<ScraperService>();
                    var jobSiteManager = provider.GetRequiredService<JobSiteManager>();
                    var jobSites = jobSiteManager.LoadJobSites();
                    return new ScraperBackgroundService(scraperService, jobSites);
                });



            }).Build();

            // first scraping
            var jobSitesManager = host.Services.GetRequiredService<JobSiteManager>();
            var jobSites = jobSitesManager.LoadJobSites();
            var scraperService = host.Services.GetRequiredService<ScraperService>();
            await scraperService.FirstScraping(jobSites);

            await host.RunAsync();
        }
    }
}


