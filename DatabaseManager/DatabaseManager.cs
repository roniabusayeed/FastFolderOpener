using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseManager
{
    public class DatabaseManager
    {
        private const int FIELD_COUNT = 3;
        private static string databaseFilePath;

        // Implement DatabaseManager as a Singleton.
        private static readonly DatabaseManager _instance = new DatabaseManager();
        private DatabaseManager() { }
        public static DatabaseManager GetDatabaseManager(string databaseFilePath)
        {
            DatabaseManager.databaseFilePath = databaseFilePath;
            return _instance;
        }

        /// <summary>
        /// Loads all records from a csv database file with three colums.
        /// </summary>
        /// <returns>Returns a dictionary or of records if successful.
        /// Otherwise, returns null</returns>
        public async Task<Dictionary<string, Dictionary<string, string>>> LoadAsync()
        {
            return await Task.Run(() => {
                var db = new Dictionary<string, Dictionary<string, string>>();
                try
                { 
                    // Open db file and iterate over all records.
                    using (TextReader reader = new StreamReader(databaseFilePath))
                    {

                        string line;
                        while ((line = reader.ReadLine()) != null && (line = line.Trim()) != string.Empty)
                        {
                            // Tokenize each record.
                            var tokens = line.Split(',').Select(field => field.Trim()).ToList();
                            if (tokens.Count != FIELD_COUNT || tokens.Any(field => field == string.Empty))
                                throw new FormatException();

                            // Remember fields.
                            string project = tokens[0];
                            string category = tokens[1];
                            string folderPath = tokens[2];

                            if (db.TryGetValue(project, out Dictionary<string, string> categories))
                            {
                                categories[category] = folderPath;
                            }
                            else
                            {
                                db.Add(project, new Dictionary<string, string>());
                                db[project][category] = folderPath;
                            }

                        }
                    }
                } catch { db = null; }

                return db;
            });
        }
    }
}
