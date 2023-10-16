using UnityEngine;
using Simulation.Entities.Actions.States;
using Simulation.Manager;

namespace Simulation.Entities.Actions
{
    public class ActionDrinkWater : ActionIdle
    {
        protected override float EvaluateExternalConsiderations()
        {
            return OfficeInstance.Instance.TotalWaterPlacesAvailable > 0 ? 1f * externalConsiderationWeight : -1f;
        }

        protected override void OnStateMachineStateChanged()
        {
            base.OnStateMachineStateChanged();

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                m_localCharacter.Interact(place, 1);
            }
        }

        public override void EnterAction()
        {
            place = OfficeInstance.Instance.getPlaceAvailable(OfficeInstance.EPlaceCategory.Water);
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

                bladderProperty.value += 8f * deltaTime;
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
                OfficeInstance.Instance.setPlaceAvailable(OfficeInstance.EPlaceCategory.Coffee, place);
            }
        }
    }
}