namespace D365.SQL
{
    using System.Collections.Generic;

    public class D365Credential
    {
        public D365Credential(string instanceName, string instanceRegion, string username, string password)
        {
            InstanceName = instanceName;
            InstanceRegion = instanceRegion;
            Username = username;
            Password = password;
            Tags = new List<string>();
        }

        public string InstanceName { get; set; }

        public string InstanceRegion { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public List<string> Tags { get; set; }
    }
}
