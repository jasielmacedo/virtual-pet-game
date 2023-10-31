using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.AI.UtilityAI.Property;
using Core.Game;

namespace Game.AI.UtilityAI
{
    public class UAIAgent : Actor
    {
        public delegate void OnUAIAgentEventHandler();
        public event OnUAIAgentEventHandler OnAgentInitialized;

        public delegate void OnUAIAgentActionEventHandler(string action);
        public event OnUAIAgentActionEventHandler OnAgentActionCompleted;

        public delegate void OnReceivedEventFromActionHandler(string eventName, object eventValue);
        public event OnReceivedEventFromActionHandler OnReceivedEventFromAction;

        public float secondsBetweenEvaluations = 0.0f;

        [HideInInspector]
        public List<UAILinkedAction> linkedActions = new List<UAILinkedAction>();

        public Dictionary<string, UAIProperty> properties = new Dictionary<string, UAIProperty>();

        protected List<UAIAction> demandedActions = new List<UAIAction>();

        protected bool newAction;

        protected float secondsSinceLastEvaluation = 0.0f;
        protected UAIAction previousAction, topAction;

        protected float currentActionScore;
        protected bool isTiming = true;
        protected bool paused = false;

        protected static int agentEvaluationCounter = 0;
        public static int maxAgentEvaluations = 0;

        /// <summary>
        /// Used to send variables between the agent and the actions
        /// </summary>
        public Hashtable Params
        {
            get { return m_paramsForStates; }
            set { m_paramsForStates = value; }
        }
        protected Hashtable m_paramsForStates = new();

        protected override void Awake()
        {
            base.Awake();

            UAIProperty[] props = GetComponentsInChildren<UAIProperty>();
            if (props.Length > 0)
            {
                for (int i = 0; i < props.Length; i++)
                {
                    if (!properties.ContainsKey(props[i].id))
                    {
                        properties.Add(props[i].id, props[i]);
                    }
                }
            }
        }

        protected virtual void Start()
        {
            for (int i = 0; i < linkedActions.Count; i++)
            {
                if (linkedActions[i] != null)
                {
                    linkedActions[i].action.Initialize(this);
                }
            }

            if (OnAgentInitialized != null)
                OnAgentInitialized();
        }

        protected IEnumerator ResetEvaluation()
        {
            yield return new WaitForEndOfFrame();
            agentEvaluationCounter = 0;
        }

        protected bool CanEvaluate()
        {
            if (agentEvaluationCounter == 0)
            {
                StartCoroutine(ResetEvaluation());
            }

            agentEvaluationCounter++;
            return (maxAgentEvaluations <= 0 || agentEvaluationCounter <= maxAgentEvaluations);
        }

        public void EnableAction(string actionId)
        {
            for (int i = 0; i < linkedActions.Count; i++)
            {
                if (linkedActions[i].action.Id == actionId)
                {
                    linkedActions[i].actionEnabled = true;
                }
            }
        }

        public void DisableAction(string actionId)
        {
            for (int i = 0; i < linkedActions.Count; i++)
            {
                if (linkedActions[i].action.Id == actionId)
                {
                    linkedActions[i].actionEnabled = false;
                }
            }
        }

        public bool DemandAction(string actionId)
        {
            if(IsCurrentAction(actionId))
                return false;

            for (int i = 0; i < linkedActions.Count; i++)
            {
                if (linkedActions[i].action.Id == actionId)
                {
                    demandedActions.Add(linkedActions[i].action);
                    secondsSinceLastEvaluation = 0.0f;
                    return true;
                }
            }
            return false;
        }

        public bool removeDemandedAction(int index)
        {
            if (demandedActions.Count == 0)
                return false;

            demandedActions.RemoveAt(index);
            return true;
        }

        protected virtual void Update()
        {
            UpdateAI();
        }

        public void UpdateAI()
        {
            if (paused)
                return;

            if (topAction == null)
            {
                Evaluate();
                completeAction = false;
                secondsSinceLastEvaluation = secondsBetweenEvaluations;
            }

            topAction.Tick(Time.deltaTime);

            if (topAction.Interruptible || demandedActions.Count > 0)
            {
                secondsSinceLastEvaluation -= Time.deltaTime;
                if (secondsSinceLastEvaluation <= 0.0f)
                {
                    if (EvaluateInterruption())
                    {
                        completeAction = false;
                    }
                    secondsSinceLastEvaluation = secondsBetweenEvaluations;
                }
            }

            if (completeAction)
            {
                if (!CanEvaluate())
                {
                    Debug.Log("UpdateAI(): Cannot evaluate");
                    return;
                }
                Evaluate();
                completeAction = false;
                secondsSinceLastEvaluation = secondsBetweenEvaluations;
            }
        }

        public void PhysicsUpdateAI()
        {
            if (paused)
                return;

            if (topAction != null)
            {
                topAction.PhysicsTick(Time.fixedDeltaTime);
            }
        }

        bool completeAction = false;

        public void SetCompleteAction(string id)
        {
            if (topAction == null)
                return;

            if (topAction.Id == id)
            {
                completeAction = true;
                OnAgentActionCompleted?.Invoke(id);
            }
        }

        public void Pause()
        {
            if (!paused)
                paused = true;
            else
                paused = false;
        }

        public bool IsPaused()
        {
            return paused;
        }


        public float Evaluate()
        {

            if (topAction != null)
                previousAction = topAction;

            float topActionScore = 0.0f;

            for (int i = 0; i < linkedActions.Count; i++)
            {
                if (linkedActions[i].actionEnabled)
                {
                    linkedActions[i].action.EvaluateAction();
                    if (linkedActions[i].action.Score > topActionScore)
                    {
                        topAction = linkedActions[i].action;
                        topActionScore = linkedActions[i].action.Score;
                    }
                }
            }

            if (topAction != previousAction)
            {
                newAction = true;

                if (previousAction != null)
                    previousAction.ExitAction();

                topAction.EnterAction();
            }


            if (topAction.Interruptible)
                secondsSinceLastEvaluation = 0.0f;

            currentActionScore = topActionScore;

            return topActionScore;
        }

        public bool EvaluateInterruption()
        {

            int topActionPriority = topAction.PriorityLevel;
            float topActionScore = 0.0f;
            UAIAction topInterruption = topAction;
            bool validInterruption = false;

            if (demandedActions.Count > 0)
            {
                topInterruption = demandedActions[0];
                topActionScore = topAction.Score;
                demandedActions.RemoveAt(0);
                Debug.Log("Demanded action received");
                validInterruption = true;
            }
            else
            {

                for (int i = 0; i < linkedActions.Count; i++)
                {
                    if (linkedActions[i].actionEnabled)
                    {
                        if (linkedActions[i].action.PriorityLevel < topActionPriority)
                        {
                            linkedActions[i].action.EvaluateAction();
                            if (linkedActions[i].action.Score > currentActionScore && linkedActions[i].action.Score > topActionScore)
                            {
                                topInterruption = linkedActions[i].action;
                                topActionScore = linkedActions[i].action.Score;
                                validInterruption = true;
                            }
                        }
                    }
                }
            }

            if (validInterruption)
            {
                newAction = true;
                if (topAction != null)
                {
                    topAction.ExitAction();
                    SetCompleteAction(topAction.Id);
                }

                topAction = topInterruption;
                topAction.EnterAction();

                currentActionScore = topActionScore;

                if (topAction.Interruptible)
                    secondsSinceLastEvaluation = 0.0f;

                return true;
            }
            return false;
        }

        public UAIAction GetCurrentAction()
        {
            return topAction;
        }

        public bool IsCurrentAction(string id)
        {
            if (topAction == null)
                return false;

            return topAction.Id == id;
        }


        public void SetPropertyFloatValue(string id, float value)
        {
            if (properties.ContainsKey(id) && properties[id] is UAIPropertyBoundedFloat)
            {
                (properties[id] as UAIPropertyBoundedFloat).value = value;
            }
        }

        public void SetPropertyIntValue(string id, int value)
        {
            if (properties.ContainsKey(id) && properties[id] is UAIPropertyBoundedInt)
            {
                (properties[id] as UAIPropertyBoundedInt).value = value;
            }
        }

        public void SetPropertyDoubleValue(string id, double value)
        {
            if (properties.ContainsKey(id) && properties[id] is UAIPropertyBoundedDouble)
            {
                (properties[id] as UAIPropertyBoundedDouble).value = value;
            }
        }

        public void SetPropertyBooleanValue(string id, bool value)
        {
            if (properties.ContainsKey(id) && properties[id] is UAIPropertyBoolean)
            {
                (properties[id] as UAIPropertyBoolean).value = value;
            }
        }

        public void SendEventToOwner(string name, object value)
        {
            OnReceivedEventFromAction?.Invoke(name, value);
        }
    }

    [System.Serializable]
    public class UAILinkedAction
    {
        public UAIAction action;
        public bool actionEnabled = true;
    }
}

