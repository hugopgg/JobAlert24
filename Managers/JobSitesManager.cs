
using Newtonsoft.Json;
using JobAlert.Models;
using JobAlert.Logger;

namespace JobAlert.Managers
{
    public class JobSiteManager()
    {

        private readonly FileConsoleLogger _logger = new();
        private const string RESOURCES_DIR = "Ressources";
        private const string _filePath = $"{RESOURCES_DIR}/job_sites.json";


        public List<JobSite> LoadJobSites()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    _logger.Warn($"Job sites file not found at {_filePath}. Returning an empty list.");
                    return new List<JobSite>();
                }

                var json = File.ReadAllText(_filePath);
                _logger.Info("Job sites loaded successfully.");
                return JsonConvert.DeserializeObject<List<JobSite>>(json) ?? new List<JobSite>();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to load job sites.", ex);
                return [];
            }
        }

        public void SaveJobSite(JobSite site)
        {
            try
            {
                var jobSites = LoadJobSites();
                jobSites.Add(site);
                SaveJobSites(jobSites);
                _logger.Info($"Job site '{site.Name}' added successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to save the job site.", ex);
            }
        }

        public void SaveJobSites(List<JobSite> jobSites)
        {
            try
            {
                var json = JsonConvert.SerializeObject(jobSites, Formatting.Indented);
                File.WriteAllText(_filePath, json);
                _logger.Info("All job sites saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to save job sites.", ex);
            }
        }

        public void RemoveJobSite(string name)
        {
            try
            {
                var jobSites = LoadJobSites();
                int removedCount = jobSites.RemoveAll(site => site.Name == name);
                SaveJobSites(jobSites);

                if (removedCount > 0)
                    _logger.Info($"Job site '{name}' removed successfully.");
                else
                    _logger.Warn($"Job site '{name}' not found in the list.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to remove job site '{name}'.", ex);
            }
        }
    }
}
