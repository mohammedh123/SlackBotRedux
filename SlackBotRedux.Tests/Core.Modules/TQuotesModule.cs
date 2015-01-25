using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SlackBotRedux.Core;
using SlackBotRedux.Core.Data;
using SlackBotRedux.Core.Models;
using SlackBotRedux.Core.Modules;

namespace SlackBotRedux.Tests.Core.Modules
{
    [TestClass]
    public class TQuotesModule
    {
        private const string BotName = "milkbot";
        protected QuotesModule Subject;

        protected User DummyUser;
        protected Mock<IRecentMessageRepository> RecentMessageRepository;
        protected Mock<IBot> Bot;
        protected Mock<IMessageSender> MessageSender;

        [TestInitialize]
        public virtual void InitializeMocks()
        {
            RecentMessageRepository = new Mock<IRecentMessageRepository>();

            DummyUser = new User()
            {
                Id = "blah",
                Name = "User"
            };
        }

        public void InitializeSubject()
        {
            Subject = new QuotesModule(RecentMessageRepository.Object);
        }

        [TestClass]
        public class RegisterToBot : TQuotesModule
        {
            [TestMethod]
            public void ShouldRespondWithErrorBecauseOfBadUsername()
            {
                TestRespondToWithMessage(BotName + ", remember Kaladin storming",
                    mock => mock
                        .Setup(ts => ts.GetUserByUsername(It.IsAny<string>()))
                        .Returns((User) null));

                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), ErrorMessages.NoUserExistsForUsername(DummyUser.Name, "Kaladin")));
            }

            [TestMethod]
            public void ShouldRespondWithErrorBecauseOfSelfTarget()
            {
                TestRespondToWithMessage(String.Format("{0}, remember {1} storming", BotName, DummyUser.Name),
                    mock => mock
                        .Setup(ts => ts.GetUserByUsername(It.IsAny<string>()))
                        .Returns(DummyUser));

                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), ErrorMessages.CantUseSelfAsTarget(DummyUser.Name, "quote")));
            }

            [TestMethod]
            public void ShouldRespondWithErrorBecauseOfNoQuotesForUser()
            {
                var rememberTarget = new User() {Id = "gah", Name = "Jasnah"};

                // Jasnah has not said anything containing "storming"
                RecentMessageRepository.Setup(irmr => irmr.GetRecentMessagesByUserId(rememberTarget.Id))
                                       .Returns<string>(id => new MessageList());

                TestRespondToWithMessage(String.Format("{0}, remember {1} storming", BotName, rememberTarget.Name),
                    mock => mock
                        .Setup(ts => ts.GetUserByUsername(It.IsAny<string>()))
                        .Returns(rememberTarget));

                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), ErrorMessages.NoQuotesForUser(DummyUser.Name, rememberTarget.Name, "storming")));
            }

            private void TestRespondToWithMessage(string msgText, Action<Mock<ITeamState>> teamStateSetupFunc )
            {
                // Setup
                MessageSender = new Mock<IMessageSender>();
                InitializeSubject();
                var msg = new TextInputBotMessage(new InputMessage
                {
                    Channel = "somewhere",
                    Text = msgText
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

        private class MessageList : List<InputMessageSlim>
        {
            public void Add(string text)
            {
                Add(new InputMessageSlim() {Text = text});
            }
        }
    }
}
