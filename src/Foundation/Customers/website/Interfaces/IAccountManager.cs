using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Commerce.Entities.Customers;
using Sitecore.Commerce.Services.Customers;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;

namespace Sitecore.HabitatHome.Foundation.Customers.Interfaces
{
    public interface IAccountManager : Commerce.XA.Foundation.Connect.Managers.IAccountManager
    {
        ManagerResponse<CreateUserResult, CommerceUser> RegisterUserCustomer(IStorefrontContext storefrontContext, string userName, string password, string email, string secondaryEmail);
    }
}