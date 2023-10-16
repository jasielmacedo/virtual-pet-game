using UnityEngine;
using Simulation.Entities.Actions.States;
using Simulation.Manager;

namespace Simulation.Entities.Actions
{
    public class ActionWork : ActionIdle
    {
        protected override float EvaluateExternalConsiderations()
        {
            if (getConditionToNotWork() || (Owner as SimCharacter).CurrentWorkStation == null)
                return -100f;

            return 1f * externalConsiderationWeight;
        }

        public override void EnterAction()
        {
            SimCharacter character = (Owner as SimCharacter);
            place = character.CurrentWorkStation;
            if (place != null)
            {
                m_stateMachine.Params["destination"] = place.GetActorLocation;
            }
            m_stateMachine.ChangeState<StateMoveTo>();
            m_interruptible = false;
        }

        private bool getConditionToNotWork()
        {
            return !OfficeInstance.Instance.IsWorkingHour || hungerProperty.normalizedValue > 0.9f || bladderProperty.normalizedValue > 0.8f || thirstyProperty.normalizedValue > 0.95f;
        }


        public override void Tick(float deltaTime)
        {
            m_stateMachine.Tick(deltaTime);

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                bladderProperty.value -= 0.1f * deltaTime;
                hungerProperty.value += 0.5f * deltaTime;
                energyProperty.value -= 0.6f * deltaTime;

                if (getConditionToNotWork())
                {
                    if (!OfficeInstance.Instance.IsWorkingHour)
                        Owner.DemandAction("home");
                    Owner.SetCompleteAction(this.Id);
                    return;
                }
                else if (funProperty.normalizedValue < 0.1)
                {
                    Owner.DemandAction("relax");
                    Owner.SetCompleteAction(this.Id);
                    return;
                }

            }
        }

        public override void ExitAction()
        {
            base.ExitAction();
        }
    }
}