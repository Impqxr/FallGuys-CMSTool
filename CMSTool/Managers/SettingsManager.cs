using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FGCMSTool.Managers
{
    public class SettingsManager
    {
        public static SettingsManager? Settings { get; set; }

        public enum DecryptStrat
        {
            Default,
            Formatting,
            Parts
        }

        public enum EncryptStrart
        {
            V1,
            V2,
        }


        public class SettingsJson
        {
            public string? XorKey;
            public DecryptStrat DecryptStrat;
            public EncryptStrart EncryptStrart;

            public SettingsJson Copy()
            {
                return new SettingsJson
                {
                    XorKey = XorKey,
                    DecryptStrat = DecryptStrat,
                    EncryptStrart = EncryptStrart
                };
            }
        }

        public SettingsJson? SavedSettings;
        public readonly SettingsJson DefaultSettings = new()
        {
            XorKey = "a#!sC0,.",
            DecryptStrat = DecryptStrat.Default,
            EncryptStrart = EncryptStrart.V2
        };

        string? ConfigPath;

        public void Load(string baseDir)
        {
#if RELEASE_WIN_X64 || DEBUG
            ConfigPath = Path.Combine(baseDir, "Assets", "config.json");
#else
            ConfigPath = Path.Combine(baseDir, "config.json");
#endif

            try
            {
                if (!File.Exists(ConfigPath))
                {
                    SavedSettings = DefaultSettings.Copy();
                }
                else
                {
                    SavedSettings = JsonConvert.DeserializeObject<SettingsJson>(File.ReadAllText(ConfigPath));
                }
            }
            catch
            {
                SavedSettings = DefaultSettings.Copy();
            }
        }

        public void Save() => File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(SavedSettings));
    }
}
