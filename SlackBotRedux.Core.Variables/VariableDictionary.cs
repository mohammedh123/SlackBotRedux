using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MiscUtil;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Condensation;
using SlackBotRedux.Core.Variables.Interfaces;
using VariableGraph = QuickGraph.BidirectionalGraph<SlackBotRedux.Core.Variables.VariableDefinition, QuickGraph.SEdge<SlackBotRedux.Core.Variables.VariableDefinition>>;

namespace SlackBotRedux.Core.Variables
{
    public class VariableDictionary : IVariableDictionary
    {
        private readonly string _variablePrefix;
        private readonly string _allowedVariableNameCharacters;
        private readonly Dictionary<string, VariableDefinition> _variables;
        private readonly VariableGraph _graph;

        private readonly Regex _variableReferencesRegex;

        public VariableDictionary(string variablePrefix, string allowedVariableNameCharacters)
        {
            _variablePrefix = variablePrefix;
            _allowedVariableNameCharacters = allowedVariableNameCharacters;
            _variables = new Dictionary<string, VariableDefinition>();
            _variableReferencesRegex = new Regex(String.Format(@"(?<VariableNames>{0}{1}+)", Regex.Escape(variablePrefix), _allowedVariableNameCharacters), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _graph = new VariableGraph();
        }

        public bool AddVariable(string variableName, bool isProtected = false)
        {
            if (!variableName.StartsWith(_variablePrefix)) variableName = _variablePrefix + variableName;
            if (_variables.ContainsKey(variableName)) return false;

            var newVar = new VariableDefinition(variableName, true, isProtected);

            _variables.Add(variableName, newVar);
            _graph.AddVertex(newVar);

            return true;
        }

        public VariableDefinition GetVariable(string variableName)
        {
            if (!variableName.StartsWith(_variablePrefix)) variableName = _variablePrefix + variableName;

            VariableDefinition varDef;
            _variables.TryGetValue(variableName, out varDef);

            return varDef;
        }

        public string ResolveRandomValueForVariable(string variableName, Func<VariableDefinition, string> defaultValueFunc)
        {
            var varDef = GetVariable(variableName);
            if (varDef == null) return null;
            if (!varDef.Values.Any()) return defaultValueFunc(varDef);

            // TODO: condensate on every add, rather than every resolve
            var condensedGraph = _graph.CondensateStronglyConnected<VariableDefinition, SEdge<VariableDefinition>, BidirectionalGraph<VariableDefinition, SEdge<VariableDefinition>>>();

            return ResolveRandomValueForVariableImpl(varDef, defaultValueFunc, condensedGraph, new HashSet<VariableDefinition>(), new HashSet<VariableGraph>()) ?? defaultValueFunc(varDef);
        }

        private string ResolveRandomValueForVariableImpl(VariableDefinition varDef,
            Func<VariableDefinition, string> defaultValueFunc,
            IVertexListGraph<VariableGraph, CondensedEdge<VariableDefinition, SEdge<VariableDefinition>, VariableGraph>>
                condensedGraph, HashSet<VariableDefinition> seenVariadicValues, HashSet<VariableGraph> exhaustedSccs)
        {
            var containingSubgraph = condensedGraph.Vertices.Single(subgraph => subgraph.ContainsVertex(varDef));

            // if its a variadic value, add it to the seen list
            if (varDef.VariablesReferenced.Any()) seenVariadicValues.Add(varDef);

            if (varDef.IsVariable) {
                string currentValue = null;

                while (currentValue == null) {
                    // pick a random value to use from the possible list of values
                    // or if no more available values, try to get a random element from the scc
                    var variableToUse = GetRandomUnusedValue(varDef, seenVariadicValues) ??
                                        GetRandomUnusedValue(containingSubgraph, seenVariadicValues);

                    // something special here -- if null is returned, then that scc is exhausted of all possible values
                    // mark it down and try to get an unseen value from the scc's out-edges
                    if (variableToUse == null) {
                        exhaustedSccs.Add(containingSubgraph);
                        var value = ResolveRandomVariableFromOutEdges(defaultValueFunc, condensedGraph,
                                                                      containingSubgraph,
                                                                      seenVariadicValues,
                                                                      exhaustedSccs);
                        // no values from ANY outedges for the scc? then there are no possible values to use
                        return value;
                    }

                    currentValue = ResolveRandomValueForVariableImpl(variableToUse, defaultValueFunc, condensedGraph,
                                                                     seenVariadicValues, exhaustedSccs);
                }

                return currentValue;
            }
            else if (varDef.VariablesReferenced.Any()) {
                var currentValue = varDef.Value;

                foreach (var referencedVar in varDef.VariablesReferenced) {
                    var replacementValue = ResolveRandomValueForVariableImpl(referencedVar, defaultValueFunc,
                                                                             condensedGraph,
                                                                             seenVariadicValues, exhaustedSccs);

                    currentValue = currentValue.Replace(referencedVar.Value, replacementValue);
                }

                return currentValue;
            }
            else {
                // a vanilla value
                return varDef.Value;
            }
        }

        private VariableDefinition GetRandomUnusedValue(VariableDefinition variable, IEnumerable<VariableDefinition> seenVariadicValues)
        {
            var availableValues = variable.Values.Except(seenVariadicValues).ToList();
            if (!availableValues.Any()) return null;

            return availableValues[StaticRandom.Next(0, availableValues.Count)];
        }

        private VariableDefinition GetRandomUnusedValue(VariableGraph scc, IEnumerable<VariableDefinition> seenVariadicValues)
        {
            var availableValues = scc.Vertices.Except(seenVariadicValues).ToList();
            if (!availableValues.Any()) return null;

            return availableValues[StaticRandom.Next(0, availableValues.Count)];
        }

        private string ResolveRandomVariableFromOutEdges(Func<VariableDefinition, string> defaultValueFunc, IVertexListGraph<VariableGraph, CondensedEdge<VariableDefinition, SEdge<VariableDefinition>, VariableGraph>> condensedGraph, VariableGraph scc, HashSet<VariableDefinition> seenVariadicValues, HashSet<VariableGraph> exhaustedSccs)
        {
            // until there are no more unvisited sccs, do the following:
            // - pick a random out-edge that doesn't lead to an exhausted scc
            // - pick a random variable from it
            // - attempt to resolve it
            // - if null, try another unvisited scc

            string value = null;

            while (value == null) {
                var outEdges = condensedGraph.OutEdges(scc).Where(edge => !exhaustedSccs.Contains(edge.Target)).ToList();
                if (!outEdges.Any()) return null;

                var randomOutEdge = outEdges[StaticRandom.Next(0, outEdges.Count)];
                var randomVarFromScc = GetRandomUnusedValue(randomOutEdge.Target, seenVariadicValues);

                value = ResolveRandomValueForVariableImpl(randomVarFromScc, defaultValueFunc, condensedGraph,
                                                          seenVariadicValues, exhaustedSccs);
            }

            return value;
        }

        public IEnumerable<VariableDefinition> GetAllValuesForVariable(string variableName)
        {
            var varDef = GetVariable(variableName);
            return varDef == null ? null : varDef.Values;
        }

        public TryAddValueResult TryAddValue(string variableName, string value)
        {
            var existingVariable = GetVariable(variableName);
            if (existingVariable == null) return TryAddValueResult.VariableDoesNotExist;
            if (existingVariable.IsProtected) return TryAddValueResult.VariableIsProtected;
            if (existingVariable.Values.FirstOrDefault(v => v.Value == value) != null) return TryAddValueResult.ValueAlreadyExists;

            var variablesReferenced = new HashSet<VariableDefinition>();
            var matches = _variableReferencesRegex.Matches(value);
            var isValueActuallyVariable = false;
            foreach (Match match in matches) {
                if (match.Value == value) isValueActuallyVariable = true;

                var referencedVarName = match.Groups["VariableNames"].Value;
                AddVariable(referencedVarName, false);
                variablesReferenced.Add(_variables[referencedVarName]);
            }

            var valueDef = isValueActuallyVariable
                ? GetVariable(value)
                : new VariableDefinition(value, false) { VariablesReferenced = variablesReferenced };

            existingVariable.VariablesReferenced.UnionWith(variablesReferenced);
            existingVariable.AddValue(valueDef);

            _graph.AddVerticesAndEdge(new SEdge<VariableDefinition>(existingVariable, valueDef));
            _graph.AddVerticesAndEdgeRange(variablesReferenced.Where(vd => vd != valueDef).Select(vd => new SEdge<VariableDefinition>(valueDef, vd)));

            return TryAddValueResult.Success;
        }

        public TryRemoveValueResult TryRemoveValue(string variableName, string value)
        {
            var existingVariable = GetVariable(variableName);
            if (existingVariable == null) return TryRemoveValueResult.VariableDoesNotExist;
            if (existingVariable.IsProtected) return TryRemoveValueResult.VariableIsProtected;

            var existingValue = existingVariable.RemoveValue(value);
            if (existingValue == null) return TryRemoveValueResult.ValueDoesNotExist;

            // variables can have other stuff pointing to them in the graph, so we cant just remove the vertex
            if (existingValue.IsVariable) {
                _graph.RemoveEdge(new SEdge<VariableDefinition>(existingVariable, existingValue));
            }
            else {
                _graph.RemoveVertex(existingValue);
            }

            return TryRemoveValueResult.Success;
        }

        public bool SetVariableProtection(string variableName, bool isProtected)
        {
            var existingVariable = GetVariable(variableName);
            if (existingVariable == null) return false;

            existingVariable.IsProtected = isProtected;
            return true;
        }
    }
}
