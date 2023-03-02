using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace WebConsultaSMS.Utils
{
    public static class FunctionsHelper
    {
        public static string Decrypt(this string raw)
        {
            try
            {
                var splited = raw.Split('$');
                return Decrypt(
                    Convert.FromBase64String(splited[2]),
                    Convert.FromBase64String(splited[0]),
                    Convert.FromBase64String(splited[1])
                );
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return exp.Message;
            }
        }

        public static string Encryptor(this string raw)
        {
            try
            {
                using (Aes aes = Aes.Create("System.Security.Cryptography.AesManaged"))
                {
                    // Encrypt string
                    byte[] encrypted = Encrypt(raw, aes.Key, aes.IV);
                    return Convert.ToBase64String(aes.Key)
                        + "$"
                        + Convert.ToBase64String(aes.IV)
                        + "$"
                        + Convert.ToBase64String(encrypted);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return exp.Message;
            }
        }

        public static string DecodeXMLString(this string xmlEncode)
        {
            return xmlEncode
                .Replace("&lt;", "<")
                .Replace("&amp;", "&")
                .Replace("&gt;", ">")
                .Replace("&quot;", "\"")
                .Replace("&apos;", "'");
        }

        public static StringBuilder ToProperties<T>(this T request)
        {
            StringBuilder result = new StringBuilder();
            Type _type = request.GetType();

            IEnumerable<PropertyInfo> listaProperties = _type.GetRuntimeProperties();

            var properties = listaProperties.Select(
                p =>
                    new PropsName()
                    {
                        Name = Attribute.IsDefined(p, typeof(DescriptionAttribute))
                            ? (
                                Attribute.GetCustomAttribute(p, typeof(DescriptionAttribute))
                                as DescriptionAttribute
                            ).Description
                            : p.Name,
                        Value = p.GetValue(request),
                        Type = p.PropertyType.FullName
                    }
            );

            foreach (var prop in properties)
            {

                string value = prop.Value.ToString();
                if (prop.Type.Contains("decimal", StringComparison.InvariantCultureIgnoreCase))
                {
                    decimal.TryParse(prop.Value.ToString(), out decimal convertedValue);
                    value = convertedValue.ToString("C2");
                }
                result.Append($"{prop.Name} {value}");
                result.AppendLine();
            }

            return result;
        }

        private static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            using (Aes aes = Aes.Create("System.Security.Cryptography.AesManaged"))
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    // creamos una llave para encriptar y desencriptar de la data que envian
                    using (
                        CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)
                    )
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data
            return encrypted;
        }

        private static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create AesManaged
            using (Aes aes = Aes.Create("System.Security.Cryptography.AesManaged"))
            {
                // Create a decryptor
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }

        internal static DateTime ToDateTime(this string fecha)
        {
            return DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
