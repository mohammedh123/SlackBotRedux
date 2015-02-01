namespace SlackBotRedux.Core.Models.Slack
{
    public class UserChangeMessage
    {
        public EventType Type { get; set; }

        public User User { get; set; }
    }
}
