using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GreenBill.Helpers
{
    public class Validator
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < 8)
                return false;


            return true;
        }

        public static bool IsStringOnly(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            return text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            string pattern = @"^09\d{9}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }

        public static bool ShouldContainLetter(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            return text.Any(char.IsLetter);
        }
    }
}