using System.Collections.Specialized;
using System.Configuration;

namespace Common
{
    public static class CommonMethods
    {
        public static string GetAppSetting(string key)
        {
            string result = "";

            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? "Not Found";

            }
            catch (ThesisException)
            {
                throw new ThesisException("Application settings not found.");
            }

            return result;
        }
    }
}
