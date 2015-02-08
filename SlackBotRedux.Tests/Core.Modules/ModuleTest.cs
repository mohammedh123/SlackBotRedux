using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SlackBotRedux.Core;
using SlackBotRedux.Core.Models.Slack;
using SlackBotRedux.Core.Modules;

namespace SlackBotRedux.Tests.Core.Modules
{
    [TestClass]
    public abstract class ModuleTest<TSubjectType> where TSubjectType : IBotModule
    {
        protected const string BotName = "milkbot";
        protected TSubjectType Subject;
        protected User DummyUser;
        protected Mock<IMessageSender> MessageSender;

        protected abstract void InitializeSubject();

        protected ModuleTest()
        {
            DummyUser = new User()
            {
                Id = "blah",
                Name = "User"
            };
        }

        protected void TestRespondToWithMessage(string msgText, Action<Mock<IMessageSender>> msgSenderSetupFunc, Action<Mock<ITeamState>> teamStateSetupFunc)
        {
            // Setup
            MessageSender = new Mock<IMessageSender>();
            msgSenderSetupFunc(MessageSender);

            InitializeSubject();
            var msg = new TextInputBotMessage(new InputMessage
            {
                Channel = "somewhere",
                Text = msgText,
                User = DummyUser.Id
            }, DummyUser);

            // Act
            var bot = new Bot(BotName, MessageSender.Object);
            var mockTeamState = new Mock<ITeamState>();
            teamStateSetupFunc(mockTeamState);
            bot.TeamState = mockTeamState.Object;

            Subject.RegisterToBot(bot);
            bot.ReceiveMessage(msg);
        }
    }
}
