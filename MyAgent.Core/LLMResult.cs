using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAgent.Core
{
    public class LLMResult
    {
        public string Notes { get; set; }
        public string Answer { get; set; }

        public LLMResult()
        {
            Notes = string.Empty;
            Answer = string.Empty;
        }
    }
}

