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
        private readonly IVariableConfiguration _config;

        public VariableRepository(IDbConnection conn, IVariableConfiguration config)
        {
            _variableCache = new VariableDictionary(config.PrefixString, config.AllowedNameCharactersRegex, config.InvalidNameCharactersRegex);
            _conn = conn;
            _config = config;

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

        public AddVariableResult AddVariable(string variableName, bool isProtected)
        {
            var result = _variableCache.AddVariable(variableName, isProtected);
            if (result != AddVariableResult.Success) return result;

            InsertVariable(variableName, isProtected);

            return result;
        }

        public bool DeleteVariable(string variableName, bool overrideProtection)
        {
            var result = _variableCache.DeleteVariable(variableName, overrideProtection);
            if (!result) return false;

            const string sql = @"
                DELETE dbo.VariableValues FROM dbo.VariableValues VV
                JOIN Variables V ON VV.VariableId = V.Id
                WHERE V.Name = @variableName;
                DELETE FROM dbo.Variables WHERE Name = @variableName;
            ";

            return _conn.Execute(sql, new {variableName}) != 0;
        }

        public VariableDefinition GetVariable(string variableName)
        {
            return _variableCache.GetVariable(variableName);
        }

        public string ResolveRandomValueForVariable(string variableName, Func<VariableDefinition, string> defaultValueFunc)
        {
            return _variableCache.ResolveRandomValueForVariable(variableName, defaultValueFunc);
        }

        public IEnumerable<VariableDefinition> GetAllValuesForVariable(string variableName)
        {
            return _variableCache.GetAllValuesForVariable(variableName);
        }

        public TryAddValueResult TryAddValue(string variableName, string value)
        {
            var result = _variableCache.TryAddValue(variableName, value);
            if (result.Result != TryAddValueResultEnum.Success) return result;

            foreach (var newVar in result.NewlyCreatedVariables) {
                // todo: make it 1 db call rather than n
                InsertVariable(StripPrefix(newVar.Value), newVar.IsProtected);
            }

            const string sql = @"
                INSERT INTO dbo.VariableValues (VariableId, Value, CreatedDate)
                SELECT Id, @value, @now
                FROM Variables
                WHERE Name = @variableName 
            ";

            var now = DateTimeOffset.UtcNow;
            _conn.Execute(sql, new {value, variableName, now});

            return result;
        }

        public TryRemoveValueResult TryRemoveValue(string variableName, string value)
        {
            var result = _variableCache.TryRemoveValue(variableName, value);
            if (result != TryRemoveValueResult.Success) return result;

            const string sql = @"
                DELETE dbo.VariableValues FROM dbo.VariableValues VV
                JOIN Variables V ON VV.VariableId = V.Id
                WHERE V.Name = @variableName AND VV.Value = @value
            ";

            _conn.Execute(sql, new {variableName, value});

            return result;
        }

        public bool SetVariableProtection(string variableName, bool isProtected)
        {
            if (!_variableCache.SetVariableProtection(variableName, isProtected)) return false;

            const string sql = @"
                UPDATE dbo.Variables
                SET IsProtected = @isProtected
                WHERE Name = @variableName
            ";

            _conn.Execute(sql, new {variableName, isProtected});

            return true;
        }

        private string StripPrefix(string variableName)
        {
            if (variableName.StartsWith(_config.PrefixString))
                variableName = variableName.Substring(_config.PrefixString.Length);

            return variableName;
        }
        
        private void InsertVariable(string variableName, bool isProtected)
        {
            var now = DateTimeOffset.UtcNow;
            var newVariable = new Variable()
            {
                CreatedDate = now,
                IsProtected = isProtected,
                Name = variableName,
                LastModifiedDate = now
            };
            _conn.Insert(newVariable);
        }
    }
}
