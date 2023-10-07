using UnityEngine;
using Core.Game;
using Game.Manager;

namespace Game.Entities
{
    public class Place : Actor
    {
        public enum EPlaceType
        {
            Stand,
            Sit,
        }

        public GameObject ObjectOfInterest => _objectOfInterest;
        [SerializeField] private GameObject _objectOfInterest;

        public EPlaceType PlaceType => _placeType;
        [SerializeField] private EPlaceType _placeType;

        public OfficeInstance.EPlaceCategory Category => _placeCategory;
        [SerializeField] private OfficeInstance.EPlaceCategory _placeCategory;

        [SerializeField] private bool rotateObjInterest;
        [SerializeField] private Vector3 ObjInterestAngleTarget;

        Vector3 objInterestInitialPosition;
        Vector3 objInterestInitialRotationAngle;

        protected override void Awake()
        {
            base.Awake();
            OfficeInstance.Instance.setPlaceAvailable(_placeCategory, this);

            if (_objectOfInterest != null)
            {
                objInterestInitialPosition = _objectOfInterest.transform.position;
                objInterestInitialRotationAngle = _objectOfInterest.transform.eulerAngles;
            }
        }

        public void StartUsing()
        {
            if (rotateObjInterest && _objectOfInterest)
            {
                _objectOfInterest.transform.rotation = Quaternion.Euler(ObjInterestAngleTarget);
            }
        }

        public void StopUsing()
        {
            if (_objectOfInterest)
            {
                _objectOfInterest.transform.rotation = Quaternion.Euler(objInterestInitialRotationAngle);
            }
        }
    }
}