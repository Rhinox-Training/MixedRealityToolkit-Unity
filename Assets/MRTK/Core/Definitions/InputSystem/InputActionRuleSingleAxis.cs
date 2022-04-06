// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Generic Input Action Rule for raising actions based on specific criteria.
    /// </summary>
    [Serializable]
    public struct InputActionRuleSingleAxis : IInputActionRule<float>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseAction">The Base Action that the rule will listen to.</param>
        /// <param name="ruleAction">The Action to raise if the criteria is met.</param>
        /// <param name="criteria">The criteria to check against for determining if the action should be raised.</param>
        public InputActionRuleSingleAxis(MixedRealityInputAction baseAction, MixedRealityInputAction ruleAction, float criteria)
        {
            this.baseAction = baseAction;
            this.ruleAction = ruleAction;
            this.criteria = criteria;
            this.useCriteriaRule = false;
            this.criteriaRule = new InputActionRuleCriteriaSingleAxis(criteria, ActionRuleComparison.Equals);
        }

        [SerializeField]
        [Tooltip("The Base Action that the rule will listen to.")]
        private MixedRealityInputAction baseAction;

        /// <inheritdoc />
        public MixedRealityInputAction BaseAction => baseAction;

        [SerializeField]
        [Tooltip("The Action to raise if the criteria is met.")]
        private MixedRealityInputAction ruleAction;

        /// <inheritdoc />
        public MixedRealityInputAction RuleAction => ruleAction;

        [SerializeField]
        [Tooltip("The criteria to check against for determining if the action should be raised.")]
        private float criteria;
        
        /// <inheritdoc />
        public float Criteria => criteria;
        
        [SerializeField]
        [Tooltip("Boolean to toggle whether the InputSystem will check Criteria or CriteriaRule when parsing events.")]
        private bool useCriteriaRule;

        /// <inheritdoc />
        public bool UseCriteriaRule => useCriteriaRule;
        
        [SerializeField]
        [Tooltip("The custom criteria checking behaviour for this Action Rule, overrides criteria.")]
        private InputActionRuleCriteriaSingleAxis criteriaRule;
        
        /// <inheritdoc />
        public IInputActionRuleCriteria<float> CriteriaRule => criteriaRule;
    }
}