using System;
using System.Text;

namespace ChatGptMod
{
    public class XorEncoder
    {
        public static string Encode(string input, string key)
        {
            var output = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (char)(input[i] ^ key[i % key.Length]);
            }
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(new string(output)));
        }

        public static string Decode(string encoded, string key)
        {
            var decodedBytes = Convert.FromBase64String(encoded);
            var input = Encoding.UTF8.GetString(decodedBytes);
            var output = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (char)(input[i] ^ key[i % key.Length]);
            }
            return new string(output);
        }
    }
}