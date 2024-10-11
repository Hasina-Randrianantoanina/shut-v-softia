using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHUT.Core.Data
{
    public class ConnectionStringOptions
    {
        public string? dbConnectShut { get; set; }
        public string? dbConnectShutArchive { get; set; }
    }
    
    // remis pour checker visu, a tester en enlevant
    public class DBSettings
    {
        public string? Server { get; set; }
        public string? Port { get; set; }
        public string? Database { get; set; }
        public string? UserId { get; set; }
        public string? Password { get; set; }

    }

    public class LDapSettings
    {
        public int LDapPort { get; set; }
        public string? LDapBaseDn { get; set; }
        public string? LDapHost { get; set; }
    }
    public class JWTSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string ExpirationMinutes { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
    public class InfluxSettings
    {
        public string? InfluxUrl { get; set; }
        public string? Token { get; set; }
        public string? Bucket { get; set; }
        public string? Org { get; set; }
    }
}
