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
                "947fb46a483118f6f78f9c8572ed18f1f93ec938a80880b5f018eb6c572513bf<id-separator>docker_label_com.microsoft.created-by=visual-studio docker_label_com.microsoft.visual-studio.project-name=MyLab.DockerPeeker docker_label_desktop.docker.io/binds/0/Source=C:\\Users\\ozzye\\vsdbg\\vs2017u5 docker_label_desktop.docker.io/binds/0/SourceKind=hostFile docker_label_desktop.docker.io/binds/0/Target=/remote_debugger docker_label_desktop.docker.io/binds/1/Source=C:\\Users\\ozzye\\Documents\\prog\\my\\docker-peeker\\src\\MyLab.DockerPeeker docker_label_desktop.docker.io/binds/1/SourceKind=hostFile docker_label_desktop.docker.io/binds/1/Target=/app docker_label_desktop.docker.io/binds/2/Source=C:\\Users\\ozzye\\Documents\\prog\\my\\docker-peeker\\src docker_label_desktop.docker.io/binds/2/SourceKind=hostFile docker_label_desktop.docker.io/binds/2/Target=/src/ docker_label_desktop.docker.io/binds/3/Source=C:\\Users\\ozzye\\.nuget\\packages\\ docker_label_desktop.docker.io/binds/3/SourceKind=hostFile docker_label_desktop.docker.io/binds/3/Target=/root/.nuget/fallbackpackages docker_label_desktop.docker.io/binds/4/Source=//var/run/docker.sock docker_label_desktop.docker.io/binds/4/SourceKind=hostFile docker_label_desktop.docker.io/binds/4/Target=/var/run/docker.sock";

            //Act
            var labels = ContainerLabels.Parse(testString);

            //Assert
            Assert.Equal(17, labels.Count);
            Assert.Equal("MyLab.DockerPeeker", labels["com.microsoft.visual-studio.project-name"]);
        }

        [Fact]
        public void ShouldParseLabelsWithJson()
        {
            //Arrange
            const string testString =
                "io.kubernetes.pod.name=coredns-f9fd979d6-h88lr,annotation.io.kubernetes.container.ports=[{\"name\":\"dns\",\"containerPort\":53,\"protocol\":\"UDP\"},{\"name\":\"dns-tcp\",\"containerPort\":53,\"protocol\":\"TCP\"},{\"name\":\"metrics\",\"containerPort\":9153,\"protocol\":\"TCP\"}],annotation.io.kubernetes.container.restartCount=18,annotation.io.kubernetes.pod.terminationGracePeriod=30,io.kubernetes.container.name=coredns,io.kubernetes.docker.type=container,io.kubernetes.pod.namespace=kube-system,io.kubernetes.pod.uid=19cf7564-3ab0-4d6a-8ed5-8b701600fbe8,io.kubernetes.sandbox.id=2d430f34cf83a7a9fbc4fa63207d50a9b218d2134320fb15c03f99d1d39b89ab,annotation.io.kubernetes.container.hash=a9166cd8,annotation.io.kubernetes.container.terminationMessagePath=/dev/termination-log,annotation.io.kubernetes.container.terminationMessagePolicy=File,io.kubernetes.container.logpath=/var/log/pods/kube-system_coredns-f9fd979d6-h88lr_19cf7564-3ab0-4d6a-8ed5-8b701600fbe8/coredns/18.log";

            //Act
            var labels = ContainerLabels.Parse(testString);

            //Assert
            Assert.Equal(13, labels.Count);
            Assert.Equal("File", labels["annotation.io.kubernetes.container.terminationMessagePolicy"]);
        }
    }
}
