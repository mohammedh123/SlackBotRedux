using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using NLog;
using SlackBotRedux.Configuration;
using SlackBotRedux.Core.Variables;
using SlackBotRedux.Core.Variables.Interfaces;
using SlackBotRedux.Data.Models;

namespace SlackBotRedux.Data.Sql
{
    public class VariableRepository : IVariableDictionary
    {
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();
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
            Logger.Info("Loading variable data from database.");

            const string getAllVariablesSql = @"
                SELECT Id, Name, IsProtected, CreatedDate, LastModifiedDate FROM dbo.Variables;
                SELECT VariableId, Value, CreatedDate FROM dbo.VariableValues;
            ";

            List<Variable> variables;
            List<VariableValue> variableValues;
            using (var gridReader = _conn.QueryMultiple(getAllVariablesSql)) {
                variables = gridReader.Read<Variable>().ToList();
                variableValues = gridReader.Read<VariableValue>().ToList();
            }

            variables.ForEach(v => _variableCache.AddVariable(v.Name, v.IsProtected));
            variableValues.ForEach(vv =>
            {
                var matchingVar = variables.FirstOrDefault(v => v.Id == vv.VariableId);
                if (matchingVar == null) {
                    Logger.Warn("Found variable value in database without matching variable, with variable id {0} and value = \"{1}\".", vv.VariableId, vv.Value);
                    return;
                }

                _variableCache.TryAddValue(matchingVar.Name, vv.Value);
            });

            Logger.Info("Finished loading variable data from database.");
        }

        public bool AddVariable(string variableName, bool isProtected)
        {
            var result = _variableCache.AddVariable(variableName, isProtected);
            if (!result) return false;

            var now = DateTimeOffset.UtcNow;
            var newVariable = new Variable()
            {
                CreatedDate = now,
                IsProtected = isProtected,
                Name = variableName,
                LastModifiedDate = now
            };
            _conn.Insert(newVariable);

            return true;
        }

        public bool DeleteVariable(string variableName, bool overrideProtection)
        {
            var result = _variableCache.DeleteVariable(variableName, overrideProtection);
            if (!result) return false;

            const string sql = @"DELETE dbo.Variables WHERE Name = @variableName";
            return _conn.Execute(sql, new {variableName}) != 0;
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
