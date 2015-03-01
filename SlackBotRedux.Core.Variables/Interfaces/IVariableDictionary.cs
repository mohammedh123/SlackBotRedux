using System;
using System.Collections.Generic;
using System.Linq;

namespace SlackBotRedux.Core.Variables
{
    public enum AddVariableResult
    {
        Success,
    
        VariableAlreadyExists,
        InvalidVariableName
    }

    public enum TryAddValueResultEnum
    {
        Success,

        VariableDoesNotExist,
        ValueAlreadyExists,
        VariableIsProtected
    }

    public enum TryRemoveValueResult
    {
        Success,

        VariableDoesNotExist,
        ValueDoesNotExist,
        VariableIsProtected
    }
    
    public class TryAddValueResult
    {
        public TryAddValueResultEnum Result { get; set; }

        public IEnumerable<VariableDefinition> NewlyCreatedVariables { get; set; }

        public TryAddValueResult(TryAddValueResultEnum result, IEnumerable<VariableDefinition> newlyCreatedVariables = null)
        {
            Result = result;
            NewlyCreatedVariables = newlyCreatedVariables ?? Enumerable.Empty<VariableDefinition>();
        }
    }
}

namespace SlackBotRedux.Core.Variables.Interfaces
{
    public interface IVariableDictionary
    {
        /// <summary>
        /// Adds a variable.
        /// </summary>
        AddVariableResult AddVariable(string variableName, bool isProtected);

        /// <summary>
        /// Deletes a variable.
        /// </summary>
        /// <returns><b>true</b> if the variable was deleted; <b>false</b> otherwise</returns>
        bool DeleteVariable(string variableName, bool overrideProtection);

        VariableDefinition GetVariable(string variableName);

        /// <summary>
        /// Resolves a random value for a given variable. <paramref name="defaultValueFunc"/> will be used to return a string when no values exist for the given variable. 
        /// </summary>
        /// <param name="variableName">The name of the variable</param>
        /// <param name="defaultValueFunc">The function that will return the default value to use for a given variable</param>
        /// <returns><b>null</b> if a variable doesn't exist with the given name; a value will be returned otherwise</returns>
        string ResolveRandomValueForVariable(string variableName, Func<VariableDefinition, string> defaultValueFunc);

        /// <summary>
        /// Gets all values (un-resolved) for a given variable.
        /// </summary>
        /// <returns><b>null</b> if a variable doesn't exist with the given name; returns the list of values for a variable otherwise</returns>
        IEnumerable<VariableDefinition> GetAllValuesForVariable(string variableName);

        /// <summary>
        /// Attempts to add a value to a variable. It will create any variables that are referenced that do not exist yet.
        /// </summary>
        TryAddValueResult TryAddValue(string variableName, string value);

        /// <summary>
        /// Attempts to remove a value from a variable.
        /// </summary>
        TryRemoveValueResult TryRemoveValue(string variableName, string value);

        /// <summary>
        /// Sets the protection status of a variable.
        /// </summary>
        /// <returns><b>true</b> if the variable exists; <b>false</b> otherwise</returns>
        bool SetVariableProtection(string variableName, bool isProtected);
    }
}
