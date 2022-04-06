﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Generic Input Action Rule for raising actions based on specific criteria.
    /// </summary>
    [Serializable]
    public struct InputActionRuleVectorAxis : IInputActionRule<Vector3>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseAction">The Base Action that the rule will listen to.</param>
        /// <param name="ruleAction">The Action to raise if the criteria is met.</param>
        /// <param name="criteria">The criteria to check against for determining if the action should be raised.</param>
        public InputActionRuleVectorAxis(MixedRealityInputAction baseAction, MixedRealityInputAction ruleAction, Vector3 criteria)
        {
            this.baseAction = baseAction;
            this.ruleAction = ruleAction;
            this.criteria = criteria;
            this.useCriteriaRule = false;
            this.criteriaRule = new InputActionRuleCriteriaVectorAxis(criteria);
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
        private Vector3 criteria;

        /// <inheritdoc />
        public Vector3 Criteria => criteria;
        
        [SerializeField]
        [Tooltip("Boolean to toggle whether the InputSystem will check Criteria or CriteriaRule when parsing events.")]
        private bool useCriteriaRule;

        /// <inheritdoc />
        public bool UseCriteriaRule => useCriteriaRule;
        
        [SerializeField]
        [Tooltip("The custom criteria checking behaviour for this Action Rule, overrides criteria.")]
        private InputActionRuleCriteriaVectorAxis criteriaRule;
        
        /// <inheritdoc />
        public IInputActionRuleCriteria<Vector3> CriteriaRule => criteriaRule;
    }
}