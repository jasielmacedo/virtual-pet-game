using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Core.Game;
using System.Linq;

namespace Game.AI.FSM
{
    public abstract class FSMStateMachineMonoBehaviour : Actor, IFSMStateMachine
    {
        public delegate void OnStateMachineEventHandler();

        public event OnStateMachineEventHandler OnStateChanged;

        protected FSMState m_currentState { get; set; }
        protected FSMState m_nextState { get; set; }
        protected FSMState m_initialState { get; set; }

        /// <summary>
        /// Used to send variables between states
        /// </summary>
        public Hashtable Params
        {
            get { return m_paramsForStates; }
            set { m_paramsForStates = value; }
        }
        protected Hashtable m_paramsForStates = new Hashtable();

        protected bool m_stateMachineInitialized = false;

        public abstract void RegisterStates();

        public GameObject currentGameObject() { return this.gameObject; }

        public Transform currentTransform
        {
            get { return GetActorTransform; }
        }

        /// <summary>
        /// Used to guide movement state if you like
        /// </summary>
        public virtual Vector3 WorldDestination
        {
            get { return _lastWorldDestination; }
        }

        protected Vector3 _lastWorldDestination;

        public virtual void SetWorldDestination(Vector3 destination)
        {
            _lastWorldDestination = destination;
        }

        protected override void Awake()
        {
            base.Awake();
            RegisterStates();
        }

        protected virtual void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize each state
        /// </summary>
        public virtual void Initialize()
        {
            if (m_stateMachineInitialized)
                return;

            foreach (KeyValuePair<System.Type, FSMState> pair in m_states)
            {
                pair.Value.Initialize();
            }

            m_currentState = m_initialState;
            if (m_initialState == null)
            {
                Debug.LogWarning("Initialize(): Initial State is NULL");
            }
            else
            {
                OnEnterState = true;
            }



            OnExitState = false;
            m_stateMachineInitialized = true;
        }

        /// <summary>
        /// Implements the loop and logic performance from Unity
        /// </summary>
        protected virtual void Update()
        {
            Tick(Time.deltaTime);
        }


        /// <summary>
        /// Process the states
        /// </summary>
        public void Tick(float deltaTime)
        {
            if (OnExitState)
            {
                if (m_currentState != null)
                {
                    m_currentState.ExitState();
                }
                m_currentState = m_nextState;
                m_nextState = null;

                OnExitState = false;
                OnEnterState = true;
            }

            if (OnEnterState)
            {
                if (m_currentState != null)
                {
                    m_currentState.EnterState();
                    if (OnStateChanged != null)
                        OnStateChanged.Invoke();
                }

                OnEnterState = false;
            }

            if (m_currentState != null)
                m_currentState.Tick(deltaTime);

        }

        /// <summary>
        /// Used inside FixedUpdate
        /// </summary>
        public void PhysicsTick(float fixedDeltaTime)
        {
            if (!OnEnterState && !OnExitState)
            {
                if (m_currentState != null)
                    m_currentState.PhysicsTick(fixedDeltaTime);
                else
                {
                    m_nextState = m_initialState;
                    OnExitState = true;
                }
            }
        }

        /// <summary>
        /// Used inside LateUpdate
        /// </summary>
        public void PostTick(float deltaTime)
        {
            if (!OnEnterState && !OnExitState)
            {
                if (m_currentState != null)
                    m_currentState.PostTick(deltaTime);
                else
                {
                    m_nextState = m_initialState;
                    OnExitState = true;
                }
            }
        }

        /// <summary>
        /// Use this method to change the current active state
        /// </summary>
        public void ChangeState<T>() where T : FSMState { ChangeState(typeof(T)); }

        /// <summary>
        /// Use this method to change the current active state
        /// </summary>
        public void ChangeState(System.Type T)
        {
            if (!m_states.ContainsKey(T))
                return;

            m_nextState = m_states[T];

            OnExitState = true;
        }

        /// <summary>
        /// Change the current active state to the initial state
        /// </summary>
        public void GotoInitialState()
        {
            m_nextState = m_initialState;
            OnExitState = true;
        }

        /// <summary>
        /// Change the initial state based on type
        /// </summary>
        public void SetInitialState<T>() where T : FSMState { SetInitialState(typeof(T)); }

        /// <summary>
        /// Change the initial state based on type
        /// </summary>
        public void SetInitialState(System.Type T) { m_initialState = m_states[T]; }


        /// <summary>
        /// Change the initial state based on id
        /// </summary>
        public void SetInitialState(string id)
        {
            m_initialState = m_states.First(kv => kv.Value.Id == id).Value;
        }

        /// <summary>
        /// Compare if the indicated state is the current active state in the FSM
        /// </summary>
        public bool IsCurrentState(System.Type T)
        {
            if (T != typeof(FSMState) && !T.IsSubclassOf(typeof(FSMState)))
                return false;

            return (m_currentState.GetType() == T);
        }

