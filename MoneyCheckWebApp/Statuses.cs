namespace MoneyCheckWebApp
{
    public static class Statuses
    {
        /// <summary>
        /// Означает, что пользователь уже зарегистрирован
        /// </summary>
        public const string UserAlreadyCreatedStatus = "REG_ERR>USER_WITH_FOLLOWING_USERNAME_ALREADY_CREATED";

        /// <summary>
        /// Статус, когда HTTPS необходим 
        /// </summary>
        public const string HttpsRequiredStatus = "PROTOCOL_ERR>HTTPS_PROTO_REQUIRED";

        public const string UnknownFilterStatus = "FILTER_ERR>UNKNOWN_FILTER";
    }
}