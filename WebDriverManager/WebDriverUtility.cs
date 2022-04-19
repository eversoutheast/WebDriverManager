namespace WebDriverManager
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    public class WebDriverUtility
    {
        /// <summary>
        /// Http
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Response body</returns>
        public static string ExecHttp(string url)
        {
            HttpWebResponse hwResponse = null;
            try
            {
                var hwRequest = (HttpWebRequest)WebRequest.Create(url);
                hwRequest.Proxy = WebRequest.DefaultWebProxy;
                hwRequest.Credentials = CredentialCache.DefaultCredentials;
                hwRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
                hwRequest.Method = "GET";
                hwRequest.ContentType = "application/x-www-form-urlencoded";
                hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                using (StreamReader srReader = new StreamReader(
                    hwResponse.GetResponseStream() ?? throw new InvalidOperationException(), Encoding.ASCII))
                {
                    return srReader.ReadToEnd();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                hwResponse?.Close();
            }
        }

        /// <summary>
        /// Download File from url to dest
        /// Override dest if exist
        /// Could not create directory if not exist
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="dest">target directory</param>
        public static void DownloadFile(string url, string dest)
        {
            WebClient webClient = new WebClient();
            webClient.Proxy = WebRequest.DefaultWebProxy;
            webClient.Credentials = CredentialCache.DefaultCredentials;
            webClient.Proxy.Credentials = CredentialCache.DefaultCredentials;
            webClient.DownloadFile(url, dest);
        }

        /// <summary>
        /// And '\' if not
        /// </summary>
        /// <param name="path">Directory</param>
        /// <returns>End with '\'</returns>
        public static string CheckLastSeparatorChar(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            path += path.EndsWith(Path.DirectorySeparatorChar.ToString()) ? string.Empty : Path.DirectorySeparatorChar.ToString();
            return path;
        }
    }
}
