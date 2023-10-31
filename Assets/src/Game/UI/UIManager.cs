using System.Collections;
using System.Collections.Generic;
using Game.Actors;
using Game.AI.Entities.Actions;
using Game.AI.Entities.Actions.States;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Entity")]
        [SerializeField] private Game.AI.Entities.Cat cat;
        [SerializeField] private Text txtState;

        [Header("Sliders")]
        [SerializeField] private Slider sliderHunger;
        [SerializeField] private Slider sliderThirst;
        [SerializeField] private Slider sliderEnergy;
        [SerializeField] private Slider sliderFun;


        private void Update()
        {
            UpdateCameraInteraction();
            MoveTheCatToThisPosition();

            var action = cat.GetCurrentAction() as ActionIdle;

            if(action != null){
                action.StateMachine.IsCurrentState<StateExecute>();
                string state;
                switch (action.Id){
                    case "eat":
                        state = action.StateMachine.IsCurrentState<StateExecute>() ? "Eating cat food" : "Going to Eat";
                    break;
                    case "drinkWater":
                        state = action.StateMachine.IsCurrentState<StateExecute>() ? "Drinking water" : "Going to drink water";
                    break;
                    case "play":
                        state = action.StateMachine.IsCurrentState<StateExecute>() ? "Having fun" : "Going to play with a toy";
                    break;
                    case "sleep":
                        state = action.StateMachine.IsCurrentState<StateExecute>() ? "Sleeping" : "Going to sleep";
                    break;
                    case "walkto":
                        state = action.StateMachine.IsCurrentState<StateExecute>() ? "In an specific location" : "Going to a specific location";
                    break;
                    default:
                        state = "Licking itself";
                    break;
                }
                txtState.text = $"State: {state}";
            }
        }

        [Header("Camera Movement")]
        [SerializeField] private Transform cameraBase;
        [SerializeField] private float panSpeed = 20f; // Speed at which the camera moves
        [SerializeField] private float panBorderThickness = 10f; // Distance from screen edge to start panning
        [SerializeField] private Vector2 panLimitMin;
        [SerializeField] private Vector2 panLimitMax;

        [Header("Camera Zoom")]
        [SerializeField] private float scrollSpeed = 20f; // Speed at which the camera zooms in/out
        [SerializeField] private float minFov = 30f; // Minimum field of view
        [SerializeField] private float maxFov = 70f; // Maximum field of view

        void UpdateCameraInteraction()
        {
            Vector3 pos = cameraBase.position;

            if (Input.mousePosition.y >= Screen.height - panBorderThickness)
            {
                pos.x -= panSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.y <= panBorderThickness)
            {
                pos.x += panSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.x >= Screen.width - panBorderThickness)
            {
                pos.z += panSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.x <= panBorderThickness)
            {
                pos.z -= panSpeed * Time.deltaTime;
            }

            pos.x = Mathf.Clamp(pos.x, panLimitMin.x, panLimitMax.x);
            pos.z = Mathf.Clamp(pos.z, panLimitMin.y, panLimitMax.y);

            cameraBase.position = pos;

            float fov = Camera.main.fieldOfView;
            fov -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
            fov = Mathf.Clamp(fov, minFov, maxFov);
            Camera.main.fieldOfView = fov;
        }

        [Header("Click Action")]
        [SerializeField] private LayerMask interactionLayer;
        [SerializeField] private GameObject mouseGroundIndicator;

        void MoveTheCatToThisPosition()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactionLayer))
                {
                    GameObject indicator = Instantiate(mouseGroundIndicator, hit.point, Quaternion.identity);

                    if(hit.collider.transform.TryGetComponent<InteractiveObject>(out var interaction))
                    {
                        switch(interaction.InteractiveType){
                            case InteractiveObject.EInteractiveType.FOOD:
                                cat.Params["food"] = interaction;
                                cat.DemandAction("eat");
                            break;
                            case InteractiveObject.EInteractiveType.DRINK:
                                cat.Params["drink"] = interaction;
                                cat.DemandAction("drinkWater");
                            break;
                            case InteractiveObject.EInteractiveType.TOY:
                                cat.Params["play"] = interaction;
                                cat.DemandAction("play");
                            break;
                            case InteractiveObject.EInteractiveType.SLEEP_PLACE:
                                cat.Params["sleep"] = interaction;
                                cat.DemandAction("sleep");
                            break;
                        }
                    }else{
                        cat.SetWalkTo(hit.point);
                    }
                    Destroy(indicator, 0.5f);
                }
            }
        }

        private void LateUpdate()
        {
            if (this.cat == null)
                return;

            sliderHunger.value = cat.StatHunger.normalizedValue;
            sliderThirst.value = cat.StatThirst.normalizedValue;
            sliderEnergy.value = cat.StatEnergy.normalizedValue;
            sliderFun.value = cat.StatFun.normalizedValue;
        }
    }
}
