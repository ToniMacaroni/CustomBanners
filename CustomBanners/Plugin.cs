using System.Reflection;
using CustomBanners.Configuration;
using CustomBanners.Installers;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace CustomBanners
{

    [Plugin(RuntimeOptions.DynamicInit)]
    [NoEnableDisable]
    public class Plugin
    {

        public static string Name { get; private set; }

        [Init]
        public void Init(IPALogger logger, Config conf, Zenjector zenjector)
        {
            Name = Assembly.GetExecutingAssembly().GetName().Name;

            zenjector.UseLogger(logger);
            zenjector.Install<PluginAppInstaller>(Location.App, conf.Generated<PluginConfig>());
            zenjector.Install<PluginMenuInstaller>(Location.Menu);
        }
    }
}
