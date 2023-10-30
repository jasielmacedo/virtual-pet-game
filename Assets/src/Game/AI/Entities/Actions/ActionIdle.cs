using UnityEngine;
using Game.AI.FSM;
using Game.AI.UtilityAI;
using Game.AI.UtilityAI.Property;
using Game.AI.Entities.Actions.States;
using UnityEngine.AI;
using Game.AI.Movement;


namespace Game.AI.Entities.Actions
{
    public class ActionIdle : UAIAction
    {
        protected FSMStateMachine m_stateMachine;

        protected UAIPropertyBoundedFloat hungerProperty;
        protected UAIPropertyBoundedFloat thirstProperty;
        protected UAIPropertyBoundedFloat energyProperty;
        protected UAIPropertyBoundedFloat funProperty;

        protected bool m_initialInterruptionState;
        protected Cat localCharacter;
        protected Animator OwnerAnimator;
        protected NavMeshAgent OwnerMovementController;

        public override void Initialize(UAIAgent _owner)
        {
            base.Initialize(_owner);

            m_stateMachine = new FSMStateMachine();
            m_stateMachine.SetCurrentGameObject(_owner.gameObject);

            localCharacter = _owner.GetComponent<Cat>();

            m_stateMachine.AddState<StateIdle>();
            m_stateMachine.AddState<StateMoveTo>();
            m_stateMachine.AddState<StateExecute>();

            m_stateMachine.SetInitialState<StateExecute>();
            m_stateMachine.OnStateChanged += () =>
            {
                if (Owner.IsCurrentAction(Id))
                    OnStateMachineStateChanged();
            };

            m_stateMachine.Initialize();

            hungerProperty = Owner.properties["hunger"] as UAIPropertyBoundedFloat;
            energyProperty = Owner.properties["energy"] as UAIPropertyBoundedFloat;
            funProperty = Owner.properties["fun"] as UAIPropertyBoundedFloat;
            thirstProperty = Owner.properties["thirst"] as UAIPropertyBoundedFloat;

            m_initialInterruptionState = m_interruptible;

            OwnerAnimator = Owner.GetComponentInChildren<Animator>();
            OwnerMovementController = Owner.GetComponent<NavMeshAgent>();
        }

        protected virtual void OnStateMachineStateChanged()
        {
            if (m_stateMachine.IsCurrentState<StateMoveTo>())
            {
                m_interruptible = false;
            }
            else if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                m_interruptible = m_initialInterruptionState;
            }
        }

        public override void Tick(float deltaTime)
        {
            m_stateMachine.Tick(deltaTime);

            if (m_stateMachine.IsCurrentState<StateExecute>())
            {
                m_stateMachine.Params["interaction"] = 0;

                energyProperty.value -= 0.5f * deltaTime;
                funProperty.value -= 0.3f * deltaTime;
                thirstProperty.value += 0.2f * deltaTime;

                if (energyProperty.normalizedValue <= 0.1f)
                    Owner.SetCompleteAction(this.Id);
            }
        }

        public override void ExitAction()
        {
            m_stateMachine.Params.Clear();
            m_stateMachine.GotoInitialState();
            m_stateMachine.Tick(Time.deltaTime);
        }
    }
}