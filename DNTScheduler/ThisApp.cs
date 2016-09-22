using System.Net;

namespace DNTScheduler
{
    /// <summary>
    ///
    /// </summary>
    public static class ThisApp
    {
        /// <summary>
        ///
        /// </summary>
        public static string SiteRootUrl;


        /// <summary>
        /// DNTScheduler needs a ping service to keep it alive.
        /// </summary>
        public static void WakeUp()
        {
            if (string.IsNullOrWhiteSpace(SiteRootUrl))
            {
                return;
            }

            using (var client = new WebClient())
            {
                client.Credentials = CredentialCache.DefaultNetworkCredentials;
                client.Headers.Add("User-Agent", "DNTScheduler 1.0");
                client.DownloadData(SiteRootUrl);
            }
        }
    }
}