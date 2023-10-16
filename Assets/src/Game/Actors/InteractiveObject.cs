using UnityEngine;
using Core.Game;
using Game.Manager;

namespace Game.Actors
{
    public class InteractiveObject : Actor
    {
        public enum EInteractiveType {
            FOOD,
            DRINK,
            TOY,
            SLEEP_PLACE,
        }

        public EInteractiveType InteractiveType => _interactiveType;

        [Header("Interactive")]
        [SerializeField] private EInteractiveType _interactiveType;


        protected virtual void Start(){
            HomeInstance.Instance.RegisterObject(this);
        }

        protected override void OnDestroy()
        {
            HomeInstance.Instance?.UnregisterObject(this);
        }

        public virtual void StartUsing(){}
        public virtual void StopUsing(){}
    }
}