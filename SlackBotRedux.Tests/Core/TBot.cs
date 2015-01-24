using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SlackBotRedux.Core;
using SlackBotRedux.Core.Models;

namespace SlackBotRedux.Tests.Core
{
    [TestClass]
    public class TBot
    {
        protected Bot Subject;
        protected string BotName = "milkbot";
        protected Mock<IMessageSender> MessageSender;

        [TestInitialize]
        public void InitializeSubject()
        {
            MessageSender = new Mock<IMessageSender>();
            Subject = new Bot(BotName, MessageSender.Object);
        }

        [TestClass]
        public class RespondTo : TBot
        {
            private void TestIfBotRespondsTo(string msgText, ref bool reactedTo)
            {
                Subject.ReceiveMessage(new TextInputBotMessage(new InputMessage { Text = msgText }, new User()));
                reactedTo.Should().BeTrue();
            }

            private void TestIfBotDoesntRespondTo(string msgText, ref bool reactedTo)
            {
                Subject.ReceiveMessage(new TextInputBotMessage(new InputMessage { Text = msgText }, new User()));
                reactedTo.Should().BeFalse();
            }

            [TestMethod]
            public void ShouldRespondWhenIncludingBotName()
            {
                // Setup
                var reactedTo = false;
                Subject.RespondTo("hello", msg =>
                {
                    reactedTo = true;
                });

                // Act+Verify
                TestIfBotRespondsTo("milkbot, hello", ref reactedTo);
                TestIfBotRespondsTo("milkbot,                  hello", ref reactedTo);
                TestIfBotRespondsTo("milkbot: hello", ref reactedTo);
                TestIfBotRespondsTo("@milkbot: hello", ref reactedTo);
                TestIfBotRespondsTo("@milkbot, hello", ref reactedTo);
            }

            [TestMethod]
            public void ShouldNotRespondWhenNotIncludingBotNameOrIncludingUnsupportedAlias()
            {
                // Setup
                var reactedTo = false;
                Subject.RespondTo("hello", msg =>
                {
                    reactedTo = true;
                });

                // Act+Verify
                TestIfBotDoesntRespondTo("hello", ref reactedTo);
                TestIfBotDoesntRespondTo("milkbot hello", ref reactedTo);
                TestIfBotDoesntRespondTo("@milkbot hello", ref reactedTo);
            }
            
            [TestMethod]
            public void ShouldPopulateMatchesWhenBotRespondsTo()
            {
                // Setup
                var reactedTo = false;
                Subject.RespondTo(@"hello (\w+)", msg =>
                {
                    reactedTo = true;

                    var textMsg = (TextInputBotMessage) msg.Message;
                    textMsg.Match.Should().NotBeNull();
                    textMsg.Match.Groups.Count.Should().Be(2);
                    textMsg.Match.Groups[1].Value.Should().Be("world");
                });

                // Act+Verify
               TestIfBotRespondsTo("milkbot, hello world", ref reactedTo);
            }

            [TestMethod]
            public void ShouldIgnoreMessagesThatDoNotMatchExactly()
            {
                // Setup
                var reactedTo = false;
                Subject.RespondTo(@"hello", msg =>
                {
                    reactedTo = true;
                });

                // Act+Verify
                TestIfBotDoesntRespondTo("milkbot, hello world", ref reactedTo);
            }

            [TestMethod]
            public void ShouldThrowExceptionWhenRegexContainsIllegalAnchor()
            {
                // Act+Verify
                Subject
                    .Invoking(b => b.RespondTo(@"^hello", _ => { }))
                    .ShouldThrow<ArgumentException>();

                Subject
                    .Invoking(b => b.RespondTo(@"hello$", _ => { }))
                    .ShouldThrow<ArgumentException>();

                Subject
                    .Invoking(b => b.RespondTo(@"hello\$", _ => { }))
                    .ShouldNotThrow();

                Subject
                    .Invoking(b => b.RespondTo(@"hel\Alo", _ => { }))
                    .ShouldThrow<ArgumentException>();

                Subject
                    .Invoking(b => b.RespondTo(@"hel\Zlo", _ => { }))
                    .ShouldThrow<ArgumentException>();

                Subject
                    .Invoking(b => b.RespondTo(@"hel\zlo", _ => { }))
                    .ShouldThrow<ArgumentException>();
            }
        }
    }
}
