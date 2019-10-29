using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    public class CLAuthDto
    {
        public IDictionary<string, string> SasTokens { get; set; }

        public CdnToken CdnToken { get; set; }
    }
}