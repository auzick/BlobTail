using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace BlobTail
{
    public class UserData
    {
        public string DataFolderPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BlobTail");

        public string FilePath => Path.Combine(DataFolderPath, "UserData.json");

        private readonly UserSettings _settings;

        public UserData()
        {
            if (!Directory.Exists(DataFolderPath))
                Directory.CreateDirectory(DataFolderPath);

            if (File.Exists(FilePath))
            {
                _settings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(FilePath));
                if (_settings.ConnectionStrings == null)
                {
                    _settings.ConnectionStrings = new List<string>();
                    File.WriteAllText(FilePath, JsonConvert.SerializeObject(_settings));
                }
            }
            else
            {
                _settings = new UserSettings(){ConnectionStrings = new List<string>()};
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(_settings));
            }
        }

        public List<string> ConnectionStrings => _settings.ConnectionStrings;

        public void UpateConnectionStrings(List<string> strings)
        {
            _settings.ConnectionStrings = strings;
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(_settings));
        }

        //public void AddConnectionString(string connectionString)
        //{
        //    if (!_settings.ConnectionStrings.Contains(connectionString))
        //        _settings.ConnectionStrings.Add(connectionString);
        //}

        public class UserSettings
        {
            public List<string> ConnectionStrings;
        }
    }
}
