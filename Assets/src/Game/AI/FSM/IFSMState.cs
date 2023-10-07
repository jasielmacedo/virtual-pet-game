using UnityEngine;

namespace Game.AI.FSM
{
    interface IFSMState
    {
        string Id { get; }
        bool isActive { get; }

        void Initialize();

        void EnterState();
        void ExitState();

        void Tick(float deltaTime);
        void PhysicsTick(float fixedDeltaTime);
        void PostTick(float deltaTime);

        void OnCollisionEnter(Collision collision);
        void OnCollisionStay(Collision collision);
        void OnCollisionExit(Collision collision);

        void OnTriggerEnter(Collider collider);
        void OnTriggerStay(Collider collider);
        void OnTriggerExit(Collider collider);

        void OnAnimatorIK(int layerIndex);
    }
}
