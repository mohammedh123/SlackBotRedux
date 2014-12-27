using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlackBotRedux.Core.Models;

namespace SlackBotRedux.Core
{
    public class Bot
    {
        private TeamState _teamState;

        public void ReceiveMessage(BotMessage msg)
        {
            switch (msg.Type) {
                case BotMessageType.UpdateTeam:
                {
                    var updateTeamMsg = (UpdateTeamBotMessage) msg;
                    UpdateTeamState(updateTeamMsg.TeamState);

                    break;
                }
                case BotMessageType.UpdateUser:
                {
                    var updateUserMsg = (UpdateUserBotMessage)msg;
                    UpsertUserInTeamState(updateUserMsg.User);

                    break;
                }
                case BotMessageType.TextInput:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateTeamState(TeamState teamState)
        {
            _teamState = teamState;
        }

        private void UpsertUserInTeamState(User user)
        {
            if (_teamState.UsersById.ContainsKey(user.Id)) {
                _teamState.UsersById[user.Id] = user;
            }
            else {
                _teamState.UsersById.Add(user.Id, user);
            }
        }
    }
}
