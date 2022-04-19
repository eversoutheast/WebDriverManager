namespace WebDriverManager
{
    using System;
    using System.IO;

    public class WebDriverManagerEventArgs : EventArgs
    {
    }

    public abstract class WebDriverManager
    {
        public delegate void Notify(WebDriverManager sender, WebDriverManagerEventArgs e);

        public event Notify SetUpDriverStart;

        public event Notify SetUpDriverComplete;

        protected readonly string driverName;
        protected string baseDir;
        protected string driverVersion;
        protected string browserVersion;

        public virtual string DriverName => driverName;

        public virtual string BaseDir => baseDir;

        public virtual string FullPath => baseDir + driverName;

        public WebDriverManager(string name)
        {
            baseDir = WebDriverUtility.CheckLastSeparatorChar(System.AppDomain.CurrentDomain.BaseDirectory);
            driverName = name;
        }

        public virtual void SetUpDriver(bool forceOverride)
        {
            SetUpDriver(null, forceOverride);
        }

        public virtual void SetUpDriver(string dir = null, bool forceOverride = false)
        {
            if (!string.IsNullOrWhiteSpace(dir))
            {
                baseDir = WebDriverUtility.CheckLastSeparatorChar(dir);
            }

            OnSetUpDriverStart();
            try
            {
                if (File.Exists(FullPath) && !forceOverride)
                {
                    return;
                }

                browserVersion = GetBrowserVersion();
                if (string.IsNullOrEmpty(browserVersion))
                {
                    browserVersion = GetLatestReleaseVersion();
                }

                driverVersion = GetDriverVersion();
                DownloadDriver(driverVersion);
            }
            catch (Exception e)
            {

            }
            finally
            {
                OnSetUpDriverComplete();
            }
        }

        public abstract string GetBrowserVersion();

        public abstract string GetLatestReleaseVersion();

        public abstract string GetDriverVersion(string version = null);

        public abstract void DownloadDriver(string version, string baseDir = null);

        protected virtual void OnSetUpDriverStart()
        {
            SetUpDriverStart?.Invoke(this, new WebDriverManagerEventArgs());
        }

        protected virtual void OnSetUpDriverComplete()
        {
            SetUpDriverComplete?.Invoke(this, new WebDriverManagerEventArgs());
        }
    }
}
