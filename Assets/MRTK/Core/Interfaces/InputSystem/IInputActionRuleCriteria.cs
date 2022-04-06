// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface for defining Criteria checking behaviour for Input Action Rules
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInputActionRuleCriteria<T>
    {
        /// <summary>
        /// Method to check the criteria against for determining if the action should be raised.
        /// </summary>
        bool ShouldRaise(T criteria);
    }
}