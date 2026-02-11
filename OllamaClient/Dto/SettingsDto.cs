using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OllamaClient.Dto
{
    public class SettingsDto
    {
        public string OllamaUrl { get; set; } = "http://localhost:11434/api";

        public int OllamaTimeout { get; set; } = 600;

        public string DefaultModel { get; set; } = string.Empty;

        public string DefaultPrompt { get; set; } = "Who are you?";

        public double Temperature { get; set; } = 0.0;
    }
}
