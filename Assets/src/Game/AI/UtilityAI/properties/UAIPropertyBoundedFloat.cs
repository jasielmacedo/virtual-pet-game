using UnityEngine;
using System.Collections;

namespace Game.AI.UtilityAI.Property
{
    public class UAIPropertyBoundedFloat : UAIProperty
    {
        public float minValue = 0.0f;
        public float maxValue = 100.0f;
        public float startValue = 50.0f;
        public float ChangePerSec = 0.0f;
        private float currValue;

        protected void Awake()
        {
            if (randomizeStartValue)
                currValue = Random.Range(minValue, maxValue) + minValue;
            else
                currValue = startValue;
        }

        public override void UpdateProperty(float deltaTime)
        {
            value += deltaTime * ChangePerSec;
        }

        public float value
        {
            get { return currValue; }
            set
            {
                currValue = value;
                if (currValue < minValue)
                    currValue = minValue;
                if (currValue > maxValue)
                    currValue = maxValue;
                nValue = (currValue - minValue) / (maxValue - minValue);
            }
        }
    }
}
