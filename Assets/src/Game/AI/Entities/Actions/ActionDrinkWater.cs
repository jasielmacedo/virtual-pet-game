using UnityEngine;
using Game.AI.Entities.Actions.States;
using Game.Manager;
using Game.Actors;

namespace Game.AI.Entities.Actions
{
    public class ActionDrinkWater : ActionIdle
    {
        protected override float EvaluateExternalConsiderations()
        {
            return HomeInstance.Instance.CountByType(InteractiveObject.EInteractiveType.DRINK) > 0 ? 1f * externalConsiderationWeight : -1f;
        }

        InteractiveObject place;

        protected override void OnStateMachineStateChanged()
        {
            base.OnStateMachineStateChanged();

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                localCharacter.Interact(place);
                place.StartUsing();
            }
        }

        public override void EnterAction()
        {
            place = HomeInstance.Instance.GetRandomObject(InteractiveObject.EInteractiveType.DRINK);
            if (place != null)
            {
                m_stateMachine.Params["destination"] = place.GetActorLocation;
            }
            m_stateMachine.ChangeState<StateMoveTo>();
            durationDrinking = 3f;
        }

        float durationDrinking = 3f;

        public override void Tick(float deltaTime)
        {
            m_stateMachine.Tick(deltaTime);

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                if (m_initialInterruptionState != m_interruptible)
                    m_interruptible = m_initialInterruptionState;

                thirstyProperty.value -= 40f * deltaTime;
                durationDrinking -= deltaTime;

                if (durationDrinking <= 0)
                    Owner.SetCompleteAction(this.Id);
            }
        }

        public override void ExitAction()
        {
            base.ExitAction();
            if (place != null)
            {
                place.StopUsing();
            }
        }
    }
}