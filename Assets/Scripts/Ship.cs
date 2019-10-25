using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Scripts
{
    public class Ship : MonoBehaviour
    {
        private float _waypointEverySeconds = 0.05f;

        protected bool IsOnAndroid { get { return Application.platform == RuntimePlatform.Android; } }
        protected bool IsOnIOS { get { return Application.platform == RuntimePlatform.IPhonePlayer; } }
        protected bool IsOnMobile { get { return IsOnAndroid || IsOnIOS; } }

        private bool _isBeingDragged;
        private Vector3 _lastDragPosition;
        private bool _isMoving;

        private List<Vector3> _waypoints = new List<Vector3>();

        void FixedUpdate()
        {
            if (DidTouchBegin() && !_isBeingDragged)
            {
                _isBeingDragged = GetIsTouchOnSelf();
                if (_isBeingDragged)
                {
                    if (_isMoving)
                    {
                        _waypoints.Clear();
                        StopAllCoroutines();
                        _isMoving = false;
                    }
                    StartCoroutine(CreateWaypoints());
                }
            }
            else if (!IsTouchStillContinuing() && _isBeingDragged)
            {
                _isBeingDragged = false;
            }

            if (!_isMoving && _waypoints.Count > 0)
            {
                StartCoroutine(MoveToNextWaypoint());
            }
        }

        private Vector3 GetNextWaypoint()
        {
            var waypoint = _waypoints[0];
            _waypoints.RemoveAt(0);
            return waypoint;
        }

        private IEnumerator MoveToNextWaypoint()
        {
            var nextWaypoint = GetNextWaypoint();
            var startPosition = transform.localPosition;
            var distance = Vector3.Distance(startPosition, nextWaypoint);
            var step = 1f / (distance * 25f);
            var progress = 0f;
            var rotationProgress = 0f;

            _isMoving = true;
            var targetRotation = Quaternion.LookRotation(startPosition - nextWaypoint);         
            var startRotation = transform.localRotation;

            while (progress < 1f)
            {
                progress += step;
                transform.localPosition = Vector3.Slerp(startPosition, nextWaypoint, progress);
                if (rotationProgress < 1)
                {
                    rotationProgress += step;
                    transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, rotationProgress);
                }
                
                yield return -1;
            }

            if (_waypoints.Count > 0)
                StartCoroutine(MoveToNextWaypoint());
            else
                _isMoving = false;
        }

        private IEnumerator CreateWaypoints()
        {
            while (_isBeingDragged)
            {
                yield return new WaitForSeconds(_waypointEverySeconds);
                CreateWaypoint();
            }
        }

        private bool DidTouchBegin()
        {
            if (IsOnMobile)
                return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
            return Input.GetMouseButtonDown(0);
        }

        private bool IsTouchStillContinuing()
        {
            if (IsOnMobile)
                return Input.touchCount > 0;
            return Input.GetMouseButton(0);
        }

        private Vector3 GetTouchPosition()
        {
            if (IsOnMobile)
                return Input.GetTouch(0).position;
            return Input.mousePosition;
        }

        private bool GetIsTouchOnSelf()
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
                    return true;
            }
            return false;
        }

        private void CreateWaypoint()
        {
            var ray = Camera.main.ScreenPointToRay(GetTouchPosition());
            var raycastHits = Physics.RaycastAll(ray, Mathf.Infinity);
            float threshold = 0.025f;
            for (var i = 0; i < raycastHits.Length; i++)
            {
                if (raycastHits[i].transform.tag == "Water")
                {
                    var waypoint = transform.parent.InverseTransformPoint(raycastHits[i].point);
                    bool isTooCloseToSelf = Vector3.Distance(waypoint, transform.localPosition) < threshold;
                    bool isTooCloseToLastWaypoint = _waypoints.Count > 0 && Vector3.Distance(waypoint, _waypoints[_waypoints.Count - 1]) < threshold;
                    if (!isTooCloseToSelf && !isTooCloseToLastWaypoint)
                        _waypoints.Add(waypoint);    
                    break;
                }
            }
        }
    }
}
