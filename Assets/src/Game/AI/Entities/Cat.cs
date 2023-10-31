using Game.AI.UtilityAI;
using Game.Actors;
using UnityEngine;
using UnityEngine.AI;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace Game.AI.Entities
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Cat : UAIAgent
    {
        private NavMeshAgent _navMeshAgent;
        private Animator _localAnimator;

        public UtilityAI.Property.UAIProperty StatHunger => this.properties["hunger"];
        public UtilityAI.Property.UAIProperty StatThirst => this.properties["thirst"];
        public UtilityAI.Property.UAIProperty StatEnergy => this.properties["energy"];
        public UtilityAI.Property.UAIProperty StatFun => this.properties["fun"];

        [SerializeField] private Material[] catSkin;
        [SerializeField] private SkinnedMeshRenderer catMesh;

        protected override void Awake()
        {
            base.Awake();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _localAnimator = GetComponentInChildren<Animator>();

            OnAgentActionCompleted += OnActionCompletedEvent;
        }

        protected override void Start()
        {
            base.Start();
            catMesh.material = catSkin[Random.Range(0, catSkin.Length)];
        }

        float currentVelocityToAnimation = 0.0f;

        readonly int AnimatorIdlingHash = Animator.StringToHash("idling");
        readonly int AnimatorSpeedHash = Animator.StringToHash("speed");

        private void LateUpdate()
        {
            currentVelocityToAnimation = Mathf.MoveTowards(currentVelocityToAnimation, _navMeshAgent.velocity.sqrMagnitude, Time.deltaTime * 40f);

            _localAnimator.SetFloat(AnimatorSpeedHash, currentVelocityToAnimation);
            _localAnimator.SetBool(AnimatorIdlingHash, !InMovement);
        }

        protected override void Update()
        {
            base.Update();

            if (!InMovement)
            {
                _navMeshAgent.avoidancePriority = 0;
                if (objectToLookAt != null)
                {
                    lookToTarget(objectToLookAt.GetActorLocation);
                }
            }
            else
            {
                _navMeshAgent.avoidancePriority = 50;
            }
        }

        public void SetWalkTo(Vector3 destination)
        {
            if (!Params.ContainsKey("destination"))
                Params.Add("destination", destination);
            else
                Params["destination"] = destination;

            DemandAction("walkto");
        }

        InteractiveObject objectToLookAt;

        public void Interact(InteractiveObject obj)
        {
            objectToLookAt = obj;
        }

        public bool InMovement
        {
            get
            {
                return _navMeshAgent.pathPending || _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance || _navMeshAgent.velocity != Vector3.zero;
            }
        }

        private void OnActionCompletedEvent(string actionId)
        {
            _localAnimator.Play("Stand", 0);
        }
    }
}
