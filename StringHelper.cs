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
        private const string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        // shift value can range from 0 to 65,535 (\u0000 to \uFFFF).
        public static string EncryptByteShift(string plainText, int shift = 32)
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

        public static string DecryptByteShift(string cipherText, int shift = 32)
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


        // shift can be between 0 and 25 (0 == no encryption)
        public static string EncryptRomanShift(string plaintext, int shift = 10)
        {
            StringBuilder ciphertext = new StringBuilder();
            foreach (char character in plaintext)
            {
                if (char.IsLetter(character))
                {
                    char baseChar = char.IsUpper(character) ? 'A' : 'a';
                    // Calculate the new position after shifting
                    int newPosition = (character - baseChar + shift) % 26;
                    // Handle negative results from modulo operator in C# for negative shifts
                    if (newPosition < 0)
                    {
                        newPosition += 26;
                    }
                    ciphertext.Append((char)(baseChar + newPosition));
                }
                else
                {
                    // Non-alphabetic characters are appended without modification
                    ciphertext.Append(character);
                }
            }
            return ciphertext.ToString();
        }

        public static string DecryptRomanShift(string ciphertext, int shift = 10)
        {
            return EncryptRomanShift(ciphertext, -shift);
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


        public static string EncryptAes(string plainText, string password, string salt)
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

        public static string DecryptAes(string cipherText, string password, string salt)
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
