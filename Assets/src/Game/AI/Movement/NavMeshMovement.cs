using UnityEngine;
using UnityEngine.AI;
using Core.Utils;

namespace Game.AI.Movement
{
    public class NavMeshMovement : MovementAgent
    {
        public NavMeshAgent NavAgent
        {
            get { return _navMeshAgent; }
        }

        protected NavMeshAgent _navMeshAgent;

        public NavMeshMovement(Transform _owner) : base(_owner)
        {
            _navMeshAgent = _owner?.GetComponent<NavMeshAgent>();
            if (!_navMeshAgent)
                throw new UnityException("NavMeshMovement: NavMeshAgent is required");
        }

        public override bool InMovement
        {
            get
            {
                return _navMeshAgent.pathPending || _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance || _navMeshAgent.velocity != Vector3.zero;
            }
        }

        public override float Velocity
        {
            get
            {
                return _navMeshAgent.velocity.sqrMagnitude;
            }
        }

        public NavMeshPath PathTo => pathReutilizable;
        NavMeshPath pathReutilizable = new NavMeshPath();

        public virtual EMovementStatus GoToDestination(Vector3 destination, bool _insideNavMesh)
        {
            _lastDestination = destination;

            if (_insideNavMesh)
                _lastDestination = _navMeshAgent.NearestValidDestination(destination, pathReutilizable);

            _navMeshAgent.destination = _lastDestination;

            if (_navMeshAgent.isStopped)
                _navMeshAgent.isStopped = false;

            EMovementStatus newStatus = EMovementStatus.valid;

            switch (_navMeshAgent.path.status)
            {
                case NavMeshPathStatus.PathPartial: newStatus = EMovementStatus.partial; break;
                case NavMeshPathStatus.PathInvalid: newStatus = EMovementStatus.invalid; break;
            }

            SetMovementStatus(newStatus);
            return newStatus;
        }

        public override EMovementStatus GoToDestination(Vector3 destination)
        {
            return GoToDestination(destination, true);
        }

        public override void SetStoppingDistance(float distance)
        {
            _navMeshAgent.stoppingDistance = distance;
        }

        public override void StopMovement()
        {
            if (_navMeshAgent.enabled)
            {
                _navMeshAgent.isStopped = true;
                _navMeshAgent.ResetPath();
            }
        }

        public override void SetPause(bool isPaused)
        {
            _navMeshAgent.isStopped = isPaused;
        }
    }
}
