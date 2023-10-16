using AI.UtilityAI;
using UnityEngine;
using UnityEngine.AI;
using Simulation.Manager;

namespace Simulation.Entities
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SimCharacter : UAIAgent
    {
        private NavMeshAgent _navMeshAgent;


        [SerializeField] private CharacterCustomization modelA;
        [SerializeField] private CharacterCustomization modelB;

        private CharacterCustomization model;

        public Workstation CurrentWorkStation => simWorkstation;
        Workstation simWorkstation;

        protected override void Awake()
        {
            base.Awake();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        protected override void Start()
        {
            base.Start();
            GetPlaceToWork();
            SetCharacterRandomPreset();
        }

        void GetPlaceToWork()
        {
            simWorkstation = OfficeInstance.Instance.getPlaceAvailable(OfficeInstance.EPlaceCategory.WorkStation) as Workstation;
        }

        void SetCharacterRandomPreset()
        {
            model = Mathf.Round(Random.value) == 1 ? modelB : modelA;
            model.gameObject.SetActive(true);

            model.SetElementByIndex(CharacterCustomization.ClothesPartType.Shoes, Random.Range(1, model.shoesPresets.Count));
            model.SetElementByIndex(CharacterCustomization.ClothesPartType.Pants, Random.Range(1, model.pantsPresets.Count));
            model.SetElementByIndex(CharacterCustomization.ClothesPartType.TShirt, Random.Range(1, model.shirtsPresets.Count));
            model.SetElementByIndex(CharacterCustomization.ClothesPartType.Accessory, Random.Range(0, model.accessoryPresets.Count));
            model.SetHeadByIndex(Random.Range(0, model.headsPresets.Count));
            model.SetHairByIndex(Random.Range(0, model.hairPresets.Count));
            model.SetCharacterMaterialByIndex(Random.Range(0, model.skinMaterialPresets.Count));
        }

        float currentVelocityToAnimation = 0.0f;

        int AnimatorIdlingHash = Animator.StringToHash("idling");
        int AnimatorSpeedHash = Animator.StringToHash("speed");
        int AnimatorSittingHash = Animator.StringToHash("sitting");
        int AnimatorInteractionHash = Animator.StringToHash("interation");

        private void LateUpdate()
        {
            currentVelocityToAnimation = Mathf.MoveTowards(currentVelocityToAnimation, _navMeshAgent.velocity.sqrMagnitude, Time.deltaTime * 40f);

            foreach (var animtr in model.animators)
            {
                animtr.SetFloat(AnimatorSpeedHash, currentVelocityToAnimation);
                animtr.SetBool(AnimatorIdlingHash, !InMovement);
                animtr.SetBool(AnimatorSittingHash, sitting && !InMovement);
                animtr.SetInteger(AnimatorInteractionHash, interaction);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!InMovement)
            {
                _navMeshAgent.avoidancePriority = 0;
                if (placeToLookAt != null)
                {
                    lookToTarget(placeToLookAt.GetActorLocation + placeToLookAt.GetActorTransform.forward * 2f);
                }
            }
            else
            {
                _navMeshAgent.avoidancePriority = 50;
            }
        }

        Place placeToLookAt;

        bool sitting = false;
        int interaction = 0;

        public void Sit(Place place, int _interaction = 0)
        {
            sitting = true;
            placeToLookAt = place;
            interaction = _interaction;
        }

        public void Interact(Place place, int _interaction = 0)
        {
            placeToLookAt = place;
            interaction = _interaction;
        }

        public void Stand()
        {
            sitting = false;
            interaction = 0;

            if (placeToLookAt != null)
                placeToLookAt = null;
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
