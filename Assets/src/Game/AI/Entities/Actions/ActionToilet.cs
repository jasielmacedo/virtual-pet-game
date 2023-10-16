using UnityEngine;
using Simulation.Entities.Actions.States;
using Simulation.Manager;

namespace Simulation.Entities.Actions
{
    public class ActionToilet : ActionIdle
    {
        protected override float EvaluateExternalConsiderations()
        {
            return OfficeInstance.Instance.TotalToiletsAvailable > 0 ? 1f * externalConsiderationWeight : -1f;
        }

        protected override void OnStateMachineStateChanged()
        {
            base.OnStateMachineStateChanged();

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                place.StartUsing();
            }
        }

        public override void EnterAction()
        {
            place = OfficeInstance.Instance.getPlaceAvailable(OfficeInstance.EPlaceCategory.Toilets);
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
                hygieneProperty.value -= 5f * deltaTime;
                bladderProperty.value -= 20f * deltaTime;
                hungerProperty.value += 1f * deltaTime;

                if (bladderProperty.normalizedValue <= 0f)
                    Owner.SetCompleteAction(this.Id);
            }
        }

        public override void ExitAction()
        {
            base.ExitAction();
            if (place != null)
            {
                OfficeInstance.Instance.setPlaceAvailable(OfficeInstance.EPlaceCategory.Toilets, place);
                place.StopUsing();
            }
        }
    }
}
