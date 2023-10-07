using UnityEngine;
using System;
using Game.AI.UtilityAI.Property;

namespace Game.AI.UtilityAI
{
    [Serializable]
    public class UAIConsideration
    {
        public AnimationCurve utilityCurve;
        public UAIProperty property;
        public float weight = 1.0f;
        public bool enabled = true;

        public float propertyScore
        {
            get { return property.normalizedValue; }
        }

        public float utilityScore
        {
            get { return utilityCurve.Evaluate(property.normalizedValue); }
        }
    }
}
