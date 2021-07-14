namespace MyLab.DockerPeeker.Tools
{
    public class ContainerMetric
    {
        public double Value { get; }
        public ContainerMetricType Type { get; }

        public ContainerMetric(double value, ContainerMetricType type)
        {
            Value = value;
            Type = type;
        }
    }
}