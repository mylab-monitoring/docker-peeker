using MyLab.DockerPeeker.Tools;
using Xunit;

namespace UnitTests
{
    public class ContainerLabelsBehavior
    {
        [Fact]
        public void ShouldParse()
        {
            //Arrange
            const string testString =
                "e755bdecbf22dfe2f08c59b4ac784b69e90df69e944ba63b77983af4eb9417c6<id-separator>docker_label_com.microsoft.created-by=visual-studio docker_label_com.microsoft.visual-studio.project-name=MyLab.DockerPeeker docker_label_desktop.docker.io/binds/0/Source=C:\\Users\\ozzye\\vsdbg\\vs2017u5 docker_label_desktop.docker.io/binds/0/SourceKind=hostFile docker_label_desktop.docker.io/binds/0/Target=/remote_debugger docker_label_desktop.docker.io/binds/1/Source=C:\\Users\\ozzye\\Documents\\prog\\my\\docker-peeker\\src\\MyLab.DockerPeeker docker_label_desktop.docker.io/binds/1/SourceKind=hostFile docker_label_desktop.docker.io/binds/1/Target=/app docker_label_desktop.docker.io/binds/2/Source=C:\\Users\\ozzye\\Documents\\prog\\my\\docker-peeker\\src docker_label_desktop.docker.io/binds/2/SourceKind=hostFile docker_label_desktop.docker.io/binds/2/Target=/src/ docker_label_desktop.docker.io/binds/3/Source=C:\\Users\\ozzye\\.nuget\\packages\\ docker_label_desktop.docker.io/binds/3/SourceKind=hostFile docker_label_desktop.docker.io/binds/3/Target=/root/.nuget/fallbackpackages docker_label_desktop.docker.io/binds/4/Source=//var/run/docker.sock docker_label_desktop.docker.io/binds/4/SourceKind=hostFile docker_label_desktop.docker.io/binds/4/Target=/var/run/docker.sock";

            //Act
            var labels = ContainerLabels.Parse(testString);

            //Assert
            Assert.Equal(17, labels.Count);
            Assert.Equal("/var/run/docker.sock", labels["desktop.docker.io/binds/4/Target"]);
        }

        [Fact]
        public void ShouldParseLabelsWithJson()
        {
            //Arrange
            const string testString =
                "fe75546ebcdaa3c204e69d9ad095ccee1625bde62927ca4ea723411c40b9d04e<id-separator>docker_label_annotation.io.kubernetes.container.hash=a9166cd8 docker_label_annotation.io.kubernetes.container.ports=[{\"name\":\"dns\",\"containerPort\":53,\"protocol\":\"UDP\"},{\"name\":\"dns-tcp\",\"containerPort\":53,\"protocol\":\"TCP\"},{\"name\":\"metrics\",\"containerPort\":9153,\"protocol\":\"TCP\"}] docker_label_annotation.io.kubernetes.container.restartCount=20 docker_label_annotation.io.kubernetes.container.terminationMessagePath=/dev/termination-log docker_label_annotation.io.kubernetes.container.terminationMessagePolicy=File docker_label_annotation.io.kubernetes.pod.terminationGracePeriod=30 docker_label_io.kubernetes.container.logpath=/var/log/pods/kube-system_coredns-f9fd979d6-h88lr_19cf7564-3ab0-4d6a-8ed5-8b701600fbe8/coredns/20.log docker_label_io.kubernetes.container.name=coredns docker_label_io.kubernetes.docker.type=container docker_label_io.kubernetes.pod.name=coredns-f9fd979d6-h88lr docker_label_io.kubernetes.pod.namespace=kube-system docker_label_io.kubernetes.pod.uid=19cf7564-3ab0-4d6a-8ed5-8b701600fbe8 docker_label_io.kubernetes.sandbox.id=621b04429b8750b10289e5d6cdc423777331f4c02068ff0bea93b7599b65e5df";

            //Act
            var labels = ContainerLabels.Parse(testString);

            //Assert
            Assert.Equal(13, labels.Count);
            Assert.Equal("30", labels["annotation.io.kubernetes.pod.terminationGracePeriod"]);
        }
    }
}
