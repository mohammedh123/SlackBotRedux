using System;
using System.Collections.Generic;

namespace SlackBotRedux.Core.Variables.Interfaces
{
    public enum TryAddValueResult
    {
        Success,

        VariableDoesNotExist,
        ValueAlreadyExists
    }

    public enum TryRemoveValueResult
    {
        Success,

        VariableDoesNotExist,
        ValueDoesNotExist
    }

    interface IVariableDictionary
    {
        /// <summary>
        /// Adds a variable.
        /// </summary>
        /// <returns><b>true</b> if the variable was added; <b>false</b> if the variable already existed</returns>
        bool AddVariable(string variableName);

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
    }
}
