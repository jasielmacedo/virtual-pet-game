using UnityEngine;
using Simulation.Entities.Actions.States;
using Simulation.Manager;

namespace Simulation.Entities.Actions
{
    public class ActionLunch : ActionIdle
    {
        protected override float EvaluateExternalConsiderations()
        {
            return OfficeInstance.Instance.TotalLunchPlacesAvailable > 0 ? 1f * externalConsiderationWeight : -1f;
        }

        protected override void OnStateMachineStateChanged()
        {
            base.OnStateMachineStateChanged();

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                m_localCharacter.Interact(place, 2);
            }
        }

        public override void EnterAction()
        {
            place = OfficeInstance.Instance.getPlaceAvailable(OfficeInstance.EPlaceCategory.Lunch);
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
                hungerProperty.value -= 5f * deltaTime;
                bladderProperty.value += 2f * deltaTime;
                energyProperty.value -= 0.5f * deltaTime;

                if (hungerProperty.normalizedValue == 0f)
                    Owner.SetCompleteAction(this.Id);
            }
        }

        public override void ExitAction()
        {
            base.ExitAction();
        }
    }
}