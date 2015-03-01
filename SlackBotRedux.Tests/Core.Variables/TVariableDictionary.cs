using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackBotRedux.Core.Variables;

namespace SlackBotRedux.Tests.Core.Variables
{
    [TestClass]
    public class TVariableDictionary
    {
        private const string DefaultVariablePrefix = "$";
        private const string DefaultAllowedVariableNameCharacters = "[a-zA-Z0-9_-]";
        protected VariableDictionary Subject;

        private readonly Func<VariableDefinition, string> _defaultVarName = vd => vd.Value;

        [TestInitialize]
        public void Setup()
        {
            Subject = new VariableDictionary(DefaultVariablePrefix, DefaultAllowedVariableNameCharacters);
        }

        [TestClass]
        public class AddVariable : TVariableDictionary
        {
            [TestMethod]
            public void ShouldReturnTrueWhenVariableDidNotExistBefore()
            {
                Subject.AddVariable("noun").Should().Be(true);
            }

            [TestMethod]
            public void ShouldReturnFalseWhenVariableAlreadyExists()
            {
                Subject.AddVariable("noun").Should().Be(true);
                Subject.AddVariable("noun").Should().Be(false);
            }
        }

        [TestClass]
        public class DeleteVariable : TVariableDictionary
        {
            [TestMethod]
            public void ShouldReturnTrueWhenVariableExists()
            {
                Subject.AddVariable("noun");

                Subject.DeleteVariable("noun", false).Should().BeTrue();
                Subject.GetVariable("noun").Should().BeNull();
            }

            [TestMethod]
            public void ShouldReturnFalseWhenVariableDoesntExist()
            {
                Subject.DeleteVariable("noun", false).Should().BeFalse();
                Subject.GetVariable("noun").Should().BeNull();
            }

            [TestMethod]
            public void ShouldReturnFalseWhenVariableIsProtectedAndNoOverride()
            {
                Subject.AddVariable("noun", true);

                Subject.DeleteVariable("noun", false).Should().BeFalse();
                Subject.GetVariable("noun").Should().NotBeNull();
            }

            [TestMethod]
            public void ShouldReturnTrueWhenVariableIsProtectedAndOverride()
            {
                Subject.AddVariable("noun", true);

                Subject.DeleteVariable("noun", true).Should().BeTrue();
                Subject.GetVariable("noun").Should().BeNull();
            }
        }

        [TestClass]
        public class GetVariable : TVariableDictionary
        {
            [TestMethod]
            public void ShouldReturnNullForNonexistentVariable()
            {
                var variableDef = Subject.GetVariable("noun");
                variableDef.Should().BeNull();
            }

            [TestMethod]
            public void ShouldReturnNewlyCreatedVariable()
            {
                Subject.AddVariable("noun").Should().Be(true);
                var variableDef = Subject.GetVariable("noun");
                variableDef.Should().NotBeNull();
                variableDef.Value.Should().Be("$noun");
                variableDef.Values.Should().BeEmpty();
            }
        }

        [TestClass]
        public class ResolveRandomValueForVariable : TVariableDictionary
        {
            [TestMethod]
            public void ShouldReturnNullForNonexistentVariable()
            {
                Subject.ResolveRandomValueForVariable("nonexistent", _defaultVarName).Should().BeNull();
            }

            [TestMethod]
            public void ShouldReturnDefaultValueWhenNoValuesExistForVariable()
            {
                Subject.AddVariable("noun");

                Subject.ResolveRandomValueForVariable("noun", _defaultVarName).Should().Be("$noun");
            }

            [TestMethod]
            public void ShouldReturnValueWhenValuesExistForVariable()
            {
                Subject.AddVariable("noun");
                Subject.TryAddValue("noun", "book").Result.Should().Be(TryAddValueResultEnum.Success);

                Subject.ResolveRandomValueForVariable("noun", _defaultVarName).Should().Be("book");
            }

