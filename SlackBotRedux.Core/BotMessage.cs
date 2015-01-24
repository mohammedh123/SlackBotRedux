using System.Text.RegularExpressions;
using SlackBotRedux.Core.Models;

namespace SlackBotRedux.Core
{
    public abstract class BotMessage
    {
        public BotMessageType Type { get; set; }
        public bool IsFinishedBeingProcessed { get; set; }
        public Match Match { get; set; }

        protected BotMessage(BotMessageType type)
        {
            Type = type;
        }
    }

    public class UpdateTeamBotMessage : BotMessage
    {
        public ITeamState TeamState { get; set; }

        public UpdateTeamBotMessage(ITeamState teamState) : base(BotMessageType.UpdateTeam)
        {
            TeamState = teamState;
        }
    }

    public class UpdateUserBotMessage : BotMessage
    {
        public User User { get; set; }

        public UpdateUserBotMessage(User user) : base(BotMessageType.UpdateUser)
        {
            User = user;
        }
    }

    public class TextInputBotMessage : BotMessage
    {
        public InputMessage Message { get; set; }
        public User User { get; set; }

        public TextInputBotMessage(InputMessage msg, User user) : base(BotMessageType.TextInput)
        {
            Message = msg;
            User = user;
        }
    }

    public class FallbackMessage : BotMessage
    {
        public BotMessage OriginalMessage { get; private set; }

        public FallbackMessage(BotMessage originalMessage) : base(BotMessageType.Fallback)
        {
            OriginalMessage = originalMessage;
        }
    }

    public enum BotMessageType
    {
        UpdateTeam,
        UpdateUser,
        TextInput,
        Fallback
    }
}
