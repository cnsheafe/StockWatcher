using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings;

using Jose;

namespace StockWatcher.Model.Services
{
    public class JWTService
    {
        public string CreateToken(string username)
        {
            var payload = new Dictionary<string, object>()
            {
                {"sub", $"{username}"},
                {"iat", DateTime.Now}
            };
            // TODO: Add env variables
            RSA privateKey;

            byte[] cipher = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("cipher"));
            string passphrase = Environment.GetEnvironmentVariable("passphrase");
            using (var certificate = new X509Certificate2(cipher, passphrase))
            {
                privateKey = certificate.GetRSAPrivateKey();
            }

            return JWT.Encode(payload, privateKey, JwsAlgorithm.RS256);
        }
    }
}