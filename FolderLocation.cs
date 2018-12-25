using System.Configuration;
using System.IO;
 


namespace DKSH.MailAgent.Constants
{
    public static class FolderLocation
    {
        private static void CreateDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static string GetDirectory(string folder)
        {
            var directory = ConfigurationManager.AppSettings.Get(folder);

            if (string.IsNullOrEmpty(directory))
            {
                directory = Path.Combine(Directory.GetCurrentDirectory(), folder);
            }

            CreateDirectory(directory);
            return directory;
        }

        // TODO : JT REFACTORING

        public static string MB51Job => GetDirectory(nameof(MB51Job));

        public static string GRGIJob => GetDirectory(nameof(GRGIJob));

        public static string SupplierJob => GetDirectory(nameof(SupplierJob));

    }
}
