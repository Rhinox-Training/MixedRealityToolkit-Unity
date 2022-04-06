// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Serializable]
    public struct InputActionRuleCriteriaQuaternionAxis : IInputActionRuleCriteria<Quaternion>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="criteria">The criteria to check against for determining if the action should be raised.</param>
        public InputActionRuleCriteriaQuaternionAxis(Quaternion criteria)
        {
            this.criteria = criteria;
        }
        
        [SerializeField]
        private Quaternion criteria;

        public Quaternion Criteria
        {
            get { return criteria; }
            set { criteria = value; }
        }
        
        public bool ShouldRaise(Quaternion criteria)
        {
            return this.criteria.Equals(criteria);
        }
    }
}