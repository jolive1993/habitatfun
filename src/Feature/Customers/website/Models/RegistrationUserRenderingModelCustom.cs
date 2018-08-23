using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Sitecore.Commerce.XA.Feature.Account.Models;
using Sitecore.Commerce.XA.Foundation.Common.Attributes;

namespace Sitecore.HabitatHome.Feature.Customers.Models
{
    public class RegistrationUserRenderingModelCustom : RegistrationUserRenderingModel
    {
        [Required]
        [Display(Name = "Secondary Email Address")]
        [EmailAddressRegularExpression("Please enter a valid {0}.")]
        [StringLength(256, ErrorMessage = "The {0} should not exceed {1} characters.")]
        public string SecondaryEmail { get; set; }
    }
}