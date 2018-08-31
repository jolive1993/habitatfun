using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Sitecore.Commerce;
using Sitecore.Commerce.Entities.Customers;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Customers;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Utils;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Commerce.XA.Foundation.Connect.Providers;
using Assert = Sitecore.Diagnostics.Assert;

namespace Sitecore.HabitatHome.Foundation.Customers.Managers
{
    public class AccountManager : Commerce.XA.Foundation.Connect.Managers.AccountManager, Interfaces.IAccountManager
    {
        public AccountManager(IConnectServiceProvider connectServiceProvider, ICartManager cartManager, IStorefrontContext storefrontContext, IModelProvider modelProvider) : base(connectServiceProvider, cartManager, storefrontContext, modelProvider)
        {
        }
        public ManagerResponse<CreateUserResult, CommerceUser> RegisterUserCustomer(IStorefrontContext storefrontContext, string userName, string password,
            string email, string secondaryEmail)
        {
            Assert.ArgumentNotNull(storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNullOrEmpty(userName, nameof(userName));
            Assert.ArgumentNotNullOrEmpty(password, nameof(password));
            CreateUserResult createUserResult1;
            try
            {
                var createUserRequest = new CreateUserRequest(userName, password, email, storefrontContext.CurrentStorefront.ShopName);
                createUserRequest.Properties.Add("SecondEmail", secondaryEmail);
                createUserResult1 = CustomerServiceProvider.CreateUser(createUserRequest);
                //createUserResult1 = CustomerServiceProvider.CreateUser(new CreateUserRequest(userName, password, email, storefrontContext.CurrentStorefront.ShopName));
                if (!createUserResult1.Success)
                {
                    Helpers.LogSystemMessages(createUserResult1.SystemMessages, createUserResult1);
                }
                else if (createUserResult1.Success)
                {
                    if (createUserResult1.CommerceUser == null)
                    {
                        if (createUserResult1.SystemMessages.Count == 0)
                        {
                            createUserResult1.Success = false;
                            createUserResult1.SystemMessages.Add(new SystemMessage()
                            {
                                Message = storefrontContext.GetSystemMessage("User Already Exists")
                            });
                        }
                    }
                }
            }
            catch (MembershipCreateUserException ex)
            {
                CreateUserResult createUserResult2 = new CreateUserResult {Success = false};
                createUserResult1 = createUserResult2;
                createUserResult1.SystemMessages.Add(new SystemMessage()
                {
                    Message = ErrorCodeToString(storefrontContext, ex.StatusCode)
                });
            }
            catch (Exception ex)
            {
                CreateUserResult createUserResult2 = new CreateUserResult {Success = false};
                createUserResult1 = createUserResult2;
                createUserResult1.SystemMessages.Add(new SystemMessage()
                {
                    Message = storefrontContext.GetSystemMessage("Unknown Membership Provider Error")
                });
            }
            CreateUserResult serviceProviderResult = createUserResult1;
            return new ManagerResponse<CreateUserResult, CommerceUser>(serviceProviderResult, serviceProviderResult.CommerceUser);
        }
    }
}