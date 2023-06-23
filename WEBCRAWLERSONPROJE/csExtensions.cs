using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;


namespace WEBCRAWLERSONPROJE
{
    public static class csExtensions
    {
        public static string NormalizeURL(this string srURL)
        {
            return srURL.Split('#').FirstOrDefault().ToLower(new System.Globalization.CultureInfo("en-us"));
        }
       public static string HashURL(this string srURL)
        {
            return srURL.NormalizeURL().ComputeSha256Hash();
        }
        public static string ComputeSha256Hash(this string rawString)
        {

           // Using Statement(2022110830)

            using (SHA256 sha256Hash = SHA256.Create())
            {

                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawString));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string GetRootURL(this string srURL)
        {
            Uri myUri = new Uri(srURL);
            return myUri.Host;
        }
    }
}