using UnityEngine;
using System.Collections;

namespace Game.AI.UtilityAI.Property
{
    public class UAIPropertyBoolean : UAIProperty
    {
        public bool startValue;
        private bool currValue;

        protected void Awake()
        {
            if (randomizeStartValue)
            {
                float tempRandom = Random.value;
                if (tempRandom < 0.5)
                    currValue = false;
                else
                    currValue = true;
            }
            else
            {
                currValue = startValue;
            }
        }

        public bool value
        {
            get { return currValue; }
            set
            {
                currValue = value;
                if (currValue)
                {
                    nValue = 1.0f;
                }
                else
                {
                    nValue = 0.0f;
                }
            }
        }

    }
}

