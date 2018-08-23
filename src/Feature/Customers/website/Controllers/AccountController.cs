using System;
using Sitecore.Commerce.Entities.Customers;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Customers;
using Sitecore.Commerce.XA.Feature.Account.Models.JsonResults;
using Sitecore.Commerce.XA.Feature.Account.Repositories;
using Sitecore.Commerce.XA.Foundation.Common.Attributes;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Diagnostics;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;
using Sitecore.HabitatHome.Feature.Customers.Models;

namespace Sitecore.HabitatHome.Feature.Customers.Controllers
{
    public class AccountController : Commerce.XA.Feature.Account.Controllers.AccountController
    {
        public AccountController(ILoginRepository loginRepository, IRegistrationRepository registrationRepository, IForgotPasswordRepository forgotPasswordRepository, IChangePasswordRepository changePasswordRepository, IAccountManager accountManager, IStorefrontContext storefrontContext, IVisitorContext visitorContext, IModelProvider modelProvider, IContext sitecoreContext) : base(loginRepository, registrationRepository, forgotPasswordRepository, changePasswordRepository, accountManager, storefrontContext, visitorContext, modelProvider, sitecoreContext)
        {
        }
        [HttpPost]
        [ValidateHttpPostHandler]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult RegistrationCustom(RegistrationUserInputModelCustom inputModel)
        {
            try
            {
                Assert.ArgumentNotNull((object)inputModel, "RegistrationUserInputModel");
                RegistrationBaseJsonResult registrationBaseJsonResult = new RegistrationBaseJsonResult(this.StorefrontContext, this.SitecoreContext);
                this.ValidateModel((BaseJsonResult)registrationBaseJsonResult);
                if (registrationBaseJsonResult.HasErrors)
                    return this.Json((object)registrationBaseJsonResult, JsonRequestBehavior.AllowGet);
                string userId = this.VisitorContext.UserId;
                ManagerResponse<CreateUserResult, CommerceUser> managerResponse = this.AccountManager.RegisterUser(this.StorefrontContext, this.UpdateUserName(inputModel.UserName), inputModel.Password, inputModel.UserName);
                if (managerResponse.ServiceProviderResult.Success && managerResponse.Result != null)
                {
                    registrationBaseJsonResult.Initialize(managerResponse.Result);
                    this.AccountManager.Login(this.StorefrontContext, this.VisitorContext, managerResponse.Result.UserName, inputModel.Password, false);
                }
                else
                    registrationBaseJsonResult.SetErrors((ServiceProviderResult)managerResponse.ServiceProviderResult);
                return this.Json((object)registrationBaseJsonResult);
            }
            catch (Exception ex)
            {
                return this.Json((object)new BaseJsonResult(nameof(Registration), ex, this.StorefrontContext, this.SitecoreContext), JsonRequestBehavior.AllowGet);
            }
        }
        [AllowAnonymous]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        [StorefrontSessionState(SessionStateBehavior.ReadOnly)]
        public ActionResult RegistrationCustom()
        {
            return View(CurrentRenderingView, RegistrationRepository.GetRegistrationModel());
        }
    }
}