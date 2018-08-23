module.exports = function() {
    var sitesRoot = "C:\\inetpub\\wwwroot";
    var siteName = "SitecoreCommerce.sc";
    var webroot = "C:\\inetpub\\wwwroot";
    var instanceRoot = sitesRoot + "\\SitecoreCommerce.sc";
    var config = {
        sitecoreRoot: instanceRoot,
        instanceUrl: "https://sitecorecommerce.local/",
        websiteRoot: instanceRoot + "\\",
        sitecoreLibraries: instanceRoot + "\\bin",
        solutionName: "HabitatHome.Commerce",
        buildConfiguration: "Debug",
        buildToolsVersion: 15.0,
        buildMaxCpuCount: 1,
        buildVerbosity: "minimal",
        buildPlatform: "Any CPU",
        publishPlatform: "AnyCpu",
        runCleanBuilds: true
    };
    return config;
};