using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SlackBotRedux.Configuration;
using SlackBotRedux.Core;
using SlackBotRedux.Core.Modules;
using SlackBotRedux.Core.Variables;
using SlackBotRedux.Core.Variables.Interfaces;

namespace SlackBotRedux.Tests.Core.Modules
{
    [TestClass]
    public class TVariablesModule : ModuleTest<VariablesModule>
    {
        protected Mock<IVariableDictionary> VariableDictionary;

        protected override void InitializeSubject()
        {
            Subject = new VariablesModule(new VariableConfiguration()
            {
                AllowedNameCharactersRegex = "[a-zA-Z-_]",
                AllowedValueCharactersRegex= ".",
                InvalidNameCharactersRegex= "[^a-zA-Z-_]",
                PrefixString= "$"
            }, VariableDictionary.Object);
        }

        [TestInitialize]
        public void InitializeMocks()
        {
            VariableDictionary = new Mock<IVariableDictionary>();
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

            [TestMethod]
            public void ShouldRespondWithErrorMessageForValueAlreadyExistsForAdd()
            {
                VariableDictionary.Setup(vd => vd.TryAddValue(It.IsAny<string>(), It.IsAny<string>()))
                                  .Returns<string, string>(
                                      (name, value) => new TryAddValueResult(TryAddValueResultEnum.ValueAlreadyExists));

                TestRespondToWithMessage(BotName + ", add value vegetable noun", _ => { }, _ => { });

                VariableDictionary.Verify(vd => vd.TryAddValue("noun", "vegetable"));
                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), ErrorMessages.VariableValueAlreadyExists(DummyUser.Name, "noun", "vegetable")));
            }

            [TestMethod]
            public void ShouldRespondWithErrorMessageForProtectedVariableForAdd()
            {
                VariableDictionary.Setup(vd => vd.TryAddValue(It.IsAny<string>(), It.IsAny<string>()))
                                  .Returns<string, string>(
                                      (name, value) => new TryAddValueResult(TryAddValueResultEnum.VariableIsProtected));

                TestRespondToWithMessage(BotName + ", add value vegetable noun", _ => { }, _ => { });

                VariableDictionary.Verify(vd => vd.TryAddValue("noun", "vegetable"));
                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), ErrorMessages.CantModifyProtectedVariable(DummyUser.Name, "noun")));
            }

            [TestMethod]
            public void ShouldRespondWithSuccessForAdd()
            {
                VariableDictionary.Setup(vd => vd.TryAddValue(It.IsAny<string>(), It.IsAny<string>()))
                                  .Returns<string, string>(
                                      (name, value) => new TryAddValueResult(TryAddValueResultEnum.Success));

                TestRespondToWithMessage(BotName + ", add value vegetable noun", _ => { }, _ => { });

                VariableDictionary.Verify(vd => vd.TryAddValue("noun", "vegetable"));
                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), SuccessMessages.SuccessfulAddValueToVariable(DummyUser.Name)));
            }

            [TestMethod]
            public void ShouldRespondWithErrorMessageForVariableDoesNotExistForRemove()
            {
                VariableDictionary.Setup(vd => vd.TryRemoveValue(It.IsAny<string>(), It.IsAny<string>()))
                                  .Returns<string, string>(
                                      (name, value) => TryRemoveValueResult.VariableDoesNotExist);

                TestRespondToWithMessage(BotName + ", remove value vegetable noun", _ => { }, _ => { });

                VariableDictionary.Verify(vd => vd.TryRemoveValue("noun", "vegetable"));
                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), ErrorMessages.VariableDoesNotExist(DummyUser.Name, "noun")));
            }

            [TestMethod]
            public void ShouldRespondWithErrorMessageForValueDoesNotExistForRemove()
            {
                VariableDictionary.Setup(vd => vd.TryRemoveValue(It.IsAny<string>(), It.IsAny<string>()))
                                  .Returns<string, string>(
                                      (name, value) => TryRemoveValueResult.ValueDoesNotExist);

                TestRespondToWithMessage(BotName + ", remove value vegetable noun", _ => { }, _ => { });

                VariableDictionary.Verify(vd => vd.TryRemoveValue("noun", "vegetable"));
                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), ErrorMessages.VariableValueDoesNotExist(DummyUser.Name, "noun", "vegetable")));
            }

            [TestMethod]
            public void ShouldRespondWithErrorMessageForProtectedVariableForRemove()
            {
                VariableDictionary.Setup(vd => vd.TryRemoveValue(It.IsAny<string>(), It.IsAny<string>()))
                                  .Returns<string, string>(
                                      (name, value) => TryRemoveValueResult.VariableIsProtected);

                TestRespondToWithMessage(BotName + ", remove value vegetable noun", _ => { }, _ => { });

                VariableDictionary.Verify(vd => vd.TryRemoveValue("noun", "vegetable"));
                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), ErrorMessages.CantModifyProtectedVariable(DummyUser.Name, "noun")));
            }

            [TestMethod]
            public void ShouldRespondWithSuccessForRemove()
            {
                VariableDictionary.Setup(vd => vd.TryRemoveValue(It.IsAny<string>(), It.IsAny<string>()))
                                  .Returns<string, string>(
                                      (name, value) => TryRemoveValueResult.Success);

                TestRespondToWithMessage(BotName + ", remove value vegetable noun", _ => { }, _ => { });

                VariableDictionary.Verify(vd => vd.TryRemoveValue("noun", "vegetable"));
                MessageSender.Verify(ims => ims.EnqueueOutputMessage(It.IsAny<string>(), SuccessMessages.SuccessfulRemoveValueToVariable(DummyUser.Name)));
            }
        }
    }
}
