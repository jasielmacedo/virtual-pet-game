using UnityEngine;
using Game.AI.Entities.Actions.States;
using Game.Manager;
using Game.Actors;

namespace Game.AI.Entities.Actions
{
    public class ActionWalkTo : ActionIdle
    {
        float initialMovementSpeed = 0f;

        public override void EnterAction()
        {
            initialMovementSpeed = OwnerMovementController.speed;
            OwnerAnimator.Play("Stand", 0);

            if (Owner.Params.ContainsKey("destination"))
            {
                m_stateMachine.Params["destination"] = Owner.Params["destination"];
                OwnerMovementController.speed = 3;
                m_stateMachine.ChangeState<StateMoveTo>();
            }
            else
            {
                Owner.SetCompleteAction(this.Id);
            }
        }

        public override void Tick(float deltaTime)
        {
            m_stateMachine.Tick(deltaTime);

            if (m_stateMachine.IsCurrentState<StateMoveTo>())
            {
                funProperty.value -= 10f * deltaTime;
                hungerProperty.value += 20f * deltaTime;
                energyProperty.value += 5f * deltaTime;

            }
            else if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                Owner.SetCompleteAction(Id);
            }
        }


        public override void ExitAction()
        {
            base.ExitAction();
            OwnerMovementController.speed = initialMovementSpeed;
            OwnerAnimator.Play("Stand", 0);
        }
    }
}
