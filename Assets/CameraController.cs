using TowerBuilder;
using UnityEditor.UIElements;
using UnityEngine;

namespace TowerBuilder
{

    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        AnimationCurve inspectAnimationCurve;

        const float MOVEMENT_SPEED = 1.5f;
        const float MOVEMENT_AMOUNT = 1.2f;
        const float INSPECT_ZOOM_AMOUNT = 2f;
        const float INSPECT_ZOOM_LENGTH_S = 0.5f;

        Vector3 targetPosition;
        float movementTimer = 0;

        WorldController worldController;

        float originalZoomLevel;
        float inspectZoomLevel;
        float targetZoomLevel;
        float startZoomLevel;
        float inspectZoomTimer;

        void Awake()
        {
            targetPosition = Camera.main.transform.position;

            originalZoomLevel = Camera.main.orthographicSize;
            startZoomLevel = originalZoomLevel;
            targetZoomLevel = originalZoomLevel;
            // decreasing orthographic size increases zoom level
            inspectZoomLevel = originalZoomLevel - INSPECT_ZOOM_AMOUNT;
            inspectZoomTimer = INSPECT_ZOOM_LENGTH_S;

            worldController = WorldController.Get();
        }

        void Start()
        {
            worldController.toolsController.inspectTool.onInspectTargetUpdated += OnInspectTargetUpdate;
        }

        void Update()
        {
            HandleInput();

            // animate inspect zoom
            float zoomProgress = 1f;
            if (inspectZoomTimer < INSPECT_ZOOM_LENGTH_S)
            {
                inspectZoomTimer += Time.deltaTime;
                var normalizedProgress = inspectZoomTimer / INSPECT_ZOOM_LENGTH_S;
                zoomProgress = inspectAnimationCurve.Evaluate(normalizedProgress);
            }

            Camera.main.orthographicSize = Mathf.Lerp(startZoomLevel, targetZoomLevel, zoomProgress);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, movementTimer);
        }

        void HandleInput()
        {
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

        void OnInspectTargetUpdate()
        {
            var inspectTarget = worldController.toolsController.inspectTool.inspectTarget;

            startZoomLevel = Camera.main.orthographicSize;

            if (inspectTarget == null)
            {
                targetZoomLevel = originalZoomLevel;
            }
            else
            {
                targetZoomLevel = inspectZoomLevel;
            }

            inspectZoomTimer = 0;
        }
    }
}