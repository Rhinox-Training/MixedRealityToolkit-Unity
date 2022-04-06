// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Serializable]
    public struct InputActionRuleCriteriaDualAxis : IInputActionRuleCriteria<Vector2>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="criteria">The criteria to check against for determining if the action should be raised.</param>
        public InputActionRuleCriteriaDualAxis(Vector2 criteria)
        {
            this.criteria = criteria;
        }
        
        [SerializeField]
        private Vector2 criteria;

        public Vector2 Criteria
        {
            get { return criteria; }
            set { criteria = value; }
        }
        
        public bool ShouldRaise(Vector2 criteria)
        {
            return this.criteria.Equals(criteria);
        }
    }
}