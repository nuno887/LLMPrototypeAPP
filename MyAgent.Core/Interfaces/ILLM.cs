using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAgent.Core.Interfaces;

public interface ILLM
{
    Task<string> ProcessQuestionAsync(string question);
}