namespace MyLab.DockerPeeker
{
    public class DockerPeekerOptions
    {
        public string Socket { get; set; } = "unix:///var/run/docker.sock";

        public bool DisableServiceContainerLabels { get; set; } = true;

        public string[] ServiceLabelsWhiteList { get; set; }
    }
}
