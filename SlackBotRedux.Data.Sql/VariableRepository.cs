using System;
using System.Collections.Generic;
using System.Data;
using SlackBotRedux.Core.Variables;
using SlackBotRedux.Core.Variables.Interfaces;

namespace SlackBotRedux.Data.Sql
{
    public class VariableRepository : IVariableDictionary
    {
        private readonly VariableDictionary _variableCache;
        private readonly IDbConnection _conn;

        public VariableRepository(IDbConnection conn)
        {
            _variableCache = new VariableDictionary("$", "[a-zA-Z-_]");
            _conn = conn;

            LoadVariableData();
        }

        private void LoadVariableData()
        {
            throw new NotImplementedException();
        }

        public bool AddVariable(string variableName)
        {
            throw new NotImplementedException();
        }

        public VariableDefinition GetVariable(string variableName)
        {
            throw new NotImplementedException();
        }

        public string ResolveRandomValueForVariable(string variableName, Func<VariableDefinition, string> defaultValueFunc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VariableDefinition> GetAllValuesForVariable(string variableName)
        {
            throw new NotImplementedException();
        }

        public TryAddValueResult TryAddValue(string variableName, string value)
        {
            throw new NotImplementedException();
        }

        public TryRemoveValueResult TryRemoveValue(string variableName, string value)
        {
            throw new NotImplementedException();
        }
    }
}
