using UnityEngine;
using System.Collections;

namespace Game.AI.UtilityAI.Property
{
    [System.Serializable]
    public class UAIProperty : MonoBehaviour
    {
        public string id;
        protected float nValue;

        public float normalizedValue
        {
            get { return nValue; }
        }

        public bool randomizeStartValue;


        protected virtual void Update()
        {
            UpdateProperty(Time.deltaTime);
        }

        public virtual void UpdateProperty(float deltaTime) { }
    }
}

