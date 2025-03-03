using UnityEngine;

namespace TowerBuilder
{
    public class SkyManager : MonoBehaviour
    {
        Material skyMaterial;

        WorldController worldController;

        void Awake()
        {
            skyMaterial = transform.gameObject.GetComponent<MeshRenderer>().material;

            worldController = WorldController.Get();
        }

        void Update()
        {
            if (worldController.timeController.speed.HasChanged())
            {
                SetSkySpeedForCurrentTimeSpeed();
            }
        }

        void SetSkySpeedForCurrentTimeSpeed()
        {
            float speed;

            switch (worldController.timeController.speed.current)
            {
                case TimeSpeed.Pause:
                    speed = 0f;
                    break;
                case TimeSpeed.Normal:
                    speed = 0.1f;
                    break;
                case TimeSpeed.Fast:
                    speed = 0.2f;
                    break;
                case TimeSpeed.Fastest:
                    speed = 0.3f;
                    break;
                default:
                    speed = 0.1f;
                    break;
            }

            SetSkySpeed(speed);
        }

        void SetSkySpeed(float speed)
        {
            skyMaterial.SetFloat("_StarSpeed", speed);
        }
    }
}