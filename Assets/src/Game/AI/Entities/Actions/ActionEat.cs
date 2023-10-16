using UnityEngine;
using Game.AI.Entities.Actions.States;
using Game.Manager;
using Game.Actors;

namespace Game.AI.Entities.Actions
{
    public class ActionEat : ActionIdle
    {
        protected override float EvaluateExternalConsiderations()
        {
            return HomeInstance.Instance.CountByType(InteractiveObject.EInteractiveType.FOOD) > 0 ? 1f * externalConsiderationWeight : -1f;
        }

        InteractiveObject objectOfInterest;

        protected override void OnStateMachineStateChanged()
        {
            base.OnStateMachineStateChanged();

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                localCharacter.Interact(objectOfInterest);
                objectOfInterest.StartUsing();
            }
        }

        public override void EnterAction()
        {
            objectOfInterest = HomeInstance.Instance.GetRandomObject(InteractiveObject.EInteractiveType.FOOD);
            if (objectOfInterest != null)
            {
                m_stateMachine.Params["destination"] = objectOfInterest.GetActorLocation;
            }
            m_stateMachine.ChangeState<StateMoveTo>();
        }

        public override void Tick(float deltaTime)
        {
            m_stateMachine.Tick(deltaTime);

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                hungerProperty.value -= 5f * deltaTime;
                energyProperty.value -= 0.5f * deltaTime;

                if (hungerProperty.normalizedValue == 0f)
                    Owner.SetCompleteAction(this.Id);
            }
        }

        public override void ExitAction()
        {
            base.ExitAction();
            objectOfInterest.StopUsing();
        }
    }
}