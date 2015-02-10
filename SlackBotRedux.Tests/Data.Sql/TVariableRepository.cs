using System;
using Dapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackBotRedux.Data.Interfaces;
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
            Subject = new VariableRepository(Connection);
        }

        private int InsertNewVariableValue(string variableName, string value)
        {
            const string sql = @"
                IF NOT EXISTS (SELECT 1 FROM Variables WHERE Name = @variableName)
                BEGIN
                    INSERT INTO Variables(Name, IsProtected, CreatedDate, LastModifiedDate)
                    VALUES(@variableName, 0, @now, @now)
                END
                DECLARE @variableId INT;
                SELECT @variableId = Id FROM Variables WHERE Name = @variableName;
                INSERT INTO VariableValues(VariableId, Value, CreatedDate) VALUES(@variableId, @value, @now);
            ";

            return Connection.Execute(sql, new {variableName, value, now = DateTimeOffset.UtcNow });
        }

        [TestClass]
        public class ValidateAddingValue : TVariableRepository
        {
            [TestMethod]
            public void ShouldReturnSuccessForNewValue()
            {
                Subject.ValidateAddingValue("food", "vegetable")
                       .Should().Be(ValidateAddingValueResult.Success);
            }

            [TestMethod]
            public void ShouldReturnAlreadyExistsForExistingValue()
            {
                InsertNewVariableValue("food", "vegetable").Should().Be(2);
                Subject.ValidateAddingValue("food", "vegetable")
                       .Should().Be(ValidateAddingValueResult.AlreadyExists);
            }

            [TestMethod]
            public void ShouldReturnRecursiveWhenAddingToVariableWithNoOtherValues()
            {
                InsertNewVariableValue("food", "$vegetable").Should().Be(2);
                Subject.ValidateAddingValue("vegetable", "$food")
                       .Should().Be(ValidateAddingValueResult.Recursive);
            }

            [TestMethod]
            public void ShouldReturnSuccessWhenAddingToRecursiveVariableWithOtherValues()
            {
                InsertNewVariableValue("food", "$vegetable").Should().Be(2);
                InsertNewVariableValue("food", "onion").Should().Be(1);
                Subject.ValidateAddingValue("vegetable", "$food")
                       .Should().Be(ValidateAddingValueResult.Success);
            }

            [TestMethod]
            public void ShouldReturnRecursiveWhenAddingToThreewayRecursionWithOtherValues()
            {
                InsertNewVariableValue("food", "$vegetable").Should().Be(2);
                InsertNewVariableValue("vegetable", "onion").Should().Be(2);
                InsertNewVariableValue("root", "$food").Should().Be(2);
                Subject.ValidateAddingValue("vegetable", "$root")
                       .Should().Be(ValidateAddingValueResult.Success);
            }

            [TestMethod]
            public void ShouldReturnSuccessWhenAddingToVariableWithMatchingSubstring()
            {
                InsertNewVariableValue("food", "$vegetable").Should().Be(2);
                Subject.ValidateAddingValue("vegetable", "$foodstuff")
                       .Should().Be(ValidateAddingValueResult.Success);
            }
        }
    }
}
