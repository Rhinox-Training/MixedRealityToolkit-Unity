// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class DigitalInputActionCacheEntry
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="criteria">The criteria to check against for determining if the action should be raised.</param>
        public DigitalInputActionCacheEntry(IMixedRealityEventSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            this.source = source;
            this.handedness = handedness;
            this.inputAction = inputAction;
        }
        
        private IMixedRealityEventSource source;

        public IMixedRealityEventSource Source => source;
        
        private Handedness handedness;
        
        public Handedness Handedness => handedness;
        
        private MixedRealityInputAction inputAction;
        
        public MixedRealityInputAction InputAction => inputAction;
    }
}