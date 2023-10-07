using UnityEngine;
using System.Collections;

namespace Game.AI.UtilityAI.Property
{
    public class UAIPropertyBoundedInt : UAIProperty
    {
        public int minValue = 0;
        public int maxValue = 100;
        public int startValue = 50;
        public float ChangePerSec = 0;
        private int currValue;

        protected void Awake()
        {
            if (randomizeStartValue)
                currValue = Mathf.FloorToInt(Random.Range(minValue + 1, maxValue + 1)) - 1 + minValue;
            else
                currValue = startValue;
        }

        public override void UpdateProperty(float deltaTime)
        {
            value += Mathf.RoundToInt(deltaTime * ChangePerSec);
        }

        public int value
        {
            get { return currValue; }
            set
            {
                currValue = value;
                nValue = (currValue - minValue) / (maxValue - minValue);
                if (currValue < minValue)
                    currValue = minValue;
                if (currValue > maxValue)
                    currValue = maxValue;
            }
        }
    }
}

