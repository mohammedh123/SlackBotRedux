using System;
using System.Collections.Generic;
using System.Linq;

namespace SlackBotRedux.Core.Variables
{
    public class VariableDefinition
    {
        private readonly string _value;
        public string Value { get { return _value; } }

        public bool IsVariable { get; set; }

        internal readonly List<VariableDefinition> _values;
        public IEnumerable<VariableDefinition> Values { get { return _values; } }

        internal HashSet<VariableDefinition> VariablesReferenced { get; set; }

        public VariableDefinition(string value, bool isVariable)
        {
            _value = value;
            _values = new List<VariableDefinition>();

            IsVariable = isVariable;
            VariablesReferenced = new HashSet<VariableDefinition>();
        }

        internal void AddValue(VariableDefinition varDef)
        {
            _values.Add(varDef);
        }

        internal VariableDefinition RemoveValue(string value)
        {
            var varValue = _values.FirstOrDefault(vd => vd.Value == value);

            _values.Remove(varValue);
            return varValue;
        }

        public override string ToString()
        {
            return String.Format("{0} -> ({1})", Value, String.Join(",", Values.Select(vv => vv.Value)));
        }
    }
}
