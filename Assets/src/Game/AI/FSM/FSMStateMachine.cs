using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Game.AI.FSM
{
    [System.Serializable]
    public class FSMStateMachine : IFSMStateMachine
    {
        public delegate void OnStateMachineEventHandler();

        public event OnStateMachineEventHandler OnStateChanged;

        protected FSMState m_currentState { get; set; }
        protected FSMState m_nextState { get; set; }
        protected FSMState m_initialState { get; set; }

        protected bool m_stateMachineInitialized = false;

        /// <summary>
        /// Used to send variables between states
        /// </summary>
        public System.Collections.Hashtable Params
        {
            get { return m_paramsForStates; }
            set { m_paramsForStates = value; }
        }
        protected System.Collections.Hashtable m_paramsForStates = new System.Collections.Hashtable();

        public FSMStateMachine() { }

        /// <summary>
        /// This class is a clone of FSMStateMachineMonoBehaviour but its not a subclass of monobehaviour
        /// </summary>
        public FSMStateMachine(System.Type initState, bool init = false)
        {
            AddState(initState);

            if (init)
                Initialize();
        }

        public FSMStateMachine(System.Type initState, bool init = false, params System.Type[] moreStates)
        {
            AddState(initState);

            if (moreStates.Length > 0)
            {
                for (int i = 0; i < moreStates.Length; i++)
                {
                    AddState(moreStates[i]);
                }
            }

            SetInitialState(initState);

            if (init)
                Initialize();
        }

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
                Debug.LogWarning("Initialize() Initial State is NULL");
            }
            else
            {
                OnEnterState = true;
            }


            OnExitState = false;
            m_stateMachineInitialized = true;
        }

        /// <summary>
        /// Process the logic behind StateMachine. Change States and dispatch Start And End Events to the currentState
        /// </summary>
        /// <param name="deltaTime">Delta time.</param>
        public virtual void Tick(float deltaTime)
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
        /// Simular to FixedUpdate but just dispatch the physics function inside the State
        /// </summary>
        /// <param name="deltaTime">Fixed Delta time.</param>
        public virtual void PhysicsTick(float fixedDeltaTime)
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
        /// Simular to LateUpdate but just dispatch the LateUpdate function inside the State
        /// </summary>
        /// <param name="deltaTime">Fixed Delta time.</param>
        public virtual void PostTick(float deltaTime)
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
        /// You can change the State just indicating the type T
        /// </summary>
        public void ChangeState<T>() where T : FSMState { ChangeState(typeof(T)); }

        /// <summary>
        /// You can change the State just indicating the type T
        /// </summary>
        /// <param name="T">T needs to be FSMState parent</param>
        public void ChangeState(System.Type T)
        {
            if (!m_states.ContainsKey(T))
                return;

            m_nextState = m_states[T];

            OnExitState = true;
        }

        /// <summary>
        /// Send the machine to initial state
        /// </summary>
        public void GotoInitialState()
        {
            m_nextState = m_initialState;
            OnExitState = true;
        }

        public void SetInitialState<T>() where T : FSMState { SetInitialState(typeof(T)); }

        public void SetInitialState(System.Type T) { m_initialState = m_states[T]; }

        public void SetInitialState(string id)
        {
            m_initialState = m_states.First(kv => kv.Value.Id == id).Value;
        }

        public bool IsCurrentState(System.Type T)
        {
            if (T != typeof(FSMState) && !T.IsSubclassOf(typeof(FSMState)))
                return false;

            return (m_currentState.GetType() == T);
        }

        public bool IsCurrentState<T>() where T : FSMState
        {
            return IsCurrentState(typeof(T));
        }

        public bool IsCurrentState(string id)
        {
            return m_currentState.Id == id;
        }

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

        public void RemoveState<T>() where T : FSMState
        {
            RemoveState(typeof(T));
        }

        public void RemoveState(System.Type T)
        {
            if (T == typeof(FSMState) || T.IsSubclassOf(typeof(FSMState)))
            {
                if (m_states.ContainsKey(T))
                {
                    m_states.Remove(T);
                }
            }
        }

        public bool ContainsState<T>() where T : FSMState
        {
            return ContainsState(typeof(T));
        }

        public bool ContainsState(System.Type T)
        {
            if (T != typeof(FSMState) && !T.IsSubclassOf(typeof(FSMState)))
                return false;
            return m_states.ContainsKey(T);
        }

        public void RemoveAllStates()
        {
            m_states.Clear();
        }

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

        public virtual Vector3 WorldDestination
        {
            get { return _lastWorldDestination; }
        }

        protected Vector3 _lastWorldDestination;

        public virtual void SetWorldDestination(Vector3 destination)
        {
            _lastWorldDestination = destination;
        }

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


        protected GameObject referenceGameObject;
        protected Transform referenceTransform;

        public void SetCurrentGameObject(GameObject go)
        {
            referenceGameObject = go;
            referenceTransform = go.transform;
        }

        public GameObject currentGameObject() { return referenceGameObject; }
        public Transform currentTransform { get { return referenceTransform; } }

    }
}
