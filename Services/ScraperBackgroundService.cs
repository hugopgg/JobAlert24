using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using JobAlert.Models;
using JobAlert.Services;
using JobAlert.Logger;

namespace JobAlert.Services
{
    public class ScraperBackgroundService(ScraperService scraperService, List<JobSite> jobSites) : BackgroundService
    {

        private readonly ScraperService _scraperService = scraperService;
        private readonly List<JobSite> _jobSites = jobSites;
        private readonly FileConsoleLogger _logger = new();


        protected override async Task ExecuteAsync(CancellationToken t)
        {
            _logger.Info("JobAlert BackgroundService started.");

            while (!t.IsCancellationRequested)
            {
                try
                {
                    _logger.Info("Starting daily scraping...");
                    await _scraperService.DailyScraping(_jobSites);
                    _logger.Info("Daily scraping completed.");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error in daily scraping: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromHours(24), t);
                // await Task.Delay(TimeSpan.FromSeconds(30), t);   //test 30sec

            }

            _logger.Info("JobScraperBackgroundService stopped.");

        }


    }
}