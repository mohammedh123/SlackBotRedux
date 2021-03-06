﻿using System.Collections.Generic;
using SlackBotRedux.Core.Models.Slack;

namespace SlackBotRedux.Data.Interfaces
{
    public interface IRecentMessageRepository
    {
        void AddNewMessage(InputMessageSlim msg);

        IEnumerable<InputMessageSlim> GetRecentMessagesByUserId(string userId);
    }
}