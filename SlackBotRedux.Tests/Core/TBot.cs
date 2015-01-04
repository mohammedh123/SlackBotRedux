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
            private void TestIfBotRespondsTo(string msgText, bool reactedTo)
            {
                Subject.ReceiveMessage(new TextInputBotMessage(new InputMessage { Text = msgText }));
                reactedTo.Should().BeTrue();
            }

            [TestMethod]
            public void ShouldRespondWhenNotIncludingBotName()
            {
                // Setup
                var reactedTo = false;
                Subject.RespondTo(new Regex("hello"), msg =>
                {
                    reactedTo = true;
                });

                // Act+Verify
                TestIfBotRespondsTo("milkbot, hello", reactedTo);
                TestIfBotRespondsTo("milkbot: hello", reactedTo);
                TestIfBotRespondsTo("@milkbot hello", reactedTo);
                TestIfBotRespondsTo("@milkbot: hello", reactedTo);
                TestIfBotRespondsTo("@milkbot, hello", reactedTo);
            }
        }
    }
}
