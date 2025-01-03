using TowerBuilder;
using UnityEngine;

namespace TowerBuilder
{

    public class CameraController : MonoBehaviour
    {
        static float MOVEMENT_SPEED = 1.5f;
        static float MOVEMENT_AMOUNT = 1.2f;

        Vector3 targetPosition;
        float movementTimer = 0;

        void Awake()
        {
            targetPosition = Camera.main.transform.position;
        }

        void Update()
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

            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, movementTimer);
        }
    }
}