// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Serializable]
    public struct InputActionRuleCriteriaVectorAxis : IInputActionRuleCriteria<Vector3>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="criteria">The criteria to check against for determining if the action should be raised.</param>
        public InputActionRuleCriteriaVectorAxis(Vector3 criteria)
        {
            this.criteria = criteria;
        }
        
        [SerializeField]
        private Vector3 criteria;

        public Vector3 Criteria
        {
            get { return criteria; }
            set { criteria = value; }
        }
        
        public bool ShouldRaise(Vector3 criteria)
        {
            return this.criteria.Equals(criteria);
        }
    }
}