﻿
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.HabitatHome.Foundation.PluginEnhancements.Engine.EntityViews;

namespace Sitecore.HabitatHome.Foundation.PluginEnhancements.Engine
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Commerce.EntityViews;

    /// <summary>
    /// The configure sitecore class.  This allows a Plugin to wire up new Pipelines or to change existing ones.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services constructor.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            //Detects and registers any Pipeline blocks in the Plugi
            services.RegisterAllPipelineBlocks(assembly);

            //Manipulation of pipelines during startup
            services.Sitecore().Pipelines(config => config
             .ConfigurePipeline<IBizFxNavigationPipeline>(d =>
                 {
                     //d.Add<EnsureNavigationView>();
                 })
             .ConfigurePipeline<IGetEntityViewPipeline>(d =>
                 {
                     //d.Add<Dashboard>().Before<IFormatEntityViewPipeline>()
                     // .Add<FomAddDashboardEntity>().Before<IFormatEntityViewPipeline>();
                 })
             .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
                 {
                     //d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>();
                 })
              .ConfigurePipeline<IDoActionPipeline>(
                c =>
                {
                    c.Add<DoActionEnablePlugin>().After<ValidateEntityVersionBlock>()
                    .Add<DoActionDisablePlugin>().After<ValidateEntityVersionBlock>();
                })
            );

            //Detects and registers all commands in the Plugin
            services.RegisterAllCommands(assembly);
        }
    }
}