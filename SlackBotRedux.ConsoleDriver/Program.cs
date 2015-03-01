using Autofac;
using SlackBotRedux.AutofacModules;
using SlackBotRedux.Core;

namespace SlackBotRedux.ConsoleDriver
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var containerBuilder = GetContainerBuilder();
            var container = containerBuilder.Build();

            using (var scope = container.BeginLifetimeScope()) {
                var slackClient = scope.Resolve<ISlackClient>();
                slackClient.Start();
            }
        }

        private static ContainerBuilder GetContainerBuilder()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<SlackModule>();
            containerBuilder.RegisterModule<BotConfigurationModule>();

            return containerBuilder;
        }
    }
}
