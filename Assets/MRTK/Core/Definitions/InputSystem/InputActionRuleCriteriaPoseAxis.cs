// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Serializable]
    public struct InputActionRuleCriteriaPoseAxis : IInputActionRuleCriteria<MixedRealityPose>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="criteria">The criteria to check against for determining if the action should be raised.</param>
        public InputActionRuleCriteriaPoseAxis(MixedRealityPose criteria)
        {
            this.criteria = criteria;
        }
        
        [SerializeField]
        private MixedRealityPose criteria;

        public MixedRealityPose Criteria
        {
            get { return criteria; }
            set { criteria = value; }
        }
        
        public bool ShouldRaise(MixedRealityPose criteria)
        {
            return this.criteria.Equals(criteria);
        }
    }
}