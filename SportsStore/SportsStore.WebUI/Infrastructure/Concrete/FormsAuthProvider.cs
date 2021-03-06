﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using SportsStore.WebUI.Infrastructure.Abstract;
using static System.Web.Security.FormsAuthentication;

namespace SportsStore.WebUI.Infrastructure.Concrete
{
    public class FormsAuthProvider : IAuthProvider
    {
        public bool Authenticate(string username, string password)
        {
            var result = FormsAuthentication.Authenticate(username, password);
            if (result)
            {
                SetAuthCookie(username, false);
            }

            return result;
        }
    }
}