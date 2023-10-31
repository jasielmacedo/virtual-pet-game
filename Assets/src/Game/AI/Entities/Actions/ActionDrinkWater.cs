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
            return HomeInstance.Instance.CountByType(InteractiveObject.EInteractiveType.DRINK) > 0 && thirstProperty.normalizedValue > 0.1f ? 1f * externalConsiderationWeight : -1f;
        }

        InteractiveObject place;

        protected override void OnStateMachineStateChanged()
        {
            base.OnStateMachineStateChanged();

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                localCharacter.Interact(place);
                place.StartUsing();
                OwnerAnimator.Play("Drink", 0);
            }
        }

        public override void EnterAction()
        {
            if(Owner.Params.ContainsKey("drink")){
                place = Owner.Params["drink"] as InteractiveObject;
                Owner.Params.Remove("drink");
            }else{
                place = HomeInstance.Instance.GetRandomObject(InteractiveObject.EInteractiveType.DRINK);
            }
            
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

                thirstProperty.value -= 6f * deltaTime;
                funProperty.value -= 0.1f * deltaTime;

                if (thirstProperty.normalizedValue <= 0)
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
            OwnerAnimator.Play("Stand", 0);
        }
    }
}