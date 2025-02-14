using System.Collections;
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

        const float INSPECT_ZOOM_AMOUNT = 2f;

        // movement
        Coroutine movementCoroutine;

        // inspect zoom
        float defaultZoomLevel;
        float inspectZoomLevel;

        // TODO - lock camera to a certain tile, and store that tile here
        // The position to return to when inspect mode is exited
        Vector3 originalInspectPosition = Vector3.zero;
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
        WorldController worldController;

        //
        // Lifecycle
        // 
        void Awake()
        {
            defaultZoomLevel = Camera.main.orthographicSize;
            // decreasing orthographic size increases zoom level
            inspectZoomLevel = defaultZoomLevel - INSPECT_ZOOM_AMOUNT;

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
            if (!isInInspectMode)
            {
                HandleInput();
            }
        }

        void HandleInput()
        {
            // User input is ignored in inspect mode, for now
            if (isInInspectMode) return;

            // Input
            if (Input.GetKeyDown(KeyCode.W))
            {
                MoveBy(Vector3.up);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                MoveBy(Vector3.left);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                MoveBy(Vector3.down);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                MoveBy(Vector3.right);
            }
        }

        //
        // Animations
        //
        // TODO - stop moveing when inspect mode starts
        void MoveBy(Vector3 amount)
        {
            const float MOVEMENT_AMOUNT = 1f;
            StartMovementTo(Camera.main.transform.position + (amount * MOVEMENT_AMOUNT));
        }

        Coroutine StartMovementTo(Vector3 targetPosition)
        {
            const float MOVEMENT_SPEED_S = 0.5f;

            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
            }

            movementCoroutine = StartCoroutine(Run());
            return movementCoroutine;

            IEnumerator Run()
            {
                var startPosition = Camera.main.transform.position;

                //
                var timer = 0f;
                while (timer < MOVEMENT_SPEED_S)
                {
                    var normalizedProgress = timer / MOVEMENT_SPEED_S;
                    Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, normalizedProgress);

                    timer += Time.deltaTime;
                    yield return null;
                }
            }
        }

        Coroutine StartInspectZoomTo(IInspectTarget inspectTarget)
        {
            const float INSPECT_ZOOM_LENGTH_S = 0.5f;

            return StartCoroutine(Run());

            IEnumerator Run()
            {
                var startZoomLevel = Camera.main.orthographicSize;
                var startPosition = Camera.main.transform.position;

                Vector3 targetPosition;
                float targetZoomLevel;

                if (inspectTarget == null)
                {
                    targetPosition = originalInspectPosition;
                    targetZoomLevel = defaultZoomLevel;

                    isInInspectMode = false;
                }
                else
                {
                    // Set position to return to when inspect mode ends
                    originalInspectPosition = Camera.main.transform.position;

                    var focalPoint = inspectTarget.GetInspectFocalPoint();
                    targetPosition = new Vector3(focalPoint.x, focalPoint.y, cameraZ);
                    targetZoomLevel = inspectZoomLevel;

                    isInInspectMode = true;
                }

                var timer = 0f;
                while (timer < INSPECT_ZOOM_LENGTH_S)
                {
                    var normalizedProgress = timer / INSPECT_ZOOM_LENGTH_S;
                    var inspectZoomProgress = inspectZoomAnimationCurve.Evaluate(normalizedProgress);

                    Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, inspectZoomProgress);
                    Camera.main.orthographicSize = Mathf.Lerp(startZoomLevel, targetZoomLevel, inspectZoomProgress);

                    timer += Time.deltaTime;

                    yield return null;
                }
            }
        }

        // TODO - this could be a bit smoother
        Coroutine StartRoomShake()
        {
            const float ROOM_BUILD_SHAKE_LENGTH_S = 0.3f;
            const float ROOM_BUILD_SHAKE_INTENSITY = 0.1f;

            return StartCoroutine(Run());

            IEnumerator Run()
            {
                var timer = 0f;
                var originalPosition = Camera.main.transform.localPosition;

                while (timer < ROOM_BUILD_SHAKE_LENGTH_S)
                {
                    var normalizedProgress = timer / ROOM_BUILD_SHAKE_LENGTH_S;
                    var shakeFalloff = roomBuildShakeFalloffAnimationCurve.Evaluate(normalizedProgress);
                    var shakeVector = Random.insideUnitSphere * ROOM_BUILD_SHAKE_INTENSITY;
                    var shakeAmount = shakeVector * shakeFalloff;

                    Camera.main.transform.localPosition += shakeAmount;

                    timer += Time.deltaTime;

                    yield return null;
                }

                Camera.main.transform.localPosition = originalPosition;
            }
        }

        //
        // Event handlers
        //
        void OnInspectTargetUpdate()
        {
            var inspectTarget = worldController.toolsController.inspectTool.inspectTarget;
            StartInspectZoomTo(inspectTarget);
        }

        void OnRoomBuilt()
        {
            StartRoomShake();
        }

        void OnRoomDestroyed()
        {
            StartRoomShake();
        }
    }
}