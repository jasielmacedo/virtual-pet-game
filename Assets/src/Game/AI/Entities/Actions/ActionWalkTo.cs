using UnityEngine;
using Game.AI.Entities.Actions.States;
using Game.Manager;
using Game.Actors;

namespace Game.AI.Entities.Actions
{
    public class ActionWalkTo : ActionIdle
    {
        public override void EnterAction()
        {
            if (m_stateMachine.Params.ContainsKey("destination"))
            {
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

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                if (m_initialInterruptionState != m_interruptible)
                    m_interruptible = m_initialInterruptionState;

                funProperty.value -= 10f * deltaTime;
                hungerProperty.value += 20f * deltaTime;
                energyProperty.value += 5f * deltaTime;
                Owner.SetCompleteAction(this.Id);
            }
        }
    }
}
