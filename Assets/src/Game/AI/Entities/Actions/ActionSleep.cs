using UnityEngine;
using Game.AI.Entities.Actions.States;
using Game.Manager;
using Game.Actors;

namespace Game.AI.Entities.Actions
{
    public class ActionSleep : ActionIdle
    {
        InteractiveObject place;

        public override void EnterAction()
        {
            place = HomeInstance.Instance.GetRandomObject(InteractiveObject.EInteractiveType.SLEEP_PLACE);
            if (place != null)
            {
                m_stateMachine.Params["destination"] = place.GetActorLocation;
                m_stateMachine.ChangeState<StateMoveTo>();
            }else {
                // If no space is available. It'll sleep wherever it is.
                m_stateMachine.ChangeState<StateExecute>();
            }
        }

        public override void Tick(float deltaTime)
        {
            m_stateMachine.Tick(deltaTime);

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                if (m_initialInterruptionState != m_interruptible)
                    m_interruptible = m_initialInterruptionState;

                funProperty.value -= 10f * deltaTime;
                hungerProperty.value += 20f * deltaTime;
                energyProperty.value += 5f * deltaTime;

                if (funProperty.normalizedValue == 1f)
                    Owner.SetCompleteAction(this.Id);
            }
        }
    }
}
