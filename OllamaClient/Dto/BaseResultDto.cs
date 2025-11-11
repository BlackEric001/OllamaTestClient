using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OllamaClient.Dto
{
    internal class BaseResultDto
    {
        public bool IsValid { get; set; }
        public string? Result { get; set; }

        public BaseResultDto(bool isValid, string? result)
        {
            IsValid = isValid;
            Result = result;
        }
    }
}
