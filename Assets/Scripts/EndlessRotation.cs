using UnityEngine;

namespace Assets.Scripts
{
    public class EndlessRotation : MonoBehaviour
    {
        [SerializeField] private float _speed = 1f;

        void Update()
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + _speed / 20f, transform.localEulerAngles.z);
        }
    }
}
