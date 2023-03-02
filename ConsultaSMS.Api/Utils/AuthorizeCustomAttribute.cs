using System;
using Microsoft.AspNetCore.Authorization;
using WebConsultaSMS.Models.Enums;

namespace WebConsultaSMS.Utils
{
    public class AuthorizeCustomAttribute : AuthorizeAttribute
    {
        public AuthorizeCustomAttribute(params string[] transactions)
        {
            Roles = String.Join(",", transactions);
        }
    }
}
