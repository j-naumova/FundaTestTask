namespace FundaTestTask.ConsoleClient.ConsoleSpecific
{
    public static class ConfigHelper
    {
        public static string GetProperty(string key)
        {
            string? value = System.Configuration.ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"AppSetting '{key}' is missing or empty.");
            }
            return value;
        }

        public static bool GetBoolProperty(string key)
        {
            string value = GetProperty(key);
            return bool.Parse(value);
        }
    }
}
