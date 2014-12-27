using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackBotRedux.Core
{
    interface IModule
    {
        void Register(ISlackClient client);
    }
}
