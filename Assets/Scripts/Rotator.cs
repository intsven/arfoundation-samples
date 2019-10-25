using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts
{
    public class Rotator : MonoBehaviour, IDragHandler
    {
        public Transform world;
        public GameObject swiper;
        public float zoomSpeed = 1f;
        float direction;

        public void OnDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {

                // Get movement of the finger since last frame
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

                //actual touch
                Touch touchZero = Input.GetTouch(0);
                //current touch x position
                float touchZeroCurrPos = touchZero.position.x;
                //previous touch x position
                float touchZeroPrevPos = touchZero.position.x - touchDeltaPosition.x;
                //difference between previous and current finger position
                float Diff = touchZeroPrevPos - touchZeroCurrPos;


                if (touchZeroCurrPos < touchZeroPrevPos)
                {
                    direction = -1;
                    swiper.transform.Rotate(Vector3.up, -Diff * Time.deltaTime * zoomSpeed);
                }
                if (touchZeroCurrPos > touchZeroPrevPos)
                {
                    direction = 1;
                    swiper.transform.Rotate(Vector3.down, Diff * Time.deltaTime * zoomSpeed);
                }

            }
        }
    }
}