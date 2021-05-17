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
    public class Plugin
    {

        public static string Name { get; private set; }

        [Init]
        public void Init(IPALogger logger, Config conf, Zenjector zenjector)
        {
            Name = Assembly.GetExecutingAssembly().GetName().Name;

            zenjector.OnApp<PluginAppInstaller>().WithParameters(conf.Generated<PluginConfig>(), logger);
            zenjector.OnMenu<PluginMenuInstaller>();
        }

        [OnEnable]
        public void OnEnable()
        {
        }

        [OnDisable]
        public void OnDisable()
        {
        }
    }
}
