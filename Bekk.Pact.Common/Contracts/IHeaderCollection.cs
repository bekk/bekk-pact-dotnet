using System.Collections.Generic;

namespace Bekk.Pact.Common.Contracts
{
    public interface IHeaderCollection: IEnumerable<KeyValuePair<string, string>> 
    {
         string this [string key] {get; set;}
         IHeaderCollection Add(string key, params string[] values);
         IHeaderCollection ParseAndAdd(string header);
    }
}