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
        private Sitecore.HabitatHome.Foundation.Customers.Interfaces.IAccountManager CustomAccountManager { get; }
        public AccountController(ILoginRepository loginRepository, IRegistrationRepository registrationRepository, IForgotPasswordRepository forgotPasswordRepository, IChangePasswordRepository changePasswordRepository, Sitecore.HabitatHome.Foundation.Customers.Interfaces.IAccountManager accountManager, IStorefrontContext storefrontContext, IVisitorContext visitorContext, IModelProvider modelProvider, IContext sitecoreContext) : base(loginRepository, registrationRepository, forgotPasswordRepository, changePasswordRepository, accountManager, storefrontContext, visitorContext, modelProvider, sitecoreContext)
        {
            this.CustomAccountManager = accountManager;
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
                Assert.ArgumentNotNull(inputModel, "RegistrationUserInputModel");
                RegistrationBaseJsonResult registrationBaseJsonResult = new RegistrationBaseJsonResult(StorefrontContext, SitecoreContext);
                ValidateModel(registrationBaseJsonResult);
                if (registrationBaseJsonResult.HasErrors)
                {
                    return Json(registrationBaseJsonResult, JsonRequestBehavior.AllowGet);
                }

                ManagerResponse<CreateUserResult, CommerceUser> managerResponse = CustomAccountManager.RegisterUserCustomer(StorefrontContext, UpdateUserName(inputModel.UserName), inputModel.Password, inputModel.UserName, inputModel.SecondaryEmail);
                if (managerResponse.ServiceProviderResult.Success && managerResponse.Result != null)
                {
                    registrationBaseJsonResult.Initialize(managerResponse.Result);
                    AccountManager.Login(StorefrontContext, VisitorContext, managerResponse.Result.UserName, inputModel.Password, false);
                }
                else
                    registrationBaseJsonResult.SetErrors(managerResponse.ServiceProviderResult);
                return Json(registrationBaseJsonResult);
            }
            catch (Exception ex)
            {
                return Json(new BaseJsonResult(nameof(Registration), ex, StorefrontContext, SitecoreContext), JsonRequestBehavior.AllowGet);
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