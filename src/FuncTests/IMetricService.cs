using System.Threading.Tasks;
using MyLab.ApiClient;

namespace FuncTests
{
    [Api]
    interface IMetricService
    {
        [Get("metrics")]
        Task<string> GetMetrics();
    }
}