namespace WebDriverManager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;

    public class ChromeDriverManager : WebDriverManager
    {
        public static readonly string ChromeDriverName = "chromedriver.exe";
        private static readonly string driverSiteUrl = "https://chromedriver.storage.googleapis.com";

        public ChromeDriverManager() : base(ChromeDriverName)
        {
        }

        public sealed override string GetBrowserVersion()
        {
            //get chrome version
            string version = null;
            try
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe");
                var chromePath = key.GetValue("").ToString();
                var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(chromePath);
                version = info.FileVersion;
            }
            catch (Exception e)
            {

            }

            return version;
        }

        public sealed override string GetLatestReleaseVersion()
        {
            return WebDriverUtility.ExecHttp($"{driverSiteUrl}/LATEST_RELEASE");
        }

        public override void DownloadDriver(string version, string destDir = null)
        {
            if (!string.IsNullOrWhiteSpace(destDir))
            {
                baseDir = WebDriverUtility.CheckLastSeparatorChar(destDir);
            }

            var tempZip = baseDir + @"ChromeDriverTemp.zip";
            var source = $"{driverSiteUrl}/{version}/chromedriver_win32.zip";
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }

            WebDriverUtility.DownloadFile(source, tempZip);
            System.IO.Compression.ZipFile.ExtractToDirectory(tempZip, baseDir);
            File.Delete(tempZip);
        }

        public override string GetDriverVersion(string version = null)
        {
            version = version ?? browserVersion;
            List<int> targetVerList = new List<int>();
            foreach (var item in version.Split('.'))
            {
                if (int.TryParse(item, out var number))
                {
                    targetVerList.Add(number);
                }
                else
                {
                    throw new ApplicationException($"chrome version: {version} format is incorrect");
                }
            }

            var xmlFile = WebDriverUtility.ExecHttp(driverSiteUrl);
            var doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            XmlNamespaceManager nameSpace = new XmlNamespaceManager(doc.NameTable);
            nameSpace.AddNamespace("abc", "http://doc.s3.amazonaws.com/2006-03-01");

            //Use the latest version < chromeVersion if cannot match.
            var diffList = new int[targetVerList.Count].ToList();
            for (int i = 0; i < diffList.Count; i++)
            {
                diffList[i] = 0;
            }

            var root = doc.DocumentElement;
            var keyList = root.SelectNodes("abc:Contents/abc:Key", nameSpace);

            foreach (XmlNode key in keyList)
            {
                // *.*.*.*/chromedriver_*.zip
                var textArr = key.InnerText.Split('/');
                if (textArr.Length < 2)
                {
                    continue;
                }

                if (textArr[1].StartsWith("chromedriver_win32"))
                {
                    // exactly match
                    if (version.Equals(textArr[0]))
                    {
                        return version;
                    }

                    var verArr = textArr[0].Split('.');
                    if (verArr.Length != targetVerList.Count)
                    {
                        continue;
                    }

                    if (LaterThanTarget(verArr, targetVerList))
                    {
                        continue;
                    }

                    for (int i = 0; i < verArr.Length; i++)
                    {
                        var v = int.Parse(verArr[i]);

                        //compare next if ==
                        if (v == diffList[i])
                        {
                            continue;
                        }

                        if (v > diffList[i])
                        {
                            diffList[i] = v;
                            for (int j = i + 1; j < verArr.Length; j++)
                            {
                                diffList[j] = int.Parse(verArr[j]);
                            }
                        }

                        break;
                    }
                }
            }

            return string.Join(".", diffList);
        }

        private static bool LaterThanTarget(string[] verArr, List<int> targetVerList)
        {
            for (int i = 0; i < verArr.Length; i++)
            {
                if (!int.TryParse(verArr[i], out var v))
                {
                    return false;
                }

                //compare next if ==
                if (v == targetVerList[i])
                {
                    continue;
                }

                if (v > targetVerList[i])
                {
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}
