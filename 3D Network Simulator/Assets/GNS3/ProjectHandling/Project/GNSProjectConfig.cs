using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;

namespace GNSHandling
{
    public class GNSProjectConfig
    {
        public string Address;
        public int Port;
        public string User;
        public string Password;

        public static GNSProjectConfig LocalGNSProjectConfig()
        {
            return new GNSProjectConfig()
            {
                Address = "127.0.0.1",
                Port = 3080,
                User = "admin",
                Password = "666"
            };
        }
    }
}
