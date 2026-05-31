namespace tourism_firm
{
    [TestClass]
    public class ValidatorTests
    {
        [TestMethod]
        public void IsLoginValid_ValidLogin_ReturnsTrue()
        {
            bool result = Validator.IsLoginValid("user_123", out string error);
            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void IsLoginValid_EmptyLogin_ReturnsFalse()
        {
            bool result = Validator.IsLoginValid("", out string error);
            Assert.IsFalse(result);
            Assert.AreEqual("Логин не может быть пустым", error);
        }

        [TestMethod]
        public void IsLoginValid_TooShort_ReturnsFalse()
        {
            bool result = Validator.IsLoginValid("ab", out string error);
            Assert.IsFalse(result);
            Assert.AreEqual("Логин должен содержать не менее 3 символов", error);
        }

        [TestMethod]
        public void IsLoginValid_InvalidChars_ReturnsFalse()
        {
            bool result = Validator.IsLoginValid("русский", out string error);
            Assert.IsFalse(result);
            Assert.AreEqual("Логин может содержать только латинские буквы, цифры и знак подчёркивания", error);
        }

        [TestMethod]
        public void IsPasswordValid_ValidPassword_ReturnsTrue()
        {
            bool result = Validator.IsPasswordValid("Pass123!", out string error);
            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void IsPasswordValid_Empty_ReturnsFalse()
        {
            bool result = Validator.IsPasswordValid("", out string error);
            Assert.IsFalse(result);
            Assert.AreEqual("Пароль не может быть пустым", error);
        }

        [TestMethod]
        public void IsEmailValid_ValidEmail_ReturnsTrue()
        {
            bool result = Validator.IsEmailValid("test@example.com", out string error);
            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void IsEmailValid_Invalid_NoAt_ReturnsFalse()
        {
            bool result = Validator.IsEmailValid("testexample.com", out string error);
            Assert.IsFalse(result);
            Assert.AreEqual("Введите корректный email", error);
        }

        [TestMethod]
        public void IsEmailValid_Empty_ReturnsTrue()
        {
            bool result = Validator.IsEmailValid("", out string error);
            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void IsPhoneValid_ValidPhone_ReturnsTrue()
        {
            bool result = Validator.IsPhoneValid("+79123456789", out string error);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsPhoneValid_Invalid_TooShort_ReturnsFalse()
        {
            bool result = Validator.IsPhoneValid("12345", out string error);
            Assert.IsFalse(result);
            Assert.AreEqual("Телефон должен содержать от 10 до 15 цифр", error);
        }

        [TestMethod]
        public void IsNameValid_ValidName_ReturnsTrue()
        {
            bool result = Validator.IsNameValid("Иванов", "Фамилия", out string error);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsNameValid_Empty_ReturnsFalse()
        {
            bool result = Validator.IsNameValid("", "Фамилия", out string error);
            Assert.IsFalse(result);
            Assert.AreEqual("Фамилия не может быть пустым", error);
        }

        [TestMethod]
        public void IsDatesValid_ValidDates_ReturnsTrue()
        {
            DateTime departure = DateTime.Today.AddDays(1);
            DateTime returnDate = departure.AddDays(7);
            bool result = Validator.IsDatesValid(departure, returnDate, out string error);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsDatesValid_DepartureInPast_ReturnsFalse()
        {
            DateTime departure = DateTime.Today.AddDays(-1);
            DateTime returnDate = DateTime.Today;
            bool result = Validator.IsDatesValid(departure, returnDate, out string error);
            Assert.IsFalse(result);
            Assert.AreEqual("Дата вылета не может быть в прошлом", error);
        }

        [TestMethod]
        public void IsPriceValid_Zero_ReturnsFalse()
        {
            bool result = Validator.IsPriceValid(0, out string error);
            Assert.IsFalse(result);
            Assert.AreEqual("Цена должна быть больше 0", error);
        }

        [TestMethod]
        public void IsSeatsValid_Negative_ReturnsFalse()
        {
            bool result = Validator.IsSeatsValid(-5, out string error);
            Assert.IsFalse(result);
            Assert.AreEqual("Количество мест должно быть больше 0", error);
        }
    }
}