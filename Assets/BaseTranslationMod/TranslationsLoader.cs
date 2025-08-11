using System.Collections.Generic;
using UnityEngine;

namespace BaseTranslationMod
{
    public static class TranslationsLoader
    {

        public static Dictionary<string, string> LoadCsvToDictionary(TextAsset csvAsset)
        {
            CSVReader reader = new CSVReader();
            Dictionary<string, string> distilledData = new Dictionary<string, string>();

            List<List<string>> table = reader.ParseCSV(csvAsset.text);
            foreach (var line in table)
            {
                if (line.Count < 4) continue;
                
                distilledData.Add(line[0], line[3]);
            }

            return distilledData;
        }
    }
}