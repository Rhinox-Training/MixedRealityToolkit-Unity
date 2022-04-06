// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface for defining Input Action Rules
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInputActionRule<T>
    {
        /// <summary>
        /// The Base Action that the rule will listen to.
        /// </summary>
        MixedRealityInputAction BaseAction { get; }

        /// <summary>
        /// The Action to raise if the criteria is met.
        /// </summary>
        MixedRealityInputAction RuleAction { get; }
        
        /// <summary>
        /// Criteria to check for determining if the action should be raised.
        /// </summary>
        T Criteria { get; }
        
        /// <summary>
        /// Boolean to check whether we want to use the new CriteriaRule
        /// </summary>
        bool UseCriteriaRule { get; }
        
        /// <summary>
        /// Object with behaviour to check the criteria against for determining if the action should be raised.
        /// If configured, this replaces Criteria property.
        /// </summary>
        IInputActionRuleCriteria<T> CriteriaRule { get; }
    }
}