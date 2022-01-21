namespace MoneyCheckWebApp
{
    public static class Statuses
    {
        public const string TokenAuthorizationFailedStatus = "AUTH_ERR:TOKEN_AUTH_FLAG>>UNUATHORIZED>TOKEN_AUTH_FAILED_POINT";

        public const string CookieAuthorizationFailedStatus = "AUHT:COOKIE_AUTH_FLAG>>UNAUTHORIZED>COOKIE_AUTH_FAILED_POINT";
        
        /// <summary>
        /// Означает, что пользователь уже зарегистрирован
        /// </summary>
        public const string UserAlreadyCreatedStatus = "REG_ERR>>USER_WITH_FOLLOWING_USERNAME_ALREADY_CREATED";

        /// <summary>
        /// Статус, когда HTTPS необходим 
        /// </summary>
        public const string HttpsRequiredStatus = "PROTOCOL_ERR>>HTTPS_PROTO_REQUIRED";

        public const string UnknownFilterStatus = "FILTER_ERR>>UNKNOWN_FILTER";

#region CATEGORY_ERR

        public const string NonAccessToContextCategoryStatus = "CATEGORY_ERR>>ACCESS_REFUSED";

        public const string CategoryNotFound = "CATEGORY_ERR>>CATEGORY_NOT_FOUND";

        public const string CategoryLogoFailedStatus = "CATEGORY_ERR:NOT_CLIENT_ERROR_FLAG>>FAILED_GET_CATEGORY_LOGO";

        public const string CategoryLogoRequiredStatus = "CATEGORY_ERR>>LOGO_REQUIRED";
        
#endregion

#region Companies

        public const string CompanyNotFoundStatus = "COMPANY_ERR>>COMPANY_NOT_FOUND";

#endregion
    }
}