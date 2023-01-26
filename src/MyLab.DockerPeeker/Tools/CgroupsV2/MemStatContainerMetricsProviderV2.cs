using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;
using MyLab.DockerPeeker.Tools.StatObjectModel;

namespace MyLab.DockerPeeker.Tools.CgroupsV2
{
    public class MemStatContainerMetricsProviderV2 : IContainerMetricsProvider
    {
        private readonly IFileContentProviderV2 _fileContentProvider;

        private static readonly string[] MemStatWhiteList = 
        {
            "anon",
            "file",
            "kernel_stack",
            "percpu",
            "sock",
            "shmem",
            "file_mapped",
            "file_dirty",
            "file_writeback",
            "anon_thp",
            "inactive_anon",
            "active_anon",
            "inactive_file",
            "active_file",
            "unevictable",
            "slab_reclaimable",
            "slab_unreclaimable",
            "slab"
        };
        
        private readonly IDictionary<string, ContainerMetricType> _statMetricTypeCache =
            new Dictionary<string, ContainerMetricType>();

        public MemStatContainerMetricsProviderV2(IFileContentProviderV2 fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            var memStatContent = await _fileContentProvider.ReadMemStat(containerLongId);
            var memStat = KeyValueStat.Parse(memStatContent);
            
            var mList = new List<ContainerMetric>();

            await AddSwapCurrentAsync(mList, containerLongId);
            await AddLimitsAsync(mList, containerLongId);
            AddCache(mList, memStat);
            AddRss(mList, memStat);
            AddAllStat(mList, memStat);

            return mList;
        }

        private void AddAllStat(List<ContainerMetric> mList, KeyValueStat memStat)
        {
            foreach (var itm in memStat)
            {
                if(!Array.Exists(MemStatWhiteList, s => s == itm.Key))
                    continue;

                if (!_statMetricTypeCache.TryGetValue(itm.Key, out var metricType))
                {
                    metricType = ContainerMetricType.MemStatMetricType.AddLabel("mem_type", itm.Key);
                    _statMetricTypeCache.Add(itm.Key, metricType);
                }

                mList.Add(new(itm.Value, metricType));
            }
        }

        private void AddRss(List<ContainerMetric> mList, KeyValueStat memStat)
        {
            var anon = memStat.GetRequired("anon");
            mList.Add(new(anon, ContainerMetricType.MemRssMetricType));
        }

        private void AddCache(List<ContainerMetric> mList, KeyValueStat memStat)
        {
            mList.Add(new (memStat.GetRequired("file"), ContainerMetricType.MemCacheMetricType));
        }

        private async Task AddLimitsAsync(List<ContainerMetric> mList, string containerLongId)
        {
            var memMaxContent = await _fileContentProvider.ReadMemMax(containerLongId);
            
            if (long.TryParse(memMaxContent, out long memMax))
            {
                mList.Add(new ContainerMetric(memMax, ContainerMetricType.MemLimitMetricType));

                var memSwapMaxContent = await _fileContentProvider.ReadSwapMax(containerLongId);
                if (long.TryParse(memSwapMaxContent, out long memSwapMax))
                {
                    mList.Add(new ContainerMetric(memSwapMax, ContainerMetricType.MemSwLimitMetricType));
                }
            }
        }
        private async Task AddSwapCurrentAsync(List<ContainerMetric> mList, string containerLongId)
        {
            var memSwapCurrentContent = await _fileContentProvider.ReadMemSwapCurrent(containerLongId);
            long memSwapCurrent = long.Parse(memSwapCurrentContent);

            mList.Add(new(memSwapCurrent, ContainerMetricType.MemSwapMetricType));
        }
    }
}
