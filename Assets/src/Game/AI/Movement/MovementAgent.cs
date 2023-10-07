using UnityEngine;

namespace Game.AI.Movement
{
    public abstract class MovementAgent
    {
        public Transform Owner
        {
            get { return _owner; }
        }
        protected Transform _owner = null;

        public MovementAgent(Transform currentOwner)
        {
            _owner = currentOwner;
        }

        public virtual EMovementStatus MovementStatus
        {
            get { return _movementStatus; }
        }
        protected EMovementStatus _movementStatus = EMovementStatus.invalid;


        protected virtual void SetMovementStatus(EMovementStatus newStatus)
        {
            if (newStatus == _movementStatus)
                return;

            _movementStatus = newStatus;
        }

        public virtual bool InMovement { get; }
        public virtual float Velocity { get; }


        public Vector3 LastDestination
        {
            get { return _lastDestination; }
        }
        protected Vector3 _lastDestination;

        public abstract EMovementStatus GoToDestination(Vector3 destination);


        public abstract void StopMovement();
        public abstract void SetPause(bool isPaused);

        public virtual void SetStoppingDistance(float distance) { }

        public enum EMovementStatus
        {
            empty,
            valid,
            partial,
            invalid
        }
    }
}
