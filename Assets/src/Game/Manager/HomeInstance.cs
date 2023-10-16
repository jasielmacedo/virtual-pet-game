using UnityEngine;
using System.Linq;
using Core.Game;
using Game.Actors;
using System.Collections.Generic;

namespace Game.Manager
{
    public class HomeInstance : Singleton<HomeInstance>
    {
        public delegate void OnObjectRegistrationHandler(InteractiveObject obj);

        public event OnObjectRegistrationHandler OnObjectRegistered;
        public event OnObjectRegistrationHandler OnObjectUnregistered;

        private Dictionary<InteractiveObject.EInteractiveType, List<InteractiveObject>> registeredObjects = new();

        protected void OnEnable()
        {
            if (!SetInstance(this))
            {
                Destroy(this);
                return;
            }
        }
        
        public int CountByType(InteractiveObject.EInteractiveType type){
            return registeredObjects.ContainsKey(type) ? registeredObjects[type].Count: 0;
        }

        public InteractiveObject GetRandomObject(InteractiveObject.EInteractiveType type){
            if(!registeredObjects.ContainsKey(type) || registeredObjects[type].Count == 0) return null;

            return registeredObjects[type][Random.Range(0, registeredObjects[type].Count)];
        }

        public void RegisterObject(InteractiveObject obj){
            if(!registeredObjects.ContainsKey(obj.InteractiveType))
                registeredObjects.Add(obj.InteractiveType, new List<InteractiveObject>());
            else if(registeredObjects[obj.InteractiveType].Contains(obj))
                return;

            registeredObjects[obj.InteractiveType].Add(obj);
            OnObjectRegistered?.Invoke(obj);
        }

        public void UnregisterObject(InteractiveObject obj){
            if(!registeredObjects.ContainsKey(obj.InteractiveType) || !registeredObjects[obj.InteractiveType].Contains(obj))
                return;

            registeredObjects[obj.InteractiveType].Remove(obj);
            OnObjectUnregistered?.Invoke(obj);
        }
    }
}