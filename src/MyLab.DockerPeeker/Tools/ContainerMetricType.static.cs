namespace MyLab.DockerPeeker.Tools
{
    public partial class ContainerMetricType
    {
        public static readonly ContainerMetricType CpuUserMetricType;
        public static readonly ContainerMetricType CpuSystemMetricType;

        public static readonly ContainerMetricType BlkReadMetricType;
        public static readonly ContainerMetricType BlkWriteMetricType;

        public static readonly ContainerMetricType MemSwapMetricType;
        public static readonly ContainerMetricType MemCacheMetricType;
        public static readonly ContainerMetricType MemRssMetricType;
        public static readonly ContainerMetricType MemLimitMetricType;
        public static readonly ContainerMetricType MemSwLimitMetricType;
        public static readonly ContainerMetricType NetReceiveMetricType;
        public static readonly ContainerMetricType NetTransmitMetricType;

        static ContainerMetricType()
        {
            var cpuMetricType = new ContainerMetricType
            {
                Name = "container_cpu_jiffies_total",
                Type = "counter"
            };

            CpuUserMetricType = cpuMetricType.AddLabel("mode", "user",
                "Time is the amount of time a process has direct control of the CPU, executing process code");
            CpuSystemMetricType = cpuMetricType.AddLabel("mode", "system",
                "Time is the time the kernel is executing system calls on behalf of the process");

            var blkMetricType = new ContainerMetricType
            {
                Name = "container_blk_bytes_total",
                Type = "counter",
            };

            BlkReadMetricType = blkMetricType.AddLabel("direction", "read", "Report total input bytes");
            BlkWriteMetricType = blkMetricType.AddLabel("direction", "write", "Report total output bytes");

            var memParameterMetricType = new ContainerMetricType
            {
                Name = "container_mem_bytes",
                Type = "gauge"
            };

            MemCacheMetricType = memParameterMetricType.AddLabel("type", "cache",
                "The amount of memory used by the processes of this control group that can be associated precisely with a block on a block device");
            MemRssMetricType = memParameterMetricType.AddLabel("type", "rss",
                "The amount of memory that doesn’t correspond to anything on disk: stacks, heaps, and anonymous memory maps");
            MemSwapMetricType = memParameterMetricType.AddLabel("type", "swap",
                "The amount of swap currently used by the processes in this cgroup");

            var memLimitMetricType = new ContainerMetricType
            {
                Name = "container_mem_limit_bytes",
                Type = "gauge"
            };

            MemLimitMetricType = memLimitMetricType.AddLabel("type", "ram",
                "Indicates the maximum amount of physical memory that can be used by the processes of this control group");
            MemSwLimitMetricType = memLimitMetricType.AddLabel("type", "ramswap",
                "Indicates the maximum amount of RAM+swap that can be used by the processes of this control group");

            var netReceiveMetricType = new ContainerMetricType
            {
                Name = "container_net_bytes_total",
                Type = "counter"
            };

            NetReceiveMetricType =
                netReceiveMetricType.AddLabel("direction", "receive", "Report total received bytes");
            NetTransmitMetricType =
                netReceiveMetricType.AddLabel("direction", "transmit", "Report total transmitted bytes");
        }
    }
}