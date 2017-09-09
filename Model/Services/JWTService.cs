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

            // byte[] cipher = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("cipher"));
            // string passphrase = Environment.GetEnvironmentVariable("passphrase");
            byte[] secret = new byte[]{146, 187, 63, 46, 207, 128, 188, 94, 86, 91, 7, 79, 17, 80, 253, 228, 119, 218, 148, 89, 39, 129, 137, 217, 38, 31, 130, 227, 191, 234, 58, 175};
            // string passphrase = "mypass";
            // byte[] privateKey;
            // using (var rsa = new RSACryptoServiceProvider(256))
            // {
            //     privateKey = rsa.Encrypt(cipher, true);

            // }

            return JWT.Encode(payload, secret, JwsAlgorithm.HS256);
        }
    }
}