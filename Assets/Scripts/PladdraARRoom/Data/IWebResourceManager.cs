using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data
{
    public interface IWebResourceManager {
        Task<string> GetResourcePath (string url);
        Task<Dictionary<string, string>> GetResourcePaths(IEnumerable<string> urls);
    }
}