using Jose;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Tokenizer;

namespace Fahasa.Models
{
    public class JwtService
    {
        public static byte[] key = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };
        public static string GenerateToken(Dictionary<string, object> payload)
        {
            return Jose.JWT.Encode(payload, key, JwsAlgorithm.HS512);
        }

        public static string Decode(string token)
        {
            return Jose.JWT.Decode(token, key);
        }
    }
}