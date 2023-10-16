using UnityEngine;
using Core.Game;
using Game.Manager;

namespace Game.Actors
{
    public class Place : InteractiveObject {

        [Header("Place")]
        [SerializeField] private GameObject _additionalObjectOfInterest;

        [SerializeField] private bool rotateObjInterest;
        [SerializeField] private Vector3 ObjInterestAngleTarget;

        Vector3 objInterestInitialRotationAngle;

        protected override void Awake()
        {
            base.Awake();

            if (_additionalObjectOfInterest != null)
            {
                objInterestInitialRotationAngle = _additionalObjectOfInterest.transform.eulerAngles;
            }
        }

        public override void StartUsing()
        {
            if (rotateObjInterest && _additionalObjectOfInterest)
            {
                _additionalObjectOfInterest.transform.rotation = Quaternion.Euler(ObjInterestAngleTarget);
            }
        }

        public override void  StopUsing()
        {
            if (_additionalObjectOfInterest)
            {
                _additionalObjectOfInterest.transform.rotation = Quaternion.Euler(objInterestInitialRotationAngle);
            }
        }
    }
}