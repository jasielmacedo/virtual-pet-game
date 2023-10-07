using UnityEngine;

namespace Game.AI.FSM
{
    public interface IFSMStateMachine
    {
        void Initialize();

        System.Collections.Hashtable Params { get; set; }

        GameObject currentGameObject();
        Transform currentTransform { get; }

        Vector3 WorldDestination { get; }
        void SetWorldDestination(Vector3 destinatation);

        void Tick(float deltaTime);
        void PhysicsTick(float fixedDeltaTime);
        void PostTick(float deltaTime);

        void SetInitialState(string id);
        void SetInitialState<T>() where T : FSMState;
        void SetInitialState(System.Type T);

        void ChangeState<T>() where T : FSMState;
        void ChangeState(System.Type T);

        void GotoInitialState();

        bool IsCurrentState(string id);
        bool IsCurrentState<T>() where T : FSMState;
        bool IsCurrentState(System.Type T);

        T CurrentState<T>() where T : FSMState;
        T GetState<T>() where T : FSMState;

        void AddState<T>() where T : FSMState, new();
        void AddState(System.Type T);

        void RemoveState<T>() where T : FSMState;
        void RemoveState(System.Type T);

        bool ContainsState<T>() where T : FSMState;
        bool ContainsState(System.Type T);

        void OnCollisionEnter(Collision collision);
        void OnCollisionStay(Collision collision);
        void OnCollisionExit(Collision collision);

        void OnTriggerEnter(Collider collider);
        void OnTriggerStay(Collider collider);
        void OnTriggerExit(Collider collider);

        void OnAnimatorIK(int layerIndex);
    }
}
