namespace MyLab.DockerPeeker.Tools
{
    public class ContainerMetric
    {
        public long Value { get; }
        public ContainerMetricType Type { get; }

        public ContainerMetric(long value, ContainerMetricType type)
        {
            Value = value;
            Type = type;
        }
    }
}