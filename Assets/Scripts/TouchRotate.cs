using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Scripts
{
    public class TouchRotate : MonoBehaviour
    {
        protected bool IsOnAndroid
        {
            get { return Application.platform == RuntimePlatform.Android; }
        }

        protected bool IsOnIOS
        {
            get { return Application.platform == RuntimePlatform.IPhonePlayer; }
        }

        protected bool IsOnMobile
        {
            get { return IsOnAndroid || IsOnIOS; }
        }

        private bool _isDragging;
        private Vector3 _lastFrameMousePosition;

        void Update()
        {
            if (DidTouchBegin() || Input.GetMouseButtonDown(0))
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
            else if (_isDragging)
            {
                if (DidTouchMove() || Input.GetMouseButton(0))
                    RotateByTouchDelta();
                else if (DidTouchEnd() || Input.GetMouseButtonUp(0))
                    _isDragging = false;
            }
        }

        private bool DidTouchBegin()
        {
            return IsTouching() && GetTouch().phase == TouchPhase.Began;
        }

        private bool DidTouchMove()
        {
            return IsTouching() && GetTouch().phase == TouchPhase.Moved;
        }

        private bool DidTouchEnd()
        {
            return IsTouching() && GetTouch().phase == TouchPhase.Ended;
        }

        protected void HandleTouch()
        {
            Debug.Log("Handle Touch Begin");
            _isDragging = true;
            if (!IsOnMobile)
                UpdateLastFrameMousePosition();
        }

        protected void RotateByTouchDelta()
        {
            float xDelta;
            if (IsOnMobile)
                xDelta = IsTouching() ? GetTouch().deltaPosition.x : 0f;
            else
            {
                xDelta = Input.mousePosition.x - _lastFrameMousePosition.x;
                UpdateLastFrameMousePosition();
            }
            xDelta *= -0.1f;

            transform.Rotate(new Vector3(0f, xDelta, 0f));
        }

        private Touch GetTouch()
        {
            return Input.GetTouch(0);
        }

        private bool IsTouching()
        {
            return Input.touchCount > 0;
        }

        private void UpdateLastFrameMousePosition()
        {
            _lastFrameMousePosition = Input.mousePosition;
        }
    }
}