using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackBotRedux.Core;
using SlackBotRedux.Core.Models;

namespace SlackBotRedux.Tests.Core
{
    [TestClass]
    public class TBot
    {
        protected Bot Subject;
        protected string BotName = "milkbot";

        [TestInitialize]
        public void InitializeSubject()
        {
            Subject = new Bot(BotName);
        }

        [TestClass]
        public class RespondTo : TBot
        {
            private void TestIfBotRespondsTo(string msgText, ref bool reactedTo, bool shouldWork = true)
            {
                Subject.ReceiveMessage(new TextInputBotMessage(new InputMessage { Text = msgText }));
                reactedTo.Should().BeTrue();
            }

            private void TestIfBotDoesntRespondTo(string msgText, ref bool reactedTo)
            {
                Subject.ReceiveMessage(new TextInputBotMessage(new InputMessage { Text = msgText }));
                reactedTo.Should().BeFalse();
            }

            [TestMethod]
            public void ShouldRespondWhenIncludingBotName()
            {
                // Setup
                var reactedTo = false;
                Subject.RespondTo(new Regex("hello"), msg =>
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
                Subject.RespondTo(new Regex("hello"), msg =>
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
                Subject.RespondTo(new Regex(@"hello (\w+)"), msg =>
                {
                    reactedTo = true;

                    var textMsg = (TextInputBotMessage) msg;
                    textMsg.Match.Should().NotBeNull();
                    textMsg.Match.Groups.Count.Should().Be(2);
                    textMsg.Match.Groups[1].Value.Should().Be("world");
                });

                // Act+Verify
                Subject.ReceiveMessage(new TextInputBotMessage(new InputMessage { Text = "milkbot, hello world" }));
                reactedTo.Should().BeTrue();
            }

            [TestMethod]
            public void ShouldIgnoreMessagesThatDoNotMatchExactly()
            {
                // Setup
                var reactedTo = false;
                Subject.RespondTo(new Regex(@"hello"), msg =>
                {
                    reactedTo = true;
                });

                // Act+Verify
                Subject.ReceiveMessage(new TextInputBotMessage(new InputMessage { Text = "milkbot, hello world" }));
                reactedTo.Should().BeFalse();
            }

            [TestMethod]
            public void ShouldThrowExceptionWhenRegexContainsIllegalAnchor()
            {
                // Act+Verify
                Subject
                    .Invoking(b => b.RespondTo(new Regex(@"^hello"), _ => { }))
                    .ShouldThrow<ArgumentException>();

                Subject
                    .Invoking(b => b.RespondTo(new Regex(@"hello$"), _ => { }))
                    .ShouldThrow<ArgumentException>();

                Subject
                    .Invoking(b => b.RespondTo(new Regex(@"hel\Alo"), _ => { }))
                    .ShouldThrow<ArgumentException>();

                Subject
                    .Invoking(b => b.RespondTo(new Regex(@"hel\Zlo"), _ => { }))
                    .ShouldThrow<ArgumentException>();

                Subject
                    .Invoking(b => b.RespondTo(new Regex(@"hel\zlo"), _ => { }))
                    .ShouldThrow<ArgumentException>();
            }
        }
    }
}
