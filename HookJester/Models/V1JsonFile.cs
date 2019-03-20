using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

namespace HookJester.Models
{
    public class V1JsonFile
    {
        public IHeaderDictionary Headers;
        public IDictionary<string, string> QueryPairs = new Dictionary<string, string>();
        public IList<string> QuerySingles = new List<string>();
        public string Body;
    }
}