            [TestMethod]
            public void ShouldReturnRecursiveValueWhenValuesAreRecursiveForVariable()
            {
                Subject.AddVariable("noun");
                Subject.AddVariable("vegetable");
                Subject.TryAddValue("noun", "$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("vegetable", "$noun").Result.Should().Be(TryAddValueResultEnum.Success);

                Subject.ResolveRandomValueForVariable("noun", _defaultVarName).Should().Be("$noun");
                Subject.ResolveRandomValueForVariable("vegetable", _defaultVarName).Should().Be("$vegetable");
            }

            [TestMethod]
            public void ShouldReturnValueFromSimpleRecursiveVariable()
            {
                Subject.AddVariable("noun");
                Subject.AddVariable("vegetable");
                Subject.TryAddValue("noun", "tomato").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("noun", "$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("vegetable", "$noun").Result.Should().Be(TryAddValueResultEnum.Success);

                Subject.ResolveRandomValueForVariable("noun", _defaultVarName).Should().Be("tomato");
                Subject.ResolveRandomValueForVariable("vegetable", _defaultVarName).Should().Be("tomato");
            }

            [TestMethod]
            public void ShouldReturnValueFromThreeWayRecursiveVariable()
            {
                Subject.AddVariable("noun");
                Subject.AddVariable("vegetable");
                Subject.AddVariable("root");
                Subject.TryAddValue("noun", "$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("vegetable", "$root").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("root", "$noun").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("root", "carrot").Result.Should().Be(TryAddValueResultEnum.Success);

                Subject.ResolveRandomValueForVariable("noun", _defaultVarName).Should().Be("carrot");
                Subject.ResolveRandomValueForVariable("vegetable", _defaultVarName).Should().Be("carrot");
                Subject.ResolveRandomValueForVariable("root", _defaultVarName).Should().Be("carrot");
            }

            [TestMethod]
            public void ShouldReturnValueFromMultiRecursiveVariable()
            {
                Subject.AddVariable("noun");
                Subject.AddVariable("vegetable");
                Subject.AddVariable("root");
                Subject.TryAddValue("noun", "$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("vegetable", "$root").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("root", "ultra-$noun with some $vegetable").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("root", "carrot").Result.Should().Be(TryAddValueResultEnum.Success);

                Subject.ResolveRandomValueForVariable("noun", _defaultVarName).Should().BeOneOf("ultra-carrot with some carrot", "carrot", "ultra-carrot with some $vegetable");
                Subject.ResolveRandomValueForVariable("vegetable", _defaultVarName).Should().BeOneOf("ultra-carrot with some carrot", "carrot", "ultra-carrot with some $vegetable");
                Subject.ResolveRandomValueForVariable("root", _defaultVarName).Should().BeOneOf("ultra-carrot with some carrot", "carrot", "ultra-carrot with some $vegetable");
            }

            [TestMethod]
            public void ShouldReturnNegaAntiOnionsPossibly()
            {
                Subject.AddVariable("vegetable");
                Subject.TryAddValue("vegetable", "onion").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("vegetable", "anti-$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("vegetable", "nega-$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);

                Subject.ResolveRandomValueForVariable("vegetable", _defaultVarName)
                       .Should()
                       .BeOneOf("onion", "anti-onion", "nega-onion", "anti-nega-onion", "nega-anti-onion");
            }

            [TestMethod]
            public void ShouldReturnValueFromMultipleSccs()
            {
                Subject.AddVariable("a");
                Subject.AddVariable("b");
                Subject.AddVariable("c");
                Subject.AddVariable("d");
                Subject.AddVariable("e");

                // scc #1: a, b
                Subject.TryAddValue("a", "$b").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("b", "$a").Result.Should().Be(TryAddValueResultEnum.Success);

                // scc #2: c, d
                Subject.TryAddValue("c", "$d").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("d", "$c").Result.Should().Be(TryAddValueResultEnum.Success);

                // scc #3: e
                Subject.TryAddValue("e", "wtf").Result.Should().Be(TryAddValueResultEnum.Success);

                // connect #1 to #2, and #1 to #3
                Subject.TryAddValue("a", "$c");
                Subject.TryAddValue("a", "$e");

                for (var i = 0; i < 1000; i++)
                {
                    Subject.ResolveRandomValueForVariable("a", _defaultVarName).Should().BeOneOf("wtf");
                }
            }
        }

        [TestClass]
        public class TryAddValue : TVariableDictionary
        {
            [TestMethod]
            public void ShouldReturnSuccessForNewValue()
            {
                Subject.AddVariable("noun").Should().Be(true);

                Subject.TryAddValue("noun", "$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);

                Subject.GetVariable("noun").Values
                       .Should().Contain(vd => vd.Value == "$vegetable")
                       .And.HaveCount(1);
            }

            [TestMethod]
            public void ShouldCreateVariableForReferencedVariableInValue()
            {
                Subject.AddVariable("noun").Should().Be(true);

                Subject.TryAddValue("noun", "$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);

                Subject.GetVariable("vegetable").Values.Should().BeEmpty();
                Subject.GetVariable("noun").Values
                       .Should().Contain(vd => vd.Value == "$vegetable")
                       .And.HaveCount(1);
            }

            [TestMethod]
            public void ShouldReturnVariableDoesNotExistForNonexistentValue()
            {
                Subject.TryAddValue("food", "$vegetable").Result.Should().Be(TryAddValueResultEnum.VariableDoesNotExist);
            }

            [TestMethod]
            public void ShouldReturnVariableIsProtectedForProtectedVariable()
            {
                Subject.AddVariable("food", true);
                Subject.TryAddValue("food", "$vegetable").Result.Should().Be(TryAddValueResultEnum.VariableIsProtected);
            }

            [TestMethod]
            public void ShouldReturnAlreadyExistsForExistingValue()
            {
                Subject.AddVariable("food").Should().Be(true);

                Subject.TryAddValue("food", "vegetable").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("food", "vegetable").Result
                       .Should().Be(TryAddValueResultEnum.ValueAlreadyExists);

                Subject.GetVariable("food").Values
                       .Should().Contain(vd => vd.Value == "vegetable")
                       .And.HaveCount(1);
            }

            [TestMethod]
            public void ShouldReturnSuccessWhenAddingToVariableWithNoOtherValues()
            {
                Subject.AddVariable("food").Should().Be(true);

                var result = Subject.TryAddValue("food", "$vegetable");
                result.Result.Should().Be(TryAddValueResultEnum.Success);
                result.NewlyCreatedVariables.Should().HaveCount(1);

                Subject.TryAddValue("vegetable", "$food").Result
                       .Should().Be(TryAddValueResultEnum.Success);

                Subject.GetVariable("food").Values
                       .Should().Contain(vd => vd.Value == "$vegetable")
                       .And.HaveCount(1);
            }

            [TestMethod]
            public void ShouldReturnSuccessWhenAddingToRecursiveVariableWithOtherValues()
            {
                Subject.AddVariable("food").Should().Be(true);

                Subject.TryAddValue("food", "$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("food", "onion").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("vegetable", "$food").Result
                       .Should().Be(TryAddValueResultEnum.Success);

                Subject.GetVariable("food").Values.Select(vd => vd.Value)
                       .Should().Contain(new[] { "$vegetable", "onion" })
                       .And.HaveCount(2);
                Subject.GetVariable("vegetable").Values.Select(vd => vd.Value)
                       .Should().Contain(new[] { "$food" });
            }

            [TestMethod]
            public void ShouldReturnSuccessForComplexScenario()
            {
                Subject.AddVariable("food").Should().Be(true);
                Subject.AddVariable("vegetable").Should().Be(true);

                Subject.TryAddValue("food", "$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("vegetable", "onion").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("vegetable", "anti-$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("vegetable", "nega-$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);

                Subject.GetVariable("food").Values.Select(vd => vd.Value)
                       .Should().Contain(new[] { "$vegetable" })
                       .And.HaveCount(1);
                Subject.GetVariable("vegetable").Values.Select(vd => vd.Value)
                       .Should().Contain(new[] { "onion", "anti-$vegetable", "nega-$vegetable" })
                       .And.HaveCount(3);
            }

            [TestMethod]
            public void ShouldReturnSuccessWhenAddingToVariableWithMatchingSubstring()
            {
                Subject.AddVariable("food").Should().Be(true);

                Subject.TryAddValue("food", "$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("vegetable", "$foodstuff").Result
                       .Should().Be(TryAddValueResultEnum.Success);

                Subject.GetVariable("food").Values.Select(vd => vd.Value)
                       .Should().Contain(new[] { "$vegetable" })
                       .And.HaveCount(1);
                Subject.GetVariable("vegetable").Values.Select(vd => vd.Value)
                       .Should().Contain(new[] { "$foodstuff" })
                       .And.HaveCount(1);
            }

            [TestMethod]
            public void ShouldReturnSuccessWhenDealingWithSubstringValues()
            {
                Subject.AddVariable("food").Should().Be(true);

                Subject.TryAddValue("food", "$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("food", "anti-$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("vegetable", "onion").Result.Should().Be(TryAddValueResultEnum.Success);

                Subject.GetVariable("food").Values.Select(vd => vd.Value)
                       .Should().Contain(new[] { "$vegetable", "anti-$vegetable" })
                       .And.HaveCount(2);
                Subject.GetVariable("vegetable").Values.Select(vd => vd.Value)
                       .Should().Contain(new[] { "onion" })
                       .And.HaveCount(1);
            }
        }

        [TestClass]
        public class TryRemoveValue : TVariableDictionary
        {
            [TestMethod]
            public void ShouldReturnVariableDoesNotExistForNonexistentVariable()
            {
                Subject.TryRemoveValue("noun", "a").Should().Be(TryRemoveValueResult.VariableDoesNotExist);
            }

            [TestMethod]
            public void ShouldReturnValueDoesNotExistForNonexistentValue()
            {
                Subject.AddVariable("noun");

                Subject.TryAddValue("noun", "b");
                Subject.TryRemoveValue("noun", "a").Should().Be(TryRemoveValueResult.ValueDoesNotExist);

                Subject.GetVariable("noun").Values.Should().NotBeEmpty();
            }

            [TestMethod]
            public void ShouldReturnVariableIsProtectedForProtectedVariable()
            {
                Subject.AddVariable("noun", true);
                Subject.TryAddValue("noun", "b");

                Subject.TryRemoveValue("noun", "b").Should().Be(TryRemoveValueResult.VariableIsProtected);
            }

            [TestMethod]
            public void ShouldReturnSuccessForNormalDeletion()
            {
                Subject.AddVariable("noun");

                Subject.TryAddValue("noun", "a");
                Subject.TryRemoveValue("noun", "a").Should().Be(TryRemoveValueResult.Success);

                Subject.GetVariable("noun").Values.Should().BeEmpty();
            }

            [TestMethod]
            public void ShouldReturnSuccessForNormalDeletionOfVariable()
            {
                Subject.AddVariable("noun");

                Subject.TryAddValue("noun", "$vegetable");
                Subject.TryRemoveValue("noun", "$vegetable").Should().Be(TryRemoveValueResult.Success);

                Subject.GetVariable("noun").Values.Should().BeEmpty();
            }
        }

        [TestClass]
        public class DeletingAddingAndResolvingTogether : TVariableDictionary
        {
            [TestMethod]
            public void ShouldResolveProperlyAfterMutations()
            {
                Subject.AddVariable("a");
                Subject.AddVariable("b");
                Subject.AddVariable("c");
                Subject.AddVariable("d");
                Subject.AddVariable("e");

                // scc #1: a, b
                Subject.TryAddValue("a", "$b").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("b", "$a").Result.Should().Be(TryAddValueResultEnum.Success);

                // scc #2: c, d
                Subject.TryAddValue("c", "$d").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("d", "$c").Result.Should().Be(TryAddValueResultEnum.Success);

                // scc #3: e
                Subject.TryAddValue("e", "wtf").Result.Should().Be(TryAddValueResultEnum.Success);

                // connect #1 to #2, and #1 to #3
                Subject.TryAddValue("a", "$c");
                Subject.TryAddValue("a", "$e");

                Subject.TryAddValue("a", "$d");
                Subject.TryRemoveValue("a", "$d");

                Subject.TryAddValue("a", "bah");
                Subject.TryRemoveValue("a", "bah");

                Subject.TryAddValue("a", "bah $d");
                Subject.TryRemoveValue("a", "bah $d");

                for (var i = 0; i < 1000; i++)
                {
                    Subject.ResolveRandomValueForVariable("a", _defaultVarName).Should().BeOneOf("wtf");
                }
            }
        }
    }
}
