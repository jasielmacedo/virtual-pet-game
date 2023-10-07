using UnityEngine;
using System.Collections;

namespace Game.AI.UtilityAI.Property
{
    public class UAIPropertyBoundedDouble : UAIProperty
    {

        public double minValue = 0.0d;
        public double maxValue = 100.0d;
        public double startValue = 50.0d;
        public double ChangePerSec = 0.0d;
        private double currValue;

        protected void Awake()
        {
            if (randomizeStartValue)
                currValue = Random.value * (maxValue - minValue) + minValue;
            else
                currValue = startValue;
        }

        public override void UpdateProperty(float deltaTime)
        {
            value += deltaTime * ChangePerSec;
        }

        public double value
        {
            get { return currValue; }
            set
            {
                currValue = value;
                if (currValue < minValue)
                    currValue = minValue;
                if (currValue > maxValue)
                    currValue = maxValue;
                nValue = (float)((currValue - minValue) / (maxValue - minValue));
            }
        }
    }
}

