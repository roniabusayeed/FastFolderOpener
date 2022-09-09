using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Threading.Tasks;

namespace FastFolderOpener
{
    internal class Config
    {
        private Dictionary<string, string> _config;
        private Config() 
        {
            load();
        }
        private static Config _instance = new Config();
        public static Config GetInstance() { return _instance; }
        public void Add(string key, string value) 
        { 
            _config[key] = value;
            save();
        }
        public async Task AddAsync(string key, string value)
        {
            await Task.Run(() => Add(key, value));
        }

        /// <summary>
        /// Retrieves a configuration value for a corresponding configuration key.
        /// </summary>
        /// <param name="key">A string representing the configuration key.</param>
        /// <returns>Configuration value as a string. Or null if configuraiton
        /// key is not found.</returns>
        public string Get(string key)
        {
            return _config.TryGetValue(key, out string value) ? value : null;
        }

        public async Task<string> GetAsync(string key)
        {
            return await Task.Run(() => Get(key));
        }

        private string configurationFilePath()
        {
            string documentsDirectoryPath = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments);
            string applicationDirectory = Path.Combine(documentsDirectoryPath,
                Constants.APPLICATION_TITLE);
            Directory.CreateDirectory(applicationDirectory);
            string configurationFilePath = Path.Combine(applicationDirectory,
                Constants.APPLICATION_CONFIG_FILENAME);
            return configurationFilePath;
        }
        private void load()
        {
            try
            {
                using TextReader reader = new StreamReader(configurationFilePath());
                string serializedConfig = reader.ReadToEnd();
                _config = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedConfig);
            } catch
            {
                _config = new Dictionary<string, string>();
            }
        }
        private void save()
        {
            try
            {
                using TextWriter writer = new StreamWriter(configurationFilePath());
                string serializedConfig = JsonConvert.SerializeObject(_config, Formatting.Indented);
                writer.Write(serializedConfig);
            } catch
            {
                MessageBox.Show(Constants.SAVING_CONFIGURATION_FAILED, Constants.APPLICATION_TITLE);
            }
        }
    }
}
