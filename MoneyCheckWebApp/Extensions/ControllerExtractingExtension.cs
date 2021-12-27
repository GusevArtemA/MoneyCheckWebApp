using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.Extensions
{
    public static class ControllerExtractingExtension
    {
        /// <summary>
        /// Получает пользователя, авторизованного из ПО промежуточного слоя
        /// </summary>
        /// <returns>Авторизованный пользователь</returns>
        /// <exception cref="InvalidOperationException">Если пользователь не найден в <see cref="HttpContext"/></exception>
        public static User ExtractUser(this ControllerBase controllerBase)
        {
            const string userKey = "ContextUser";
            var context = controllerBase.HttpContext.Items;

            if (context[userKey] is not User)
            {
                throw new InvalidOperationException("Extracting user from HTTP context failed");
            }

            return context[userKey] as User;
        }
    }
}