namespace Server_CS
{
    /// <summary>
    ///     Класс для хранения связки логин/пароль
    /// </summary>
    public class RegData
    {
        /// <summary>
        ///     Логин уникальный псевдоним пользователя
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Пароль - необходим для доступа
        /// </summary>
        public string Password { get; set; }
    }
}