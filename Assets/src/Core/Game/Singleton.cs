using UnityEngine;

namespace Core.Game
{
    public abstract class Singleton<T> : Neuron where T : MonoBehaviour
    {
        protected static T _instance;

        public static T Instance
        {
            get{
                return _instance;
            }
        }

        protected bool SetInstance(T i)
        {
            if(_instance != null && _instance != i)
            {
                return false;
            }
            _instance = i;
            return true;
        }
    }
}
