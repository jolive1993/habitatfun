using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Entities.Customers;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Customers;
using Sitecore.Diagnostics;
using System;
using System.Linq;
using Sitecore.Commerce.Engine;
using Sitecore.Commerce.Engine.Connect.Pipelines;

namespace Sitecore.HabitatHome.Feature.Customers.Pipelines.UpdateUser
{
    public class UpdateUser : PipelineProcessor
    {
        public override void Process(ServicePipelineArgs args)
        {
            UpdateUserRequest request;
            UpdateUserResult result;
            ValidateArguments<UpdateUserRequest, UpdateUserResult>(args, out request, out result);
            Assert.IsNotNull((object)request.CommerceUser, "request.CommerceUser");
            Assert.IsNotNull((object)request.CommerceUser.ExternalId, "request.CommerceUser.ExternalId");
            Assert.IsNotNull((object)request.Shop, "request.Shop");
            Container container = this.GetContainer(request.Shop.Name, request.CommerceUser.ExternalId, "", "", args.Request.CurrencyCode, new DateTime?());
            string externalId = request.CommerceUser.ExternalId;
            EntityView entityView = this.GetEntityView(container, externalId, string.Empty, "Details", "EditCustomer", (ServiceProviderResult)result);
            if (!result.Success)
                return;
            this.TranslateCommerceUserToView(entityView, request.CommerceUser);
            CommerceCommand commerceCommand = this.DoAction(container, entityView, (ServiceProviderResult)result);
            if (commerceCommand != null && commerceCommand.ResponseCode.Equals("ok", StringComparison.OrdinalIgnoreCase))
                result.CommerceUser = request.CommerceUser;
            base.Process(args);
        }

        protected virtual void TranslateCommerceUserToView(EntityView view, CommerceUser commerceUser)
        {
            view.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("UserName"))).Value = commerceUser.UserName;
            view.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("Email"))).Value = commerceUser.Email;
            view.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("AccountStatus"))).Value = commerceUser.IsDisabled ? "InactiveAccount" : "ActiveAccount";
            view.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("FirstName"))).Value = commerceUser.FirstName;
            view.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("LastName"))).Value = commerceUser.LastName;
            string key = commerceUser.GetProperties().Keys.FirstOrDefault<string>((Func<string, bool>)(k => k.Equals("Phone", StringComparison.OrdinalIgnoreCase)));
            if (string.IsNullOrWhiteSpace(key))
                return;
            view.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("PhoneNumber"))).Value = commerceUser.GetPropertyValue(key).ToString();
        }
        internal static void ValidateArguments<TRequest, TResult>(ServicePipelineArgs args, out TRequest request, out TResult result) where TRequest : ServiceProviderRequest where TResult : ServiceProviderResult
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            Assert.ArgumentNotNull((object)args.Request, "args.Request");
            Assert.ArgumentNotNull((object)args.Request.RequestContext, "args.Request.RequestContext");
            Assert.ArgumentNotNull((object)args.Result, "args.Result");
            request = args.Request as TRequest;
            result = args.Result as TResult;
            Assert.IsNotNull((object)request, "The parameter args.Request was not of the expected type.  Expected {0}.  Actual {1}.", typeof(TRequest).Name, args.Request.GetType().Name);
            Assert.IsNotNull((object)result, "The parameter args.Result was not of the expected type.  Expected {0}.  Actual {1}.", typeof(TResult).Name, args.Result.GetType().Name);
        }
    }
}