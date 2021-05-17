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

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {

        public static string Name { get; private set; }

        [Init]
        public void Init(IPALogger logger, Config conf, Zenjector zenjector)
        {
            Name = Assembly.GetExecutingAssembly().GetName().Name;

            zenjector.OnApp<PluginAppInstaller>().WithParameters(conf.Generated<PluginConfig>(), logger);
            zenjector.OnMenu<PluginMenuInstaller>();
            zenjector.OnGame<PluginGameInstaller>();
        }

        [OnStart]
        public void OnApplicationStart()
        {
        }

        [OnExit]
        public void OnApplicationQuit()
        {
        }
    }
}
