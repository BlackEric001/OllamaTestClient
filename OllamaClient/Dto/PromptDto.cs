using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OllamaClient.Dto
{
    internal class PromptDto
    {
        public PromptDto(
            string model,
            string prompt,
            bool stream,
            string[]? images,
            string?format = null,
            OptionsDto? options = null
            )
        {
            Model = model;
            Prompt = prompt;
            Stream = stream;
            Images = images;
            Format = format;
            Options = options;
        }

        [JsonProperty("model")]
        public string Model {  get; }

        [JsonProperty("prompt")]
        public string Prompt { get; }

        [JsonProperty("stream")]
        public bool Stream { get; }

        [JsonProperty("images")]
        public string[]? Images { get; }

        [JsonProperty("format")]
        public string? Format { get; }

        [JsonProperty("options")]
        public OptionsDto? Options { get; }
    }
}
