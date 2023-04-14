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
                Address = "localhost",
                Port = 3080,
                User = "admin",
                Password = "666"
            };
        }
    }
}