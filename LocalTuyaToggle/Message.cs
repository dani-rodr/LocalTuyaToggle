using System;
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
}
