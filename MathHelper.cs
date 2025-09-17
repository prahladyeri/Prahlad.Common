/**
 * MathHelper.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System;

namespace Prahlad.Common
{
    static class MathHelper
    {
        private static Random rnd = new Random();

        private static string[] units = { "Zero", "One", "Two", "Three", "Four", "Five",
                                      "Six", "Seven", "Eight", "Nine", "Ten", "Eleven",
                                      "Twelve", "Thirteen", "Fourteen", "Fifteen",
                                      "Sixteen", "Seventeen", "Eighteen", "Nineteen" };

        private static string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty",
                                     "Sixty", "Seventy", "Eighty", "Ninety" };

        public static string ToIndianWords(int number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + ToIndianWords(Math.Abs(number));

            string words = "";

            if ((number / 10000000) > 0)
            {
                words += ToIndianWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }

            if ((number / 100000) > 0)
            {
                words += ToIndianWords(number / 100000) + " Lakh ";
                number %= 100000;
            }

            if ((number / 1000) > 0)
            {
                words += ToIndianWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += ToIndianWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                if (number < 20)
                    words += units[number];
                else
                {
                    words += tens[number / 10];
                    if ((number % 10) > 0)
                        words += " " + units[number % 10];
                }
            }

            return words.Trim();
        }

        /// <summary>
        /// Cast a dice with specific sides.
        /// There will be 1 in <sides> chance this will return true.
        /// </summary>
        public static bool Dice(int sides = 6)
        {
            if (rnd.Next(1, sides + 1) == 1)
            {
                return true;
            }
            return false;
        }
    }
}
