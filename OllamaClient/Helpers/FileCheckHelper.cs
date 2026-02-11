using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OllamaClient.Helpers
{
    internal class FileCheckHelper
    {
        public FileCheckHelper(bool checkResult, string? checkMessage)
        {
            CheckResult = checkResult;
            CheckMessage = checkMessage;
        }

        public  bool CheckResult { get; }
        public string? CheckMessage {  get; }
    }
}
