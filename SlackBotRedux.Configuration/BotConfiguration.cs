namespace SlackBotRedux.Configuration
{
    public interface IBotConfiguration
    {
        string ApiToken { get; set; }
        string Name { get; set; }
    }

    public class BotConfiguration : IBotConfiguration
    {
        public string ApiToken { get; set; }
        public string Name { get; set; }
    }
}
