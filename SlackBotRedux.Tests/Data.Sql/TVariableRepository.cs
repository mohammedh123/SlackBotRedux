using System.Collections;
using System.Linq;
using Dapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackBotRedux.Configuration;
using SlackBotRedux.Core.Variables;
using SlackBotRedux.Data.Models;
using SlackBotRedux.Data.Sql;

namespace SlackBotRedux.Tests.Data.Sql
{
    [TestClass]
    internal class TVariableRepository : DatabaseTest
    {
        protected VariableRepository Subject;

        [TestInitialize]
        public void InitializeSubject()
        {
            Subject = new VariableRepository(Connection, new VariableConfiguration() { AllowedNameCharactersRegex = "[a-zA-Z-_]", PrefixString = "$", InvalidNameCharactersRegex = "[^a-zA-Z-_]"});
        }

        [TestClass]
        public class LoadDataUponConstruction : TVariableRepository
        {
            [TestMethod]
            public void ShouldLoadVariableDataUponConstruction()
            {
                Subject.AddVariable("a", false).Should().Be(AddVariableResult.Success);
                Subject.AddVariable("b", false).Should().Be(AddVariableResult.Success);
                Subject.TryAddValue("a", "bah").Result.Should().Be(TryAddValueResultEnum.Success);
                Subject.TryAddValue("b", "$a").Result.Should().Be(TryAddValueResultEnum.Success);

                InitializeSubject();

                var aVariable = Subject.GetVariable("a");
                aVariable.Should().NotBeNull();
                aVariable.IsProtected.Should().BeFalse();
                aVariable.Values.Should().HaveCount(1);

                var bVariable = Subject.GetVariable("b");
                bVariable.Should().NotBeNull();
                bVariable.IsProtected.Should().BeFalse();
                bVariable.Values.Should().HaveCount(1);
                bVariable.Values.Single().Value.Should().Be("$a");
            }
        }

        [TestClass]
        public class AddVariable : TVariableRepository
        {
            [TestMethod]
            public void ShouldPersistNewVariableInDatabase()
            {
                Subject.AddVariable("noun", false).Should().Be(AddVariableResult.Success);

                var record = Connection.Query<Variable>("SELECT * FROM dbo.Variables").SingleOrDefault();
                record.Should().NotBeNull();
                record.Name.Should().Be("noun");
                record.IsProtected.Should().BeFalse();
                record.Id.Should().NotBe(0);
            }
        }

        [TestClass]
        public class DeleteVariable : TVariableRepository
        {
            [TestMethod]
            public void ShouldDeleteVariableAndAllValuesFromDatabase()
            {
                Subject.AddVariable("a", false).Should().Be(AddVariableResult.Success);
                Subject.TryAddValue("a", "bah").Result.Should().Be(TryAddValueResultEnum.Success);

                Subject.DeleteVariable("a", true);

                Connection.Query<Variable>("SELECT * FROM dbo.Variables").Should().BeEmpty();
                Connection.Query<VariableValue>("SELECT * FROM dbo.VariableValues").Should().BeEmpty();
            }
        }

        [TestClass]
        public class TryAddValue : TVariableRepository
        {
            [TestMethod]
            public void ShouldPersistNewValueInDatabase()
            {
                Subject.AddVariable("noun", false).Should().Be(AddVariableResult.Success);

                Subject.TryAddValue("noun", "test").Result.Should().Be(TryAddValueResultEnum.Success);

                Connection.Query<VariableValue>("SELECT * FROM dbo.VariableValues")
                          .Should()
                          .ContainSingle(vv => vv.Value == "test");
            }

            [TestMethod]
            public void ShouldAddVariablesReferencedIfTheyDontExist()
            {
                Subject.AddVariable("noun", false).Should().Be(AddVariableResult.Success);

                Subject.TryAddValue("noun", "$vegetable").Result.Should().Be(TryAddValueResultEnum.Success);

                Connection.Query<VariableValue>("SELECT * FROM dbo.VariableValues")
                          .Should()
                          .ContainSingle(vv => vv.Value == "$vegetable");

                Connection.Query<Variable>("SELECT * FROM dbo.Variables").Should().Contain(v => v.Name == "vegetable");
            }
        }

        [TestClass]
        public class TryRemoveValue : TVariableRepository
        {
            [TestMethod]
            public void ShouldRemoveValueFromDatabase()
            {
                Subject.AddVariable("noun", false).Should().Be(AddVariableResult.Success);
                Subject.TryAddValue("noun", "book").Result.Should().Be(TryAddValueResultEnum.Success);

                Subject.TryRemoveValue("noun", "book");

                Connection.Query<VariableValue>("SELECT * FROM dbo.VariableValues").Should().BeEmpty();
            }
        }

        [TestClass]
        public class SetVariableProtection : TVariableRepository
        {
            [TestMethod]
            public void ShouldSetVariableProtectionInDatabase()
            {
                Subject.AddVariable("noun", false).Should().Be(AddVariableResult.Success);

                Subject.SetVariableProtection("noun", true);

                Connection.Query<Variable>("SELECT * FROM dbo.Variables").Single().IsProtected.Should().BeTrue();
            }
        }
    }
}
