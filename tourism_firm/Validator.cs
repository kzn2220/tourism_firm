using System;
using System.Text.RegularExpressions;

namespace tourism_firm
{
    public static class Validator
    {
        public static bool IsLoginValid(string login, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(login))
            {
                error = "Логин не может быть пустым";
                return false;
            }
            if (login.Length < 3)
            {
                error = "Логин должен содержать не менее 3 символов";
                return false;
            }
            if (!Regex.IsMatch(login, @"^[a-zA-Z0-9_]+$"))
            {
                error = "Логин может содержать только латинские буквы, цифры и знак подчёркивания";
                return false;
            }
            return true;
        }

        public static bool IsPasswordValid(string password, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(password))
            {
                error = "Пароль не может быть пустым";
                return false;
            }
            if (password.Length < 3)
            {
                error = "Пароль должен содержать не менее 3 символов";
                return false;
            }
            if (!Regex.IsMatch(password, @"^[a-zA-Z0-9!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]+$"))
            {
                error = "Пароль может содержать только латинские буквы, цифры и спецсимволы";
                return false;
            }
            return true;
        }

        public static bool IsPhoneValid(string phone, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(phone))
                return true;

            string cleanPhone = phone.Trim();
            if (cleanPhone.StartsWith("+")) cleanPhone = cleanPhone.Substring(1);
            if (!Regex.IsMatch(cleanPhone, @"^\d+$"))
            {
                error = "Телефон должен содержать только цифры (разрешён знак + в начале)";
                return false;
            }
            if (cleanPhone.Length < 10 || cleanPhone.Length > 15)
            {
                error = "Телефон должен содержать от 10 до 15 цифр";
                return false;
            }
            return true;
        }

        public static bool IsEmailValid(string email, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(email))
                return true;

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email.Trim(), pattern))
            {
                error = "Введите корректный email";
                return false;
            }
            return true;
        }

        public static bool IsNameValid(string name, string fieldName, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(name))
            {
                error = $"{fieldName} не может быть пустым";
                return false;
            }
            if (name.Length < 2)
            {
                error = $"{fieldName} должен содержать не менее 2 символов";
                return false;
            }
            if (!Regex.IsMatch(name, @"^[a-zA-Zа-яА-ЯёЁ\- ]+$"))
            {
                error = $"{fieldName} может содержать только буквы, дефис и пробел";
                return false;
            }
            return true;
        }

        public static bool IsDatesValid(DateTime departureDate, DateTime returnDate, out string error)
        {
            error = null;
            DateTime today = DateTime.Today;

            if (departureDate < today)
            {
                error = "Дата вылета не может быть в прошлом";
                return false;
            }
            if (returnDate < departureDate)
            {
                error = "Дата возврата не может быть раньше даты вылета";
                return false;
            }
            return true;
        }

        public static bool IsPriceValid(decimal price, out string error)
        {
            error = null;
            if (price <= 0)
            {
                error = "Цена должна быть больше 0";
                return false;
            }
            return true;
        }

        public static bool IsSeatsValid(int seats, out string error)
        {
            error = null;
            if (seats <= 0)
            {
                error = "Количество мест должно быть больше 0";
                return false;
            }
            if (seats > 1000)
            {
                error = "Количество мест не может превышать 1000";
                return false;
            }
            return true;
        }
    }
}