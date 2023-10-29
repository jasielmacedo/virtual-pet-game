using UnityEngine;
using Game.AI.Entities.Actions.States;
using Game.Manager;
using Game.Actors;

namespace Game.AI.Entities.Actions
{
    public class ActionPlay : ActionIdle
    {
        protected override float EvaluateExternalConsiderations()
        {
            return HomeInstance.Instance.CountByType(InteractiveObject.EInteractiveType.TOY) > 0 ? 1f * externalConsiderationWeight : -1f;
        }

        InteractiveObject place;

        protected override void OnStateMachineStateChanged()
        {
            base.OnStateMachineStateChanged();

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                localCharacter.Interact(place);
                place.StartUsing();
                OwnerAnimator.Play("Surprise", 0);
                startExecutingActionTimer = 0;
            }
        }

        float startExecutingActionTimer = 0f;
        float initialMovementSpeed = 0f;

        public override void EnterAction()
        {
            place = HomeInstance.Instance.GetRandomObject(InteractiveObject.EInteractiveType.TOY);
            initialMovementSpeed = OwnerMovementController.speed;

            if (place != null)
            {
                m_stateMachine.Params["destination"] = place.GetActorLocation;
                OwnerMovementController.speed = 3f;
                m_stateMachine.ChangeState<StateMoveTo>();
            }
            else
            {
                m_stateMachine.IsCurrentState<StateExecute>();
            }

        }

        public override void Tick(float deltaTime)
        {
            m_stateMachine.Tick(deltaTime);

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                if (m_initialInterruptionState != m_interruptible)
                    m_interruptible = m_initialInterruptionState;

                energyProperty.value -= 1f * deltaTime;
                thirstProperty.value += 0.2f * deltaTime;
                hungerProperty.value += 0.3f * deltaTime;
                funProperty.value += 15f * deltaTime;

                startExecutingActionTimer += deltaTime;

                if (startExecutingActionTimer >= 3.8)
                {
                    Owner.SetCompleteAction(this.Id);
                }
            }
        }

        public override void ExitAction()
        {
            base.ExitAction();
            if (place != null)
            {
                place.StopUsing();
            }
            OwnerMovementController.speed = initialMovementSpeed;
        }
    }
}