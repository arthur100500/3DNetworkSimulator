namespace GNSHandling
{
    public class GNSProjectConfig
    {
        public string Address;
        public string Password;
        public int Port;
        public string User;

        public static GNSProjectConfig LocalGNSProjectConfig()
        {
            return new GNSProjectConfig
            {
                Address = "127.0.0.1",
                Port = 3080,
                User = "admin",
                Password = "666"
            };
        }
    }
}