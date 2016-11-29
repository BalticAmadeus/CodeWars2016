using System;
using System.Security.Cryptography;
using System.Text;

namespace Game.ClientCommon.Utilites
{
    public static class AuthCodeManager
    {
        public static string GetAuthCode(string rawData)
        {
            var sha1Managed = HashAlgorithm.Create("SHA1");
            if (sha1Managed == null)
                throw new ArgumentException("Specified hash algorithm type is not supported.");

            var encoding = Encoding.UTF8;
            var bytes = encoding.GetBytes(rawData);

            return BitConverter.ToString(sha1Managed.ComputeHash(bytes)).Replace("-", "").ToLower();
        }
    }
}