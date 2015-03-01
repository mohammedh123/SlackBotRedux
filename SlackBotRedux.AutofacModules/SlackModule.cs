using Autofac;
using SlackBotRedux.Core;

namespace SlackBotRedux.AutofacModules
{
    public class SlackModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SlackClient>().SingleInstance().AsImplementedInterfaces();
        }
    }
}
