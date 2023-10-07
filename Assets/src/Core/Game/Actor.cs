using UnityEngine;

namespace Core.Game
{
    public class Actor : Neuron
    {
        [Header("Actor")]
        [SerializeField] protected float lookTargetVelocity = 15f;

        public bool isLocalPlayer => true;
        public bool hasAuthority => true;

        public Transform GetActorTransform
        {
            get
            {
                return m_transform ?? (m_transform = this.gameObject.transform);
            }
        }
        private Transform m_transform;


        public Vector3 GetActorLocation
        {
            get
            {
                if (m_transform != null)
                    return m_transform.position;
                else
                    return Vector3.zero;
            }
        }

        public Quaternion GetActorRotation
        {
            get
            {
                if (m_transform != null)
                    return m_transform.rotation;
                else
                    return Quaternion.identity;
            }
        }

        public Vector3 GetActorScale
        {
            get
            {
                if (m_transform != null)
                    return m_transform.localScale;
                else
                    return Vector3.zero;
            }
        }

        protected virtual void Awake()
        {
            if (this.m_transform == null)
                this.m_transform = this.transform;
        }

        public float getDistance(Vector3 to, bool inverse = false)
        {
            if (inverse)
                return Vector3.Distance(to, GetActorTransform.position);
            return Vector3.Distance(GetActorTransform.position, to);
        }

        public virtual void Dispose(float delay = 0f)
        {
            StopAllCoroutines();

            if (delay <= 0f)
                Destroy(gameObject);
            else
                Destroy(gameObject, delay);
        }


        protected virtual void OnDestroy() { }

        protected virtual void OnEnable() { }

        public void lookToTarget(Vector3 target)
        {
            Vector3 pos = GetActorTransform.position;

            Vector3 quatPos = new Vector3(target.x, pos.y, target.z) - pos;
            if (quatPos != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(new Vector3(target.x, pos.y, target.z) - pos);
                GetActorTransform.rotation = Quaternion.Lerp(GetActorTransform.rotation, newRotation, lookTargetVelocity * Time.deltaTime);
            }
        }

        public void instantLookAtTarget(Vector3 target)
        {
            Vector3 pos = GetActorTransform.position;

            Vector3 quatPos = new Vector3(target.x, pos.y, target.z) - pos;
            if (quatPos != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(new Vector3(target.x, pos.y, target.z) - pos);
                GetActorTransform.rotation = newRotation;
            }
        }

        public static T SpawnActorFromClass<T>(string name) where T : MonoBehaviour
        {
            return SpawnActorFromClass<T>(name, Vector3.zero, Quaternion.identity);
        }

        public static T SpawnActorFromClass<T>(string name, Vector3 location, Quaternion rotation) where T : MonoBehaviour
        {
            GameObject actor = new GameObject(name, typeof(T));
            actor.transform.position = location;
            actor.transform.rotation = rotation;
            return actor.GetComponent<T>();
        }

        public static T SpawnActorFromClassChildOf<T>(string Name, Transform parent, bool WorldPositionStays) where T : MonoBehaviour
        {
            T reference = SpawnActorFromClass<T>(Name, parent.position, parent.rotation);
            reference.transform.SetParent(parent, WorldPositionStays);
            return reference;
        }
    }
}
