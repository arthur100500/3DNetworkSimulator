namespace GNS3.ProjectHandling.Project
{
    public class GnsProjectConfig
    {
        public string Address;
        public string Password;
        public int Port;
        public string User;

        public static GnsProjectConfig LocalGnsProjectConfig()
        {
            return new GnsProjectConfig
            {
                Address = "127.0.0.1",
                Port = 3080,
                User = "admin",
                Password = "666"
            };
        }
        
        public static GnsProjectConfig ProxyGnsProjectConfig()
        {
            return new GnsProjectConfig
            {
                Address = "127.0.0.1",
                Port = 9364,
                User = "admin",
                Password = "666"
            };
        }
    }
}