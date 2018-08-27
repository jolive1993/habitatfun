using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Customers;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Plugin.Customers;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Customers;
using Sitecore.Diagnostics;
using System;
using System.ComponentModel;
using System.Linq;
using Sitecore.Commerce.Engine.Connect.Pipelines;

namespace Sitecore.HabitatHome.Feature.Customers.Pipelines.CreateUser
{
    public class CreateUser : PipelineProcessor
    {
        public CreateUser(IEntityFactory entityFactory)
        {
            this.EntityFactory = entityFactory;
        }

        public IEntityFactory EntityFactory { get; private set; }

        public override void Process(ServicePipelineArgs args)
        {
            CreateUserRequest request = (CreateUserRequest)args.Request;
            CreateUserResult result = (CreateUserResult) args.Result;
            Assert.IsNotNull(request.UserName, "request.UserName");
            Assert.IsNotNull(request.Password, "request.Password");
            Container container = this.GetContainer(request.Shop.Name, string.Empty, "", "", args.Request.CurrencyCode, new DateTime?());
            CommerceUser commerceUser1 = result.CommerceUser;
            if (commerceUser1 != null && commerceUser1.UserName.Equals(request.UserName, StringComparison.OrdinalIgnoreCase))
            {
                string entityId = "Entity-Customer-" + request.UserName;
                ServiceProviderResult currentResult = new ServiceProviderResult();
                var entityView = this.GetEntityView(container, entityId, string.Empty, "Details", string.Empty, currentResult);
                if (currentResult.Success && !string.IsNullOrEmpty(entityView.EntityId))
                {
                    base.Process(args);
                    return;
                }
            }
            var entityView1 = this.GetEntityView(container, string.Empty, string.Empty, "Details", "AddCustomer", (ServiceProviderResult)result);
            if (!result.Success)
                return;
            entityView1.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("Domain"))).Value = request.UserName.Split('\\')[0];
            entityView1.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("LoginName"))).Value = request.UserName.Split('\\')[1];
            entityView1.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("AccountStatus"))).Value = "ActiveAccount";
            if (!string.IsNullOrEmpty(request.Email))
                entityView1.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("Email"))).Value = request.Email;
            CommerceCommand commerceCommand = this.DoAction(container, entityView1, (ServiceProviderResult)result);
            if (commerceCommand != null && commerceCommand.ResponseCode.Equals("ok", StringComparison.OrdinalIgnoreCase))
            {
                CommerceUser commerceUser2 = this.EntityFactory.Create<CommerceUser>("CommerceUser");
                commerceUser2.Email = request.Email;
                commerceUser2.UserName = request.UserName;
                commerceUser2.ExternalId = commerceCommand.Models.OfType<CustomerAdded>().FirstOrDefault<CustomerAdded>().CustomerId;
                result.CommerceUser = commerceUser2;
                request.Properties.Add(new PropertyItem()
                {
                    Key = "UserId",
                    Value = (object)result.CommerceUser.ExternalId
                });
            }
            base.Process(args);
        }
    }
}