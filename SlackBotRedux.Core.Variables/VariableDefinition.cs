using System;
using System.Collections.Generic;
using System.Linq;

namespace SlackBotRedux.Core.Variables
{
    // todo: this class is sort of a mess because of the constraints of quickgraph...unmess it
    public class VariableDefinition
    {
        private readonly string _value;
        public string Value { get { return _value; } }

        public bool IsVariable { get; private set; }
        public bool IsProtected { get; internal set; }

        internal readonly List<VariableDefinition> _values;
        public IEnumerable<VariableDefinition> Values { get { return _values; } }

        internal HashSet<VariableDefinition> VariablesReferenced { get; set; }

        public VariableDefinition(string value, bool isVariable, bool isProtected = false)
        {
            _value = value;
            _values = new List<VariableDefinition>();

            IsVariable = isVariable;
            IsProtected = isProtected;
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
