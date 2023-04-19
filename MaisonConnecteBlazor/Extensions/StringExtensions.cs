using System.Security.Cryptography;
using System.Text;

namespace MaisonConnecteBlazor.Extensions
{
    public static class StringExtensions
    {
        public static string Base64Encode(this string str)
        {
            byte[] textByte = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(textByte);
        }

        public static string Base64Decode(this string str)
        {
            byte[] textByte = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(textByte);
        }

        public static string ToSha1(this string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            SHA1 crypto = SHA1.Create();
            byte[] encodedBytes = crypto.ComputeHash(bytes);

            StringBuilder hashBuilder = new StringBuilder();

            foreach (byte b in encodedBytes)
            {
                hashBuilder.Append(b.ToString("x2"));
            }

            return hashBuilder.ToString();
        }

        public static string ToSha256(this string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            SHA256 crypto = SHA256.Create();
            byte[] encodedBytes = crypto.ComputeHash(bytes);

            StringBuilder hashBuilder = new StringBuilder();

            foreach (byte b in encodedBytes)
            {
                hashBuilder.Append(b.ToString("x2"));
            }

            return hashBuilder.ToString();
        }

        public static int IndexOfNth(this string str, char character, int occurence)
        {
            int occurenceCount = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == character)
                {
                    occurenceCount++;

                    if (occurenceCount == occurence)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public static int IndexOfNth(this string str, string character, int occurence)
        {
            return str.IndexOfNth(character[0], occurence);
        }
    }
}
