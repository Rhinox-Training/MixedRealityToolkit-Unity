// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Cache to track what InputActions were translated to digital and were raised
    /// </summary>
    public class DigitalInputActionCache
    {
        private readonly Dictionary<IMixedRealityEventSource, List<DigitalInputActionCacheEntry>> internalCache = new Dictionary<IMixedRealityEventSource, List<DigitalInputActionCacheEntry>>();

        public bool HasInputTranslation(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            if (!this.internalCache.ContainsKey(source))
                return false;

            foreach (var entry in this.internalCache[source])
            {
                if (entry.Source == source && entry.Handedness == handedness && entry.InputAction.Equals(inputAction))
                    return true;
            }

            return false;
        }

        public void Register(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            if (!this.internalCache.ContainsKey(source))
                this.internalCache.Add(source, new List<DigitalInputActionCacheEntry>());
            
            this.internalCache[source].Add(new DigitalInputActionCacheEntry(source, handedness, inputAction));
        }

        public void Deregister(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            if (!this.internalCache.ContainsKey(source))
                return;

            DigitalInputActionCacheEntry removalEntry = null;
            foreach (var entry in this.internalCache[source])
            {
                if (entry.Source == source && entry.Handedness == handedness && entry.InputAction.Equals(inputAction))
                {
                    removalEntry = entry;
                    break;
                }
            }

            if (removalEntry != null)
                this.internalCache[source].Remove(removalEntry);
        }
    }
}