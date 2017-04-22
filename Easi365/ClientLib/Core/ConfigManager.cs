using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace ClientLib.Core
{
    public class ConfigManager
    {
        private string _configPath;

        public ConfigManager()
        {
            _configPath = CoreManager.StartupPath;
        }

        public Easi365Settings Settings { get; set; }

        public void SaveSettings()
        {
            if (!Directory.Exists(_configPath))
                Directory.CreateDirectory(_configPath);

            using (FileStream fs = new FileStream(Path.Combine(_configPath,"config.xml"),FileMode.Create))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(Easi365Settings));
                formatter.Serialize(fs, CoreManager.ConfigManager.Settings);
            }
        }


        public Easi365Settings LoadSettings()
        {
            try
            {
                Easi365Settings settings;
                string configPath = Path.Combine(CoreManager.StartupPath, "config.xml");
                using (FileStream fs = new FileStream(configPath,FileMode.Open))
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(Easi365Settings));
                    settings = (Easi365Settings)formatter.Deserialize(fs);
                }

                if (settings != null)
                    CoreManager.ConfigManager.Settings = settings;
                else
                    throw new Exception("加载配置文件出错.");

            }
            catch
            {
                CoreManager.ConfigManager.Settings = new Easi365Settings();
                SaveSettings();
            }
            finally 
            {

            }

            return CoreManager.ConfigManager.Settings;
        }
    }
}
