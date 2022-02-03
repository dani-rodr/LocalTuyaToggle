using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LocalTuyaToggle
{
    public static class Helper
    {
        public static string Encrypt(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashmessage.Length; i++)
                {
                    builder.Append(hashmessage[i].ToString("x2"));
                }
                return builder.ToString().ToUpper();
            }
        }

        public static string TimeStamp { get; } = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds().ToString();
        public static Stream ToStream(this string value) => new MemoryStream((Encoding.UTF8).GetBytes(value ?? string.Empty));
    }
}
