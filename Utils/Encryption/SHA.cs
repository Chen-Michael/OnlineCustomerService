using System;
using System.Security.Cryptography;
using System.Text;

namespace Utils.Encryption
{
    public class SHA
    {
        public static string SHA1(String data)
        {
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            Byte[] temp1 = Encoding.UTF8.GetBytes(data);
            Byte[] temp2 = sha.ComputeHash(temp1);

            sha.Clear();

            String output = BitConverter.ToString(temp2).Replace("-", "").ToLower();

            return output;
        }
    }
}