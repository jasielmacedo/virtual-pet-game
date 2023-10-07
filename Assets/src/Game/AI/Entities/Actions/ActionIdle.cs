using UnityEngine;
using Game.AI.FSM;
using Game.AI.UtilityAI;
using Game.AI.UtilityAI.Property;
using Game.AI.Entities.Actions.States;


namespace Game.AI.Entities.Actions
{
    public class ActionIdle : UAIAction
    {
        protected FSMStateMachine m_stateMachine;

        protected UAIPropertyBoundedFloat hungerProperty;
        protected UAIPropertyBoundedFloat thirstyProperty;
        protected UAIPropertyBoundedFloat hygieneProperty;
        protected UAIPropertyBoundedFloat energyProperty;
        protected UAIPropertyBoundedFloat funProperty;
        protected UAIPropertyBoundedFloat socialProperty;
        protected UAIPropertyBoundedFloat bladderProperty;

        protected bool m_initialInterruptionState;

        public override void Initialize(UAIAgent _owner)
        {
            base.Initialize(_owner);

            m_stateMachine = new FSMStateMachine();
            m_stateMachine.SetCurrentGameObject(_owner.gameObject);

            m_stateMachine.AddState<StateIdle>();
            m_stateMachine.AddState<StateMoveTo>();
            m_stateMachine.AddState<StateExecute>();

            m_stateMachine.SetInitialState<StateExecute>();
            m_stateMachine.OnStateChanged += OnStateMachineStateChanged;

            m_stateMachine.Initialize();



            hungerProperty = Owner.properties["hunger"] as UAIPropertyBoundedFloat;
            hygieneProperty = Owner.properties["hygiene"] as UAIPropertyBoundedFloat;
            energyProperty = Owner.properties["energy"] as UAIPropertyBoundedFloat;
            funProperty = Owner.properties["fun"] as UAIPropertyBoundedFloat;
            socialProperty = Owner.properties["social"] as UAIPropertyBoundedFloat;
            bladderProperty = Owner.properties["bladder"] as UAIPropertyBoundedFloat;
            thirstyProperty = Owner.properties["thirsty"] as UAIPropertyBoundedFloat;

            m_initialInterruptionState = m_interruptible;
        }

        protected virtual void OnStateMachineStateChanged()
        {
            if (m_stateMachine.IsCurrentState<StateMoveTo>())
            {
                m_interruptible = false;
                // m_localCharacter.Stand();
            }
            else if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                m_interruptible = m_initialInterruptionState;
                // if (place != null && place.PlaceType == Place.EPlaceType.Sit)
                //    m_localCharacter.Sit(place);
            }
        }

        public override void Tick(float deltaTime)
        {
            m_stateMachine.Tick(deltaTime);

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                energyProperty.value -= 0.1f * deltaTime;

                bladderProperty.value += 0.5f * deltaTime;
                funProperty.value -= 5f * deltaTime;
                hygieneProperty.value -= 0.5f * deltaTime;

                if (energyProperty.normalizedValue <= 0.1f)
                    Owner.SetCompleteAction(this.Id);
            }
        }

        public override void ExitAction()
        {
            m_stateMachine.GotoInitialState();

            //if (place != null && place.PlaceType == Place.EPlaceType.Sit)
            //{
            //    m_localCharacter.Stand();
            //}
        }
    }
}