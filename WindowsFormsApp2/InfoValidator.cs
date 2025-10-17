using System.Text.RegularExpressions;

namespace WindowsFormsApp2
{
    internal class InfoValidator
    {
        private static readonly Regex NameRegex = new Regex(@"^[А-ЩЬЮЯҐЄІЇа-щьюяґєії'A-Za-z\s\-]{2,50}$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new Regex(@"^(\+?38)?0\d{9}$", RegexOptions.Compiled);

        public static bool ValidateName(string name, string fieldName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                errorMessage = $"Поле '{fieldName}' не може бути порожнім!";
                return false;
            }

            if (name.Length < 2)
            {
                errorMessage = $"'{fieldName}' повинно містити мінімум 2 символи!";
                return false;
            }

            if (name.Length > 50)
            {
                errorMessage = $"'{fieldName}' не може перевищувати 50 символів!";
                return false;
            }

            if (!NameRegex.IsMatch(name))
            {
                errorMessage = $"'{fieldName}' може містити тільки літери, пробіли, дефіси та апострофи!";
                return false;
            }

            if (Regex.IsMatch(name, @"[\s\-]{2,}"))
            {
                errorMessage = $"'{fieldName}' не може містити послідовні пробіли або дефіси!";
                return false;
            }

            return true;
        }

        public static bool ValidatePhone(string rawPhone, out string normalizedPhone, out string errorMessage)
        {
            normalizedPhone = null;
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(rawPhone))
            {
                errorMessage = "Поле 'Номер телефону' не може бути порожнім!";
                return false;
            }

            string cleanPhone = Regex.Replace(rawPhone, @"[^\d\+]", "");

            if (!PhoneRegex.IsMatch(cleanPhone))
            {
                errorMessage = "Введіть коректний номер телефону!\n\nПриклади:\n• 0501234567\n• +380501234567\n• 380501234567";
                return false;
            }

            string digitsOnly = Regex.Replace(cleanPhone, @"[^\d]", "");

            if (digitsOnly.Length == 10 && digitsOnly.StartsWith("0"))
            {
                normalizedPhone = $"+38{digitsOnly}";
            }
            else if (digitsOnly.Length == 12 && digitsOnly.StartsWith("380"))
            {
                normalizedPhone = $"+{digitsOnly}";
            }
            else
            {
                errorMessage = "Некоректний формат номера телефону!";
                return false;
            }

            string operatorCode = normalizedPhone.Substring(4, 2);
            if (!Regex.IsMatch(operatorCode, @"^(39|50|63|66|67|68|73|91|92|93|94|95|96|97|98|99)"))
            {
                errorMessage = "Некоректний код мобільного оператора! (Недійсний формат 0XX)";
                return false;
            }

            return true;
        }

        public static string FormatPhoneForDisplay(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "";

            string digitsOnly = Regex.Replace(phone, @"[^\d]", "");

            if (digitsOnly.Length == 12 && digitsOnly.StartsWith("380"))
            {
                return $"+{digitsOnly.Substring(0, 2)} ({digitsOnly.Substring(2, 3)}) {digitsOnly.Substring(5, 3)}-{digitsOnly.Substring(8, 2)}-{digitsOnly.Substring(10, 2)}";
            }
            else if (digitsOnly.Length == 10 && digitsOnly.StartsWith("0"))
            {
                return $"({digitsOnly.Substring(0, 3)}) {digitsOnly.Substring(3, 3)}-{digitsOnly.Substring(6, 2)}-{digitsOnly.Substring(8, 2)}";
            }

            return phone;
        }
    }
}
