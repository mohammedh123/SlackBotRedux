﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SlackBotRedux.Core;
using SlackBotRedux.Core.Models.Slack;
using SlackBotRedux.Core.Modules;
using SlackBotRedux.Data.Interfaces;

namespace SlackBotRedux.Tests.Core.Modules
{
    [TestClass]
    public class TQuotesModule : ModuleTest<QuotesModule>
    {
        protected Mock<IRecentMessageRepository> RecentMessageRepository;
        protected Mock<IQuoteRepository> QuoteRepository;

        [TestInitialize]
        public virtual void InitializeMocks()
        {
            RecentMessageRepository = new Mock<IRecentMessageRepository>();
            QuoteRepository = new Mock<IQuoteRepository>();
        }

        protected override void InitializeSubject()
        {
            Subject = new QuotesModule(RecentMessageRepository.Object, QuoteRepository.Object);
        }

        [TestClass]
        public class RegisterToBot : TQuotesModule
        {
            [TestMethod]
            public void ShouldRespondWithErrorBecauseOfBadUsername()
            {
                TestRespondToWithMessage(BotName + ", remember Kaladin storming", _ => { },
                    mock => mock
                        .Setup(ts => ts.GetUserByUsername(It.IsAny<string>()))
                        .Returns((User) null));

                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), ErrorMessages.NoUserExistsForUsername(DummyUser.Name, "Kaladin")));
            }

            [TestMethod]
            public void ShouldRespondWithErrorBecauseOfSelfTarget()
            {
                TestRespondToWithMessage(String.Format("{0}, remember {1} storming", BotName, DummyUser.Name), _ => { },
                    mock => mock
                        .Setup(ts => ts.GetUserByUsername(It.IsAny<string>()))
                        .Returns(DummyUser));

                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), ErrorMessages.CantUseSelfAsTarget(DummyUser.Name, "quote")));
            }

            [TestMethod]
            public void ShouldRespondWithErrorBecauseOfNoQuotesForUser()
            {
                var rememberTarget = new User() { Id = "gah", Name = "Jasnah" };

                // Jasnah has not said anything containing "storming"
                RecentMessageRepository.Setup(irmr => irmr.GetRecentMessagesByUserId(rememberTarget.Id))
                                       .Returns<string>(id => new MessageList());

                TestRespondToWithMessage(String.Format("{0}, remember {1} storming", BotName, rememberTarget.Name), _ => { },
                    mock => mock
                        .Setup(ts => ts.GetUserByUsername(It.IsAny<string>()))
                        .Returns(rememberTarget));

                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), ErrorMessages.NoQuotesForUser(DummyUser.Name, rememberTarget.Name, "storming")));
            }

            [TestMethod]
            public void ShouldRespondWithSuccess()
            {
                var rememberTarget = new User() { Id = "gah", Name = "Jasnah" };
                var textIncludingStorming = "storming parshmen taking our jobs";

                // Jasnah has said something containing "storming"
                RecentMessageRepository.Setup(irmr => irmr.GetRecentMessagesByUserId(rememberTarget.Id))
                                       .Returns<string>(id => new MessageList(){ textIncludingStorming });

                TestRespondToWithMessage(String.Format("{0}, remember {1} storming", BotName, rememberTarget.Name), _ => { },
                    mock => mock
                        .Setup(ts => ts.GetUserByUsername(It.IsAny<string>()))
                        .Returns(rememberTarget));

                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), SuccessMessages.SuccessfulRemember(DummyUser.Name, textIncludingStorming)));
            }
        }
    }
}
