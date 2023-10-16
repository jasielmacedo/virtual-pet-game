using UnityEngine;
using Simulation.Entities.Actions.States;
using Simulation.Manager;

namespace Simulation.Entities.Actions
{
    public class ActionRelax : ActionIdle
    {
        protected override float EvaluateExternalConsiderations()
        {
            return OfficeInstance.Instance.TotalToiletsAvailable > 0 ? 1f * externalConsiderationWeight : -1f;
        }

        public override void EnterAction()
        {
            place = OfficeInstance.Instance.getPlaceAvailable(OfficeInstance.EPlaceCategory.Relax);
            if (place != null)
            {
                m_stateMachine.Params["destination"] = place.GetActorLocation;
            }
            m_stateMachine.ChangeState<StateMoveTo>();
        }

        public override void Tick(float deltaTime)
        {
            m_stateMachine.Tick(deltaTime);

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                if (m_initialInterruptionState != m_interruptible)
                    m_interruptible = m_initialInterruptionState;

                funProperty.value += 30f * deltaTime;
                hungerProperty.value += 20f * deltaTime;
                energyProperty.value -= 5f * deltaTime;

                if (funProperty.normalizedValue == 1f)
                    Owner.SetCompleteAction(this.Id);
            }
        }

        public override void ExitAction()
        {
            base.ExitAction();
            if (place != null)
            {
                OfficeInstance.Instance.setPlaceAvailable(OfficeInstance.EPlaceCategory.Relax, place);
            }
        }
    }
}
