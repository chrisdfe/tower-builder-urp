using TowerBuilder;
using UnityEditor.UIElements;
using UnityEngine;

namespace TowerBuilder
{

    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        AnimationCurve inspectZoomAnimationCurve;

        [SerializeField]
        AnimationCurve roomBuildShakeFalloffAnimationCurve;

        const float MOVEMENT_SPEED = 1.5f;
        const float MOVEMENT_AMOUNT = 1.2f;
        const float INSPECT_ZOOM_AMOUNT = 2f;
        const float INSPECT_ZOOM_LENGTH_S = 0.5f;

        // movement
        Vector3 targetPosition;
        float movementTimer = 0;

        // inspect zoom1
        float originalZoomLevel;
        float inspectZoomLevel;
        float targetZoomLevel;
        float startZoomLevel;
        float inspectZoomTimer = INSPECT_ZOOM_LENGTH_S;
        // The position to return to when inspect mode is exited
        // TODO - lock camera to a certain tile, and store that tile here
        Vector3 originalInspectPosition = Vector3.zero;
        Vector3 startInspectPosition = Vector3.zero;
        Vector3 targetInspectPosition;
        bool isInInspectMode = false;

        // This never changes
        float _cameraZ = float.NegativeInfinity;
        float cameraZ
        {
            get
            {
                if (_cameraZ == float.NegativeInfinity)
                {
                    _cameraZ = Camera.main.transform.position.z;
                }
                return _cameraZ;
            }
        }

        // camera shake
        const float ROOM_BUILD_SHAKE_LENGTH_S = 0.3f;
        const float ROOM_BUILD_SHAKE_INTENSITY = 0.15f;
        float roomBuildShakeTimer = ROOM_BUILD_SHAKE_LENGTH_S;
        // float currentRoomShakeValue = ROOM_BUILD_SHAKE_INTENSITY;

        WorldController worldController;

        //
        // Lifecycle
        // 
        void Awake()
        {
            originalInspectPosition = Camera.main.transform.position;
            targetPosition = Camera.main.transform.position;
            targetInspectPosition = Camera.main.transform.position;

            originalZoomLevel = Camera.main.orthographicSize;
            startZoomLevel = originalZoomLevel;
            targetZoomLevel = originalZoomLevel;

            // decreasing orthographic size increases zoom level
            inspectZoomLevel = originalZoomLevel - INSPECT_ZOOM_AMOUNT;

            worldController = WorldController.Get();
        }

        void Start()
        {
            worldController.toolsController.inspectTool.onInspectTargetUpdated += OnInspectTargetUpdate;
            worldController.buildingsController.onRoomBuilt += OnRoomBuilt;
            worldController.buildingsController.onRoomDestroyed += OnRoomDestroyed;
        }

        void Update()
        {
            // if (inspectZoomTimer < INSPECT)
            if (inspectZoomTimer < INSPECT_ZOOM_LENGTH_S)
            {
                inspectZoomTimer += Time.deltaTime;
                var normalizedProgress = inspectZoomTimer / INSPECT_ZOOM_LENGTH_S;
                var inspectZoomProgress = inspectZoomAnimationCurve.Evaluate(normalizedProgress);

                Camera.main.transform.position = Vector3.Lerp(startInspectPosition, targetInspectPosition, inspectZoomProgress);
                Camera.main.orthographicSize = Mathf.Lerp(startZoomLevel, targetZoomLevel, inspectZoomProgress);
            }
            else
            {
                HandleInput();
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, movementTimer);
            }

            if (roomBuildShakeTimer < ROOM_BUILD_SHAKE_LENGTH_S)
            {
                var normalizedProgress = roomBuildShakeTimer / ROOM_BUILD_SHAKE_LENGTH_S;
                var shakeFalloff = roomBuildShakeFalloffAnimationCurve.Evaluate(normalizedProgress);

                // if (currentRoomShakeValue == ROOM_BUILD_SHAKE_INTENSITY)
                // {
                //     currentRoomShakeValue = -ROOM_BUILD_SHAKE_INTENSITY;
                // }
                // else
                // {
                //     currentRoomShakeValue = ROOM_BUILD_SHAKE_INTENSITY;
                // }

                // var shakeVector = new Vector3(
                //     0,
                //     // Random.insideUnitSphere.y,
                //     ROOM_BUILD_SHAKE_INTENSITY,
                //     0
                // );

                var shakeVector = Random.insideUnitSphere * ROOM_BUILD_SHAKE_INTENSITY;
                var shakeAmount = shakeVector * shakeFalloff;

                Camera.main.transform.localPosition += shakeAmount;
                roomBuildShakeTimer += Time.deltaTime;
            }
            else
            {
                roomBuildShakeTimer = ROOM_BUILD_SHAKE_LENGTH_S;
            }
        }

        void HandleInput()
        {
            // User input is ignored in inspect mode, for now
            if (isInInspectMode) return;

            bool shouldResetTimer = false;

            // Input
            if (Input.GetKeyDown(KeyCode.W))
            {
                targetPosition += Vector3.up * MOVEMENT_AMOUNT;
                shouldResetTimer = true;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                targetPosition += Vector3.left * MOVEMENT_AMOUNT;
                shouldResetTimer = true;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                targetPosition += Vector3.down * MOVEMENT_AMOUNT;
                shouldResetTimer = true;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                targetPosition += Vector3.right * MOVEMENT_AMOUNT;
                shouldResetTimer = true;
            }

            // Movement
            if (shouldResetTimer)
            {
                movementTimer = 0;
            }
            else
            {
                movementTimer += Time.deltaTime / MOVEMENT_SPEED;
            }
        }

        //
        // Event handlers
        //
        void OnInspectTargetUpdate()
        {
            var inspectTarget = worldController.toolsController.inspectTool.inspectTarget;

            startZoomLevel = Camera.main.orthographicSize;
            startInspectPosition = Camera.main.transform.position;
            originalInspectPosition = Camera.main.transform.position;

            if (inspectTarget == null)
            {
                targetInspectPosition = originalInspectPosition;
                targetZoomLevel = originalZoomLevel;

                isInInspectMode = false;
            }
            else
            {
                originalInspectPosition = Camera.main.transform.position;
                var focalPoint = inspectTarget.GetInspectFocalPoint();
                targetInspectPosition = new Vector3(focalPoint.x, focalPoint.y, cameraZ);
                targetZoomLevel = inspectZoomLevel;

                isInInspectMode = true;
            }

            targetPosition = targetInspectPosition;

            inspectZoomTimer = 0;
        }

        void OnRoomBuilt()
        {
            roomBuildShakeTimer = 0f;
        }

        void OnRoomDestroyed()
        {
            // TODO - seperate destroy shake type
            roomBuildShakeTimer = 0f;
        }
    }
}