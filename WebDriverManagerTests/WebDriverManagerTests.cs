using NUnit.Framework;
using System.IO;

namespace WebDriverManager.Tests
{
    [TestFixture()]
    public class WebDriverManagerTests
    {
        [Test]
        public void DownloadDefault()
        {
            var driverManger = new ChromeDriverManager();
            File.Delete(driverManger.FullPath);
            driverManger.SetUpDriver();
            Assert.True(File.Exists(driverManger.FullPath));
            File.Delete(driverManger.FullPath);
        }

        [Test]
        public void DownloadCustomPath()
        {
            var driverManger = new ChromeDriverManager();
            File.Delete(driverManger.FullPath);
            driverManger.SetUpDriver(System.AppDomain.CurrentDomain.BaseDirectory + "driver\\");
            Assert.True(File.Exists(driverManger.FullPath));
            File.Delete(driverManger.FullPath);
        }

        [Test]
        public void DownloadDriver()
        {
            var dir = System.AppDomain.CurrentDomain.BaseDirectory;
            dir += dir.EndsWith(Path.DirectorySeparatorChar.ToString()) ? string.Empty : Path.DirectorySeparatorChar.ToString();
            var driverManger = new ChromeDriverManager();
            File.Delete(driverManger.FullPath);

            string latest = driverManger.GetLatestReleaseVersion();
            driverManger.DownloadDriver(latest, dir);
            Assert.True(File.Exists(driverManger.FullPath));
            File.Delete(driverManger.FullPath);
        }

        [Test]
        public void GetLatest()
        {
            var driverManger = new ChromeDriverManager();
            string last = driverManger.GetLatestReleaseVersion();
            string driverLast = driverManger.GetDriverVersion(last);
            Assert.That(string.Equals(last, driverLast));
            File.Delete(driverManger.FullPath);
        }
    }
}