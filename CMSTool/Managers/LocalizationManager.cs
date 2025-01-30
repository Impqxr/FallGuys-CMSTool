using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FGCMSTool.Managers
{
    public class LocalizationManager
    {
        private static readonly Lazy<Dictionary<string, string>?> LazyLangEntries = new(() =>
        {
            try
            {
                //todo lang switching 
                using var resStr = Assembly.GetExecutingAssembly().GetManifestResourceStream(FindLang("en"));
                using var reader = new StreamReader(resStr!);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());
            }
            catch
            {
                return null;
            }
        });

        static string FindLang(string lang)
        {
            foreach (var name in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                if (name.Contains("Localization") && name.Contains(lang))
                    return name;
            }

            return string.Empty;
        }

        public static Dictionary<string, string>? LangEntries => LazyLangEntries.Value;
        const string linkDef = @"\{ref:(.*?)\}";
        const string NullList = "NULL: {0}";
        const string Missing = "MISSING: {0}";

        public static string LocalizedString(string? key, object[]? format = null)
        {
            if (LangEntries == null)
                return string.Format(NullList, key);

            LangEntries.TryGetValue(key!, out var value);

            if (value == null)
                return string.Format(Missing, key);

            string result = value;

            foreach (Match match in Regex.Matches(result, linkDef))
            {
                var refKey = match.Groups[1].Value;
                LangEntries.TryGetValue(refKey, out var value_2);

                if (value_2 != null)
                    result = result.Replace(match.Value, value_2);
                else
                    result = result.Replace(match.Value, string.Format(Missing, refKey));
            }


            if (format != null)
            {
                int waitingForFormat = Regex.Matches(result, @"\{\d+\}").Count;
                if (format.Length == waitingForFormat)
                    result = string.Format(result, format);
            }
            return result;
        }
    }
}
