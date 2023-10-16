using UnityEngine;
using Simulation.Entities.Actions.States;
using Simulation.Manager;

namespace Simulation.Entities.Actions
{
    public class ActionGoToHome : ActionIdle
    {
        public override void EnterAction()
        {
            place = OfficeInstance.Instance.Entrance;
            if (place != null)
            {
                m_stateMachine.Params["destination"] = place.GetActorLocation;
            }
            m_stateMachine.ChangeState<StateMoveTo>();
            waitingTime = Random.Range(0.0f, 4.0f);
        }

        float waitingTime = 0;

        public override void Tick(float deltaTime)
        {
            m_stateMachine.Tick(deltaTime);

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                hungerProperty.value -= 5f * deltaTime;
                bladderProperty.value -= 1f * deltaTime;
                energyProperty.value += 1f * deltaTime;

                if (OfficeInstance.Instance.IsWorkingHour)
                {
                    if (waitingTime < 0f)
                        Owner.SetCompleteAction(this.Id);
                    else
                        waitingTime -= deltaTime;
                }
            }
        }

        public override void ExitAction()
        {
            base.ExitAction();
        }
    }
}