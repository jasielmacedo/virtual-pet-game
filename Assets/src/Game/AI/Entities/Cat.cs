using Game.AI.UtilityAI;
using Game.Actors;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AI.Entities
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Cat : UAIAgent
    {
        private NavMeshAgent _navMeshAgent;
        private Animator _localAnimator;

        protected override void Awake()
        {
            base.Awake();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _localAnimator = GetComponentInChildren<Animator>();
        }

        protected override void Start()
        {
            base.Start();
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
                    lookToTarget(objectToLookAt.GetActorLocation + objectToLookAt.GetActorTransform.forward * 2f);
                }
            }
            else
            {
                _navMeshAgent.avoidancePriority = 50;
            }
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
    }
}
