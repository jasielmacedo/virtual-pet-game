using UnityEngine;
using Game.AI.Entities.Actions.States;
using Game.Manager;
using Game.Actors;

namespace Game.AI.Entities.Actions
{
    public class ActionSleep : ActionIdle
    {
        public override void EnterAction()
        {
            GameObject[] availablePlaces = GameObject.FindGameObjectsWithTag("sleep");

            if (availablePlaces.Length > 0)
            {
                Transform randomPlace = availablePlaces[Random.Range(0, availablePlaces.Length)].transform;
                m_stateMachine.Params["destination"] = randomPlace.position;
                m_stateMachine.ChangeState<StateMoveTo>();
            }
            else
            {
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

                funProperty.value -= 0.5f * deltaTime;
                hungerProperty.value += 0.8f * deltaTime;
                energyProperty.value += 4f * deltaTime;
                thirstProperty.value += 0.1f * deltaTime;

                if (funProperty.normalizedValue == 1f)
                    Owner.SetCompleteAction(this.Id);
            }
        }
    }
}
