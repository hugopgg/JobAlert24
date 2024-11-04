using JobAlert.Logger;
using JobAlert.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace JobAlert.Managers
{
    public class XPathManager()
    {

        private readonly FileConsoleLogger _logger = new();

        private const string RESOURCES_DIR = "Ressources";

        private const string _filepath = $"{RESOURCES_DIR}/xpaths.json";
        public Dictionary<string, XPath> LoadXpaths()
        {
            try
            {
                if (!File.Exists(_filepath))
                {
                    throw new FileNotFoundException("No file with XPath found.");
                }

                var json = File.ReadAllText(_filepath);
                var xpathData = JsonConvert.DeserializeObject<Dictionary<string, XPath>>(json);

                return xpathData ?? [];
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while loading XPath file: {ex.Message}", ex);
                return [];
            }
        }
    }
}
