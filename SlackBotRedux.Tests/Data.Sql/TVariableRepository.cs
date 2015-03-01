using System.Collections;
using System.Linq;
using Dapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackBotRedux.Configuration;
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
            Subject = new VariableRepository(Connection, new VariableConfiguration() { AllowedNameCharactersRegex = "[a-zA-z-_]", PrefixString = "$"});
        }

        [TestClass]
        public class LoadDataUponConstruction : TVariableRepository
        {
            [TestMethod]
            public void ShouldLoadVariableDataUponConstruction()
            {
                Subject.AddVariable("a", false);
                Subject.AddVariable("b", false);
                Subject.TryAddValue("a", "bah");
                Subject.TryAddValue("b", "$a");

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
                Subject.AddVariable("noun", false);

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
                Subject.AddVariable("a", false);
                Subject.TryAddValue("a", "bah");

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
                Subject.AddVariable("noun", false);

                Subject.TryAddValue("noun", "test");

                Connection.Query<VariableValue>("SELECT * FROM dbo.VariableValues")
                          .Should()
                          .ContainSingle(vv => vv.Value == "test");
            }

            [TestMethod]
            public void ShouldAddVariablesReferencedIfTheyDontExist()
            {
                Subject.AddVariable("noun", false);

                Subject.TryAddValue("noun", "$vegetable");

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
                Subject.AddVariable("noun", false);
                Subject.TryAddValue("noun", "book");

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
                Subject.AddVariable("noun", false);

                Subject.SetVariableProtection("noun", true);

                Connection.Query<Variable>("SELECT * FROM dbo.Variables").Single().IsProtected.Should().BeTrue();
            }
        }
    }
}
