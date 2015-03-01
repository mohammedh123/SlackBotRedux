using System.Configuration;
using Autofac;
using SlackBotRedux.Configuration;

namespace SlackBotRedux.AutofacModules
{
    public class BotConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var settings = new BotConfiguration
                {
                    ApiToken = ConfigurationManager.AppSettings["Bot.ApiToken"],
                    Name = ConfigurationManager.AppSettings["Bot.Name"]
                };

                return settings;
            }).SingleInstance();
        }
    }
}
