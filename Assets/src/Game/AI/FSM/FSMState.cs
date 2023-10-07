using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI.FSM
{
    /// <summary>
    /// FSMState is the base class from which every FSM State script derives.
    /// </summary>
    [System.Serializable]
    public abstract class FSMState : IFSMState
    {
        public abstract string Id { get; }

        public IFSMStateMachine Owner { get; internal set; }


        public bool isActive { get { return Owner.IsCurrentState(GetType()); } }

        /// <summary>
        /// Initialize this State. You can use Like Start() or Awake() method.
        /// This is called during the initialization of the state machine
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// This method is called when this state became the current active state
        /// </summary>
        public virtual void EnterState() { }

        /// <summary>
        /// This method is called when this state is no longer the current active state
        /// </summary>
        public virtual void ExitState() { }

        /// <summary>
        /// Useful to manipulate Animator inside the State
        /// </summary>
        public virtual void OnAnimatorIK(int layerIndex) { }

        public virtual void OnCollisionEnter(Collision collision) { }
        public virtual void OnCollisionExit(Collision collision) { }
        public virtual void OnCollisionStay(Collision collision) { }

        public virtual void OnTriggerEnter(Collider collider) { }
        public virtual void OnTriggerExit(Collider collider) { }
        public virtual void OnTriggerStay(Collider collider) { }

        /// <summary>
        /// Used to update the logic in runtime
        /// This method is only called if this state is the current active state
        /// </summary>
        /// <param name="deltaTime">float Delta time. Time.deltaTime</param>
        public virtual void Tick(float deltaTime) { }

        /// <summary>
        /// Physics Tick is used to apply physics logic inside the state. Use on FixedUpdate
        /// This method is only called if this state is the current active state
        /// </summary>
        /// <param name="fixedDeltaTime">Fixed delta time. Time.fixedDeltaTime</param>
        public virtual void PhysicsTick(float fixedDeltaTime) { }

        /// <summary>
        /// Called on LateUpdate if needed
        /// This method is only called if this state is the current active state
        /// </summary>
        /// <param name="deltaTime">Delta time. Time.deltaTime</param>
        public virtual void PostTick(float deltaTime) { }
    }
}