        /// <summary>
        /// Compare if the indicated state is the current active state in the FSM
        /// </summary>
        public bool IsCurrentState<T>() where T : FSMState
        {
            return IsCurrentState(typeof(T));
        }

        /// <summary>
        /// Compare if the indicated state is the current active state in the FSM
        /// </summary>
        public bool IsCurrentState(string id)
        {
            return m_currentState.Id == id;
        }

        /// <summary>
        /// Add states to the FSM. T is subclass of FSMState
        /// </summary>
        public void AddState<T>() where T : FSMState, new()
        {
            if (!ContainsState<T>())
            {
                FSMState item = new T()
                {
                    Owner = this
                };
                m_states.Add(typeof(T), item);

                if (m_states.Count == 1)
                    SetInitialState<T>();

                if (m_stateMachineInitialized)
                    item.Initialize();
            }
        }

        /// <summary>
        /// Add states to the FSM. T is subclass of FSMState
        /// </summary>
        public void AddState(System.Type T)
        {
            if (T != typeof(FSMState) && !T.IsSubclassOf(typeof(FSMState)))
                return;

            if (!ContainsState(T))
            {
                FSMState item = (FSMState)System.Activator.CreateInstance(T);
                item.Owner = this;

                m_states.Add(T, item);

                if (m_states.Count == 1)
                    SetInitialState(T);

                if (m_stateMachineInitialized)
                    item.Initialize();
            }
        }

        /// <summary>
        /// Add states to the FSM. T is subclass of FSMState
        /// </summary>
        public void RemoveState<T>() where T : FSMState
        {
            RemoveState(typeof(T));
        }

        /// <summary>
        /// Remove states from the FSM. T is subclass of FSMState
        /// </summary>
        public void RemoveState(System.Type T)
        {
            if (T == typeof(FSMState) || T.IsSubclassOf(typeof(FSMState)))
            {
                if (this.IsCurrentState(T))
                    throw new UnityException("FSM: You can't delete the current active state");

                if (m_states.ContainsKey(T))
                {
                    m_states.Remove(T);
                }
            }
        }

        /// <summary>
        /// Check if this state exists on the FSM. T is subclass of FSMState
        /// </summary>
        public bool ContainsState<T>() where T : FSMState
        {
            return ContainsState(typeof(T));
        }

        /// <summary>
        /// Check if this state exists on the FSM. T is subclass of FSMState
        /// </summary>
        public bool ContainsState(System.Type T)
        {
            if (T != typeof(FSMState) && !T.IsSubclassOf(typeof(FSMState)))
                return false;
            return m_states.ContainsKey(T);
        }

        /// <summary>
        /// Remove all states available
        /// </summary>
        public void RemoveAllStates()
        {
            m_states.Clear();
            if (m_currentState != null)
            {
                m_currentState.ExitState();
                m_currentState = null;
            }
            m_nextState = null;
        }

        /// <summary>
        /// Return the object of the current state
        /// </summary>
        public T CurrentState<T>() where T : FSMState
        {
            return (T)m_currentState;
        }

        public T GetState<T>() where T : FSMState
        {
            return (T)m_states[typeof(T)];
        }

        protected Dictionary<System.Type, FSMState> m_states = new Dictionary<System.Type, FSMState>();

        protected bool OnEnterState { get; set; }
        protected bool OnExitState { get; set; }

        public virtual void OnAnimatorIK(int layerIndex)
        {
            if (!OnEnterState && !OnExitState)
            {
                if (m_currentState != null)
                {
                    m_currentState.OnAnimatorIK(layerIndex);
                }
            }
        }

        public virtual void OnCollisionEnter(Collision collision)
        {
            if (!OnEnterState && !OnExitState)
            {
                if (m_currentState != null)
                {
                    m_currentState.OnCollisionEnter(collision);
                }
            }
        }

        public virtual void OnCollisionExit(Collision collision)
        {
            if (!OnEnterState && !OnExitState)
            {
                if (m_currentState != null)
                {
                    m_currentState.OnCollisionExit(collision);
                }
            }
        }

        public virtual void OnCollisionStay(Collision collision)
        {
            if (!OnEnterState && !OnExitState)
            {
                if (m_currentState != null)
                {
                    m_currentState.OnCollisionStay(collision);
                }
            }
        }

        public virtual void OnTriggerEnter(Collider collider)
        {
            if (!OnEnterState && !OnExitState)
            {
                if (m_currentState != null)
                {
                    m_currentState.OnTriggerEnter(collider);
                }
            }
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            if (!OnEnterState && !OnExitState)
            {
                if (m_currentState != null)
                {
                    m_currentState.OnTriggerExit(collider);
                }
            }
        }

        public virtual void OnTriggerStay(Collider collider)
        {
            if (!OnEnterState && !OnExitState)
            {
                if (m_currentState != null)
                {
                    m_currentState.OnTriggerStay(collider);
                }
            }
        }
    }
}
