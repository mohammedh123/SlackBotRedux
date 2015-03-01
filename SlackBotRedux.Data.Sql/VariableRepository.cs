using System;
using System.Collections.Generic;
using System.Data;
using SlackBotRedux.Configuration;
using SlackBotRedux.Core.Variables;
using SlackBotRedux.Core.Variables.Interfaces;

namespace SlackBotRedux.Data.Sql
{
    public class VariableRepository : IVariableDictionary
    {
        private readonly VariableDictionary _variableCache;
        private readonly IDbConnection _conn;

        public VariableRepository(IDbConnection conn, IVariableConfiguration config)
        {
            _variableCache = new VariableDictionary(config.PrefixString, config.AllowedNameCharactersRegex);
            _conn = conn;

            LoadVariableData();
        }

        private void LoadVariableData()
        {
        }

        public bool AddVariable(string variableName, bool isProtected)
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

        public bool SetVariableProtection(string variableName, bool isProtected)
        {
            throw new NotImplementedException();
        }
    }
}
