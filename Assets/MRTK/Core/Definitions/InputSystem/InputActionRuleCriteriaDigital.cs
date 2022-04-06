// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Serializable]
    public struct InputActionRuleCriteriaDigital : IInputActionRuleCriteria<bool>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="criteria">The criteria to check against for determining if the action should be raised.</param>
        public InputActionRuleCriteriaDigital(bool criteria)
        {
            this.criteria = criteria;
        }
        
        [SerializeField]
        private bool criteria;

        public bool Criteria
        {
            get { return criteria; }
            set { criteria = value; }
        }
        
        public bool ShouldRaise(bool criteria)
        {
            return this.criteria = criteria;
        }
    }
}