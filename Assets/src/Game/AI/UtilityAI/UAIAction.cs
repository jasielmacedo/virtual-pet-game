using UnityEngine;
using System.Collections.Generic;
using Core.Game;

namespace Game.AI.UtilityAI
{
    [System.Serializable]
    public abstract class UAIAction : Neuron
    {
        public string Id { get { return m_id; } set { m_id = value; } }
        [SerializeField] protected string m_id;

        public int PriorityLevel { get { return m_priorityLevel; } set { m_priorityLevel = value; } }
        [Tooltip("Lowest Values is high priority")]
        [SerializeField] protected int m_priorityLevel;

        public bool Interruptible { get { return m_interruptible; } set { m_interruptible = value; } }
        [SerializeField] protected bool m_interruptible;

        public UAIAgent Owner
        {
            get { return m_owner; }
        }
        private UAIAgent m_owner;

        [HideInInspector]
        public List<UAIConsideration> considerations = new List<UAIConsideration>();

        public virtual float Score { get { return m_actionScore; } set { m_actionScore = value; } }
        protected float m_actionScore;

        /// <summary>
        /// Initialize the Action. Called on Start()
        /// </summary>
        /// <param name="_owner">UAIAgent Owner.</param>
        public virtual void Initialize(UAIAgent _owner)
        {
            m_owner = _owner;
        }


        /// <summary>
        /// Set true when using external considerations
        /// </summary>
        [SerializeField] protected bool considerExternalConsiderationsWhenEvaluate = false;
        [SerializeField] protected float externalConsiderationWeight = 1f;

        /// <summary>
        /// Used to consider external factors when calculationg considerations (0f-1f)
        /// </summary>
        protected virtual float EvaluateExternalConsiderations()
        {
            return 0f;
        }

        public void EnableConsideration(string propertyName)
        {
            for (int i = 0; i < considerations.Count; i++)
            {
                if (considerations[i].property.id == propertyName)
                    considerations[i].enabled = true;
            }
        }

        public void DisableConsideration(string propertyName)
        {
            for (int i = 0; i < considerations.Count; i++)
            {
                if (considerations[i].property.id == propertyName)
                    considerations[i].enabled = false;
            }
        }

        /// <summary>
        /// Evaluate if this action can be used in the current moment
        /// </summary>
        public void EvaluateAction()
        {
            m_actionScore = EvaluateExternalConsiderations();
            int enabledConsiderationsCount = considerExternalConsiderationsWhenEvaluate ? 1 : 0;

            for (int i = 0; i < considerations.Count; i++)
            {

                if (considerations[i].enabled)
                {
                    m_actionScore += considerations[i].utilityScore * considerations[i].weight;
                    enabledConsiderationsCount++;
                }
            }

            if (enabledConsiderationsCount > 0 && m_actionScore > 0f)
                m_actionScore = m_actionScore / enabledConsiderationsCount;
        }

        public virtual void EnterAction() { }
        public virtual void ExitAction() { }

        /// <summary>
        /// Used to update the logic in runtime
        /// </summary>
        /// <param name="deltaTime">float Delta time. Time.deltaTime</param>
        public virtual void Tick(float deltaTime) { }

        /// <summary>
        /// Physics Tick is used to apply physics logic inside the Action. Use on FixedUpdate
        /// </summary>
        /// <param name="fixedDeltaTime">Fixed delta time. Time.fixedDeltaTime</param>
        public virtual void PhysicsTick(float fixedDeltaTime) { }

        /// <summary>
        /// Called on LateUpdate if needed
        /// </summary>
        /// <param name="deltaTime">Delta time. Time.deltaTime</param>
        public virtual void PostTick(float deltaTime) { }
    }
}

