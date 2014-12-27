using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SlackBotRedux.Core.Models;

namespace SlackBotRedux.Core
{
    public abstract class BotMessage
    {
        public BotMessageType Type { get; private set; }

        public BotMessage(BotMessageType type)
        {
            Type = type;
        }
    }

    public class UpdateTeamBotMessage : BotMessage
    {
        public TeamState TeamState { get; set; }

        public UpdateTeamBotMessage(TeamState teamState) : base(BotMessageType.UpdateTeam)
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
        public Match Match { get; set; }

        public TextInputBotMessage(InputMessage msg) : base(BotMessageType.TextInput)
        {
            Message = msg;
        }
    }

    public enum BotMessageType
    {
        UpdateTeam,
        UpdateUser,
        TextInput
    }
}
