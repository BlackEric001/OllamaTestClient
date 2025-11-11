using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OllamaClient.Dto
{

    public class Models
    {
        public Model[] models { get; set; }
    }

    public class Model
    {
        public string name { get; set; }
        public string model { get; set; }
        public DateTime modified_at { get; set; }
        public long size { get; set; }
        public string digest { get; set; }
        public Details details { get; set; }
    }

    public class Details
    {
        public string parent_model { get; set; }
        public string format { get; set; }
        public string family { get; set; }
        public object families { get; set; }
        public string parameter_size { get; set; }
        public string quantization_level { get; set; }
    }

}
