/**
 * StringHelper.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Prahlad.Common
{
    public static class StringHelper
    {
        // RFC 4648 alphabet
        internal const string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        private static readonly string[] RomanMap =
        {
            "I","II","III","IV","V","VI","VII","VIII","IX","X",
            "XI","XII","XIII","XIV","XV","XVI","XVII","XVIII","XIX","XX",
            "XXI","XXII","XXIII","XXIV","XXV","XXVI"
        };

        internal static string LiteEncryptShift(string plainText, int shift = 32)
        {
            if (plainText == null) return null;
            StringBuilder sb = new StringBuilder();
            foreach (char c in plainText)
            {
                char shifted = (char)(c + shift);
                sb.Append(shifted);
            }
            return sb.ToString();
        }

        internal static string LiteDecryptShift(string cipherText, int shift = 32)
        {
            if (cipherText == null) return null;
            StringBuilder sb = new StringBuilder();
            foreach (char c in cipherText)
            {
                char shifted = (char)(c - shift);
                sb.Append(shifted);
            }
            return sb.ToString();
        }

        internal static string LiteEncryptRoman(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            StringBuilder sb = new StringBuilder();
            foreach (char c in plainText.ToUpper())
            {
                if (c >= 'A' && c <= 'Z')
                    sb.Append(RomanMap[c - 'A']).Append("-");
                else
                    sb.Append(c);
            }
            if (sb.Length > 0) sb.Length--; // remove last dash
            return sb.ToString();
        }

        internal static string LiteDecryptRoman(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;
            StringBuilder sb = new StringBuilder();
            string[] parts = cipherText.Split('-');
            foreach (var part in parts)
            {
                int idx = Array.IndexOf(RomanMap, part);
                if (idx >= 0)
                    sb.Append((char)('A' + idx));
                else
                    sb.Append(part);
            }
            return sb.ToString();
        }

        public static byte[] Base32Decode(string base32)
        {
            int buffer = 0;
            int bitsLeft = 0;
            List<byte> result = new List<byte>();

            foreach (char c in base32)
            {
                int val = Base32Chars.IndexOf(c);
                if (val < 0)
                    throw new ArgumentException($"Invalid Base32 character: {c}");

                buffer <<= 5;
                buffer |= val & 0x1F;
                bitsLeft += 5;

                if (bitsLeft >= 8)
                {
                    result.Add((byte)((buffer >> (bitsLeft - 8)) & 0xFF));
                    bitsLeft -= 8;
                }
            }

            return result.ToArray();
        }


        public static string Base32Encode(byte[] bytes)
        {
            StringBuilder result = new StringBuilder();
            int buffer = bytes[0];
            int next = 1;
            int bitsLeft = 8;

            while (bitsLeft > 0 || next < bytes.Length)
            {
                if (bitsLeft < 5)
                {
                    if (next < bytes.Length)
                    {
                        buffer <<= 8;
                        buffer |= bytes[next++] & 0xFF;
                        bitsLeft += 8;
                    }
                    else
                    {
                        int pad = 5 - bitsLeft;
                        buffer <<= pad;
                        bitsLeft += pad;
                    }
                }

                int index = (buffer >> (bitsLeft - 5)) & 0x1F;
                bitsLeft -= 5;
                result.Append(Base32Chars[index]);
            }

            return result.ToString();
        }


        internal static string Encrypt(string plainText, string password, string salt)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
                var key = new Rfc2898DeriveBytes(password, saltBytes, 10000);
                aes.Key = key.GetBytes(32);
                aes.IV = key.GetBytes(16);

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                        sw.Write(plainText);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        internal static string Decrypt(string cipherText, string password, string salt)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
                var key = new Rfc2898DeriveBytes(password, saltBytes, 10000);
                aes.Key = key.GetBytes(32);
                aes.IV = key.GetBytes(16);

                byte[] buffer = Convert.FromBase64String(cipherText);

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream(buffer))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
