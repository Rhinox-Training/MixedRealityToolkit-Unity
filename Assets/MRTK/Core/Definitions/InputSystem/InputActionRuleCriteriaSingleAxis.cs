// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Serializable]
    public struct InputActionRuleCriteriaSingleAxis : IInputActionRuleCriteria<float>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="criteria">The criteria to check against for determining if the action should be raised.</param>
        public InputActionRuleCriteriaSingleAxis(float criteria, ActionRuleComparison comparison = ActionRuleComparison.Equals)
        {
            this.criteria = criteria;
            this.comparison = comparison;
        }
        
        [SerializeField]
        [Tooltip("The criteria to check against for determining if the action should be raised.")]
        private float criteria;

        public float Criteria
        {
            get { return criteria; }
            set { criteria = value; }
        }

        [SerializeField] [Tooltip("How to check the criteria against the BaseAction criteria.")]
        private ActionRuleComparison comparison;

        public ActionRuleComparison Comparison
        {
            get { return comparison; }
            set { comparison = value; }
        }

        /// <inheritdoc />
        public bool ShouldRaise(float criteria)
        {
            switch (comparison)
            {
                case ActionRuleComparison.Equals:
                    return criteria.Equals(this.criteria);
                case ActionRuleComparison.LessThan:
                    return criteria < this.criteria;
                case ActionRuleComparison.LessThanOrEquals:
                    return criteria <= this.criteria;
                case ActionRuleComparison.GreaterThan:
                    return criteria > this.criteria;
                case ActionRuleComparison.GreaterThanOrEquals:
                    return criteria >= this.criteria;
                default:
                    throw new ArgumentOutOfRangeException($"ActionRuleComparison {comparison} is not supported.");
            }
        }
    }
}