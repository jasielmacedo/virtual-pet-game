using UnityEngine;
using Core.Utils;
using Game.AI.Movement;

namespace Game.AI.FSM.BasicStates
{
    public class MoveState : Game.AI.FSM.FSMState
    {
        public override string Id { get { return "move"; } }

        public MovementAgent Movement
        {
            get { return _movementAgent; }
        }
        MovementAgent _movementAgent;

        public override void Initialize()
        {
            base.Initialize();

            _movementAgent = new NavMeshMovement(Owner.currentTransform);
        }

        public bool MovementPaused
        {
            get { return _walkPaused; }
        }

        protected bool _walkPaused = false;

        public void PauseMovement() { _walkPaused = true; }
        public void ResumeeMovement() { _walkPaused = false; }

        public override void EnterState()
        {
            _movementAgent.StopMovement();
            _lastValidDestinaton = Owner.WorldDestination;
        }

        Vector3 _lastValidDestinaton;
        Vector3 _lastReceivedDestination;

        public override void Tick(float deltaTime)
        {
            if (!IsMoving())
            {
                if (_walkPaused)
                    return;

                if (DistanceSqr < 2f)
                {
                    _movementAgent.StopMovement();
                    Owner.GotoInitialState();
                    return;
                }

                _lastReceivedDestination = Owner.WorldDestination;
                MovementAgent.EMovementStatus status = _movementAgent.GoToDestination(_lastReceivedDestination);
                _lastValidDestinaton = _movementAgent.LastDestination;

                if (status == MovementAgent.EMovementStatus.partial || status == MovementAgent.EMovementStatus.invalid)
                {
                    // to validate this behaviour you can check the movement status outside of the state
                    _movementAgent.StopMovement();
                    Owner.GotoInitialState();
                    return;
                }
            }
            else
            {
                if (_walkPaused)
                {
                    _movementAgent.SetPause(true);
                    return;
                }


                if (Owner.WorldDestination != _lastReceivedDestination)
                {
                    _lastReceivedDestination = Owner.WorldDestination;
                    _movementAgent.GoToDestination(_lastReceivedDestination);
                    _lastValidDestinaton = _movementAgent.LastDestination;
                }
            }
        }

        public float DistanceSqr
        {
            get
            {
                Vector3 distance = _lastValidDestinaton - Owner.currentTransform.position;
                return distance.sqrMagnitude;
            }
        }

        public bool IsMoving()
        {
            return _movementAgent.InMovement;
        }

        public override void ExitState()
        {
            _movementAgent.StopMovement();
        }
    }
}
