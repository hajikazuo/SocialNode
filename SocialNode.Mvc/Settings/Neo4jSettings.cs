﻿namespace SocialNode.Mvc.Settings
{
    public class Neo4jSettings
    {
        public string Uri { get; set; } = "";
        public string Database { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
