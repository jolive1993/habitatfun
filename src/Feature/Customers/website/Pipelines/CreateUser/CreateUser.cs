using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Customers;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Customers;
using System;
using System.Linq;
using Microsoft.OData.Client;
using Sitecore.Commerce;
using Sitecore.Commerce.Engine.Connect.Pipelines;
using Sitecore.Commerce.Plugin.Customers;
using Sitecore.Commerce.Core;
using System.Threading.Tasks;
using Assert = Sitecore.Diagnostics.Assert;
using Container = Sitecore.Commerce.Engine.Container;

namespace Sitecore.HabitatHome.Feature.Customers.Pipelines.CreateUser
{
    public class CreateUser : PipelineProcessor
    {
        public CreateUser(IEntityFactory entityFactory)
        {
            this.EntityFactory = entityFactory;
        }

        public IEntityFactory EntityFactory { get; }

        public override void Process(ServicePipelineArgs args)
        {
            ValidateArguments<CreateUserRequest, CreateUserResult>(args, out var request, out var result);
            Assert.IsNotNull(request.UserName, "request.UserName");
            Assert.IsNotNull(request.Password, "request.Password");
            Container container = GetContainer(request.Shop.Name, string.Empty, "", "", args.Request.CurrencyCode,
                new DateTime?());
            CommerceUser commerceUser1 = result.CommerceUser;
            if (commerceUser1 != null &&
                commerceUser1.UserName.Equals(request.UserName, StringComparison.OrdinalIgnoreCase))
            {
                string entityId = "Entity-Customer-" + request.UserName;
                ServiceProviderResult currentResult = new ServiceProviderResult();
                EntityView entityView = GetEntityView(container, entityId, string.Empty, "Details", string.Empty,
                    currentResult);
                if (currentResult.Success && !string.IsNullOrEmpty(entityView.EntityId))
                {
                    base.Process(args);
                    return;
                }
            }

            EntityView entityView1 =
                GetEntityView(container, string.Empty, string.Empty, "Details", "AddCustomer", result);
            if (!result.Success)
                return;
            entityView1.Properties.FirstOrDefault(p => p.Name.Equals("Domain")).Value = request.UserName.Split('\\')[0];
            entityView1.Properties.FirstOrDefault(p => p.Name.Equals("LoginName")).Value =
                request.UserName.Split('\\')[1];
            entityView1.Properties.FirstOrDefault(p => p.Name.Equals("AccountStatus")).Value = "ActiveAccount";
            if (!string.IsNullOrEmpty(request.Email))
                entityView1.Properties.FirstOrDefault(p => p.Name.Equals("Email")).Value = request.Email;
            CommerceCommand commerceCommand = DoAction(container, entityView1, result);
            if (commerceCommand != null &&
                commerceCommand.ResponseCode.Equals("ok", StringComparison.OrdinalIgnoreCase))
            {
                CommerceUser commerceUser2 = EntityFactory.Create<CommerceUser>("CommerceUser");
                commerceUser2.Email = request.Email;
                commerceUser2.UserName = request.UserName;
                commerceUser2.ExternalId = commerceCommand.Models.OfType<CustomerAdded>().FirstOrDefault()?.CustomerId;
                result.CommerceUser = commerceUser2;
                request.Properties.Add(new PropertyItem()
                {
                    Key = "UserId",
                    Value = result.CommerceUser.ExternalId
                });
                string entityId = "Entity-Customer-" + request.UserName;
                TestSave(container, entityId, request, result, commerceUser2.ExternalId);
            }

            base.Process(args);
        }

        internal static void ValidateArguments<TRequest, TResult>(ServicePipelineArgs args, out TRequest request,
            out TResult result) where TRequest : ServiceProviderRequest where TResult : ServiceProviderResult
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.Request, "args.Request");
            Assert.ArgumentNotNull(args.Request.RequestContext, "args.Request.RequestContext");
            Assert.ArgumentNotNull(args.Result, "args.Result");
            request = args.Request as TRequest;
            result = args.Result as TResult;
            Assert.IsNotNull(request,
                "The parameter args.Request was not of the expected type.  Expected {0}.  Actual {1}.",
                typeof(TRequest).Name, args.Request.GetType().Name);
            Assert.IsNotNull(result,
                "The parameter args.Result was not of the expected type.  Expected {0}.  Actual {1}.",
                typeof(TResult).Name, args.Result.GetType().Name);
        }

        private void TestSave(Container container, string entityId, CreateUserRequest request, CreateUserResult result, string externalID)
        {
            container.SaveChangesDefaultOptions = SaveChangesOptions.ReplaceOnUpdate;
            var entityView = GetEntityView(container, entityId, "Composer-3ef588483fd54918a9e1f2e4d7dd3761", "EditView", "EditView", result);
            var entityviewTest = GetEntityView(container, externalID, "Composer-3ef588483fd54918a9e1f2e4d7dd3761", "EditView", "EditView", result);
            entityView.Properties.FirstOrDefault(x => x.Name == "Secondary Email").Value = (string)request.Properties["SecondEmail"];
            entityView.EntityId = externalID;
            var resultCommand = DoAction(container, entityView, result);
            container.SaveChanges();
            var checkIfitWorked = GetEntityView(container, entityId, "", "master", "", result);
        }
    }
}