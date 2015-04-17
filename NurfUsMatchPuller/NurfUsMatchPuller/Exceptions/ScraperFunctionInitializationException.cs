using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NurfUsMatchPuller.Exceptions
{
    public class ScraperFunctionInitializationException : Exception
    {
        public ScraperFunctionInitializationException(String caller)
            : base(String.Format("{0} failed to initialize scraper function dictionary", caller))
        {

        }
    }
}
