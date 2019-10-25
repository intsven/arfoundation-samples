using UnityEngine;
using UnityEngine.Events;

namespace Scripts
{
    public class ARButton : MonoBehaviour
    {
        public UnityEvent OnTouch = new UnityEvent();

        protected bool IsOnAndroid { get { return Application.platform == RuntimePlatform.Android; } }
        protected bool IsOnIOS { get { return Application.platform == RuntimePlatform.IPhonePlayer; } }
        protected bool IsOnMobile { get { return IsOnAndroid || IsOnIOS; } }

        private bool _isTouching;

        void Update()
        {
            if (DidTouchBegin() || !IsOnMobile && Input.GetMouseButton(0))
            {
                Vector3 touchPosition;

                if (IsOnMobile && DidTouchBegin())
                    touchPosition = Input.GetTouch(0).position;
                else
                    touchPosition = Input.mousePosition;

                var ray = Camera.main.ScreenPointToRay(touchPosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == gameObject)
                        HandleTouch();
                }
            }
        }

        private bool DidTouchBegin()
        {
            if (Input.touchCount > 0 && !_isTouching)
            {
                _isTouching = true;
                return true;
            }
            if (_isTouching && Input.touchCount <= 0)
                _isTouching = false;

            return false;
        }

        protected virtual void HandleTouch()
        {
            Debug.Log("3----------->TOOOOUCH!");
            OnTouch.Invoke();
        }
    }
}
