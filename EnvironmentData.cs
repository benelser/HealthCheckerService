using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HealthChecker
{
    class EnvironmentData
    {
        public int Heartbeat { get; set; }
        public string Destination { get; set; }
        public List<string> To { get; set; }

        public static EnvironmentData Read()
        {
            string cwd = Directory.GetCurrentDirectory();
            string EnvironmentDataFilePath = $"{cwd}\\data.json";
            using (StreamReader r = new StreamReader(EnvironmentDataFilePath))
            {
                string json = r.ReadToEnd();
                return JsonSerializer.Deserialize<EnvironmentData>(json);

            }
        }
    }

}