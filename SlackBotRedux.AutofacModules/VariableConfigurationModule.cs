using System.Configuration;
using Autofac;
using SlackBotRedux.Configuration;

namespace SlackBotRedux.AutofacModules
{
    public class VariableConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var settings = new VariableConfiguration()
                {
                    AllowedNameCharactersRegex = ConfigurationManager.AppSettings["Variable.AllowedNameCharactersRegex"],
                    PrefixString = ConfigurationManager.AppSettings["Variable.PrefixString"]
                };

                return settings;
            }).SingleInstance();
        }
    }
}
