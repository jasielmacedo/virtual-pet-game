using System.Security.Cryptography;
using System.Text;

namespace Core.save
{
    public class Crypt
    {
        public static string Encrypt(string text)
        {
            return Crypt.InOutConversor(text);
        }

        public static string Decrypt(string text)
        {
            return Crypt.InOutConversor(text);
        }

        private static int key = 122;

        private static string InOutConversor(string text)
        {
            StringBuilder inSb = new StringBuilder(text);
            StringBuilder outSb = new StringBuilder(text.Length);
            char c;
            for (int i = 0; i < text.Length; i++)
            {
                c = inSb[i];
                c = (char)(c ^ Crypt.key);
                outSb.Append(c);
            }
            return outSb.ToString();
        }

        public static string Md5Sum(string converting)
        {
            byte[] hash;

            using (MD5 md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.UTF8.GetBytes(converting));
            }

            return Encoding.UTF8.GetString(hash);
        }
    }
}
