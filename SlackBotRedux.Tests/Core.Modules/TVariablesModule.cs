using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SlackBotRedux.Core;
using SlackBotRedux.Core.Modules;

namespace SlackBotRedux.Tests.Core.Modules
{
    [TestClass]
    public class TVariablesModule : ModuleTest<VariablesModule>
    {
        protected override void InitializeSubject()
        {
            Subject = new VariablesModule();
        }

        [TestClass]
        public class RegisterToBot : TVariablesModule
        {
            [TestMethod]
            public void ShouldRespondWithErrorMessageForCreateVar()
            {
                TestRespondToWithMessage(BotName + ", create var dfsdfsdf", _ => { }, _ => { });

                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), ErrorMessages.RedirectCreateVar(DummyUser.Name)));
            }
        }
    }
}
