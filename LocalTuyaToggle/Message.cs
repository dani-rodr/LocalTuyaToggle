using System.Collections.Generic;

namespace LocalTuyaToggle
{
    public class Result
    {
        public string access_token { get; set; }
        public int expire_time { get; set; }
        public string refresh_token { get; set; }
        public string uid { get; set; }
    }

    public class Response
    {
        public int code { get; set; }
        public string msg { get; set; }
        public Result result { get; set; }
        public bool success { get; set; }
        public long t { get; set; }
    }

    public class CommandResponse
    {
        public bool result { get; set; }
        public bool success { get; set; }
        public long t { get; set; }
    }

    public class KeyValuePair
    {
        public string code { get; set; }
        public object value { get; set; }
    }

    public class DeviceStatus
    {
        public List<KeyValuePair> result { get; set; }
        public bool success { get; set; }
        public long t { get; set; }
    }
}
