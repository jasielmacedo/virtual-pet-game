using UnityEngine;
using System.Collections;

namespace Core.Game
{
    public class Neuron : MonoBehaviour
    {
        public int instanceID
        {
            get{
                return this.gameObject.GetInstanceID();
            }
        }
    }
}
