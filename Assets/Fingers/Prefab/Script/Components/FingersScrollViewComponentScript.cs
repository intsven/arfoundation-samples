﻿//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRubyShared
{
    [AddComponentMenu("Fingers Gestures/Component/ScrollView", 6)]
    public class FingersScrollViewComponentScript : MonoBehaviour
    {
        [Tooltip("The content to act as a scroll view.")]
        public GameObject ScrollContent;

        [Tooltip("The game object of the element containing the scroll view. Can be a canvas or a panel, etc.")]
        public GameObject ScrollContentContainer;

        [Tooltip("Canvas camera, null if canvas is screen space.")]
        public Camera CanvasCamera;

        [Tooltip("The max speed for the scroll view. When a pan finishes, velocity will be applied and the content will move a bit more.")]
        [Range(0.01f, 4096.0f)]
        public float MaxSpeed = 1024.0f;

        [Tooltip("The threshold of zoom scale at which a double tap will zoom out instead of zoom in.")]
        [Range(1.0f, 10.0f)]
        public float DoubleTapZoomOutThreshold = 2.5f;

        [Tooltip("The scale at which a double tap will zoom out to.")]
        [Range(0.1f, 10.0f)]
        public float DoubleTapZoomOutValue = 0.5f;

        [Tooltip("The scale at which a double tap will zoom in to.")]
        [Range(0.1f, 10.0f)]
        public float DoubleTapZoomInValue = 4.0f;

        [Tooltip("How long a double tap will animate the zoom in and out.")]
        [Range(0.01f, 3.0f)]
        public float DoubleTapAnimationTimeSeconds = 0.5f;

        [Tooltip("How quickly pan velocity reduces when a pan finishes. Lower values reduce faster.")]
        [Range(0.01f, 0.999f)]
        public float PanDampening = 0.95f;

        [Tooltip("How quickly the scale velocity reduces when a scale finishes. Lower values reduce faster.")]
        [Range(0.01f, 0.999f)]
        public float ScaleDampening = 0.1f;

        [Tooltip("How fast the scale gesture scales in and out. Lower values scale more slowly.")]
        [Range(0.0001f, 0.1f)]
        public float ScaleSpeed = 0.01f;

        [Tooltip("The minimum scale multiplier.")]
        [Range(0.01f, 1.0f)]
        public float MinimumScale = 0.1f;

        [Tooltip("The maximum scale multiplier.")]
        [Range(0.01f, 100.0f)]
        public float MaximumScale = 8.0f;

        [Tooltip("How quickly the content bounces back to the center if it is moved or scaled out of bounds. Higher values move to the center faster.")]
        [Range(0.01f, 1.0f)]
        public float BounceModifier = 0.2f;

        [Tooltip("Optional, a text element to show debug text, useful for debugging the scroll view.")]
        public UnityEngine.UI.Text DebugText;

        private RectTransform contentRectTransform;
        private RectTransform containerRectTransform;

        // pan animation state
        private Vector2 panVelocity;

        // scale animation state
        private float zoomSpeed = 1.0f;
        private Vector2 lastScaleFocus;

        // double tap animation state
        private float doubleTapScaleTimeSecondsRemaining;
        private float doubleTapScaleStart;
        private float doubleTapScaleEnd;
        private Vector2 doubleTapPosStart;
        private Vector2 doubleTapPosEnd;

        /// <summary>
        /// Scale gesture for zooming in and out
        /// </summary>
        public ScaleGestureRecognizer ScaleGesture { get; private set; }

        /// <summary>
        /// Pan gesture for moving the scroll view around
        /// </summary>
        public PanGestureRecognizer PanGesture { get; private set; }

        /// <summary>
        /// Double tap gesture to zoom in and out
        /// </summary>
        public TapGestureRecognizer DoubleTapGesture { get; private set; }

        /// <summary>
        /// Get a rect that will be fully visible centered around a center point at a scale
        /// TODO: Add a function to zoom to a rect with animation
        /// </summary>
        /// <param name="scale">Scale</param>
        /// <param name="center">Center point</param>
        /// <returns>Rect that is fully visible at scale and center point</returns>
        public Rect ZoomRectForScaleAndCenter(float scale, Vector2 center)
        {
            Rect zoomRect = new Rect();
            Rect rtRect = contentRectTransform.rect;
            scale = Mathf.Clamp(scale, MinimumScale, MaximumScale);

            // The zoom rect is in the content view's coordinates. 
            // At a zoom scale of 1.0, it would be the size of the content bounds.
            // As the zoom scale decreases, so more content is visible, the size of the rect grows.
            zoomRect.height = rtRect.height / scale;
            zoomRect.width = rtRect.width / scale;

            // choose an origin so as to get the right center.
            zoomRect.xMin = center.x - (zoomRect.width / 2.0f);
            zoomRect.yMin = center.y - (zoomRect.height / 2.0f);

            return zoomRect;
        }

        private void WriteDebug(string text, params object[] args)
        {

#if UNITY_EDITOR

            if (DebugText != null && DebugText.isActiveAndEnabled)
            {
                if (DebugText.text.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.None).Length > 38)
                {
                    DebugText.text = string.Empty;
                }
                DebugText.text += string.Format(text + System.Environment.NewLine, args);
            }

#endif

        }

        private void HandleDoubleTap()
        {
            // handle double tap
            if (doubleTapScaleTimeSecondsRemaining > 0.0f)
            {
                doubleTapScaleTimeSecondsRemaining = Mathf.Max(0.0f, doubleTapScaleTimeSecondsRemaining - Time.deltaTime);
                float lerp = 1.0f - (doubleTapScaleTimeSecondsRemaining / DoubleTapAnimationTimeSeconds);
                float scaleValue = Mathf.Lerp(doubleTapScaleStart, doubleTapScaleEnd, lerp);
                contentRectTransform.localScale = new Vector3(scaleValue, scaleValue, 1.0f);
                contentRectTransform.anchoredPosition = Vector2.Lerp(doubleTapPosStart, doubleTapPosEnd, lerp);
            }
        }

        private void HandlePan()
        {
            contentRectTransform.anchoredPosition += (panVelocity * Time.deltaTime);
            panVelocity *= PanDampening;
        }

        private void HandleZoom()
        {
            zoomSpeed = Mathf.Lerp(zoomSpeed, 1.0f, ScaleDampening);

            float scaleMultiplier = ScrollContent.transform.localScale.x * zoomSpeed;
            if (ScaleGesture.State != GestureRecognizerState.Executing)
            {
                // bounce back zoom scale
                if (scaleMultiplier > MaximumScale)
                {
                    scaleMultiplier = Mathf.Lerp(scaleMultiplier, MaximumScale, BounceModifier);
                }
                else if (scaleMultiplier < MinimumScale)
                {
                    scaleMultiplier = Mathf.Lerp(scaleMultiplier, MinimumScale, BounceModifier);
                }
            }
            else
            {
                // clamp to a little beyond the max, will snap back when scale gesture finishes
                scaleMultiplier = Mathf.Clamp(scaleMultiplier, MinimumScale * 0.75f, MaximumScale * 1.333f);
            }

            // keep track of where the scale gesture is in local coordinates and then where it moves to after the scale is applied
            //  we need to adjust position with the difference so that the content stays right where it was under the scale gesture
            Vector2 pointBeforeScale, pointAfterScale;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(contentRectTransform, lastScaleFocus, CanvasCamera, out pointBeforeScale);
            contentRectTransform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1.0f);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(contentRectTransform, lastScaleFocus, CanvasCamera, out pointAfterScale);
            contentRectTransform.anchoredPosition += ((pointAfterScale - pointBeforeScale) * scaleMultiplier);
        }

        private Rect ScaleRect(Rect rect, float scaleX, float scaleY)
        {
            float newWidth = rect.width * scaleX;
            float newHeight = rect.height * scaleY;
            rect.xMin -= ((newWidth - rect.width) * 0.5f);
            rect.yMin -= ((newHeight - rect.height) * 0.5f);
            rect.width = newWidth;
            rect.height = newHeight;
            return rect;
        }

        private void EnsureVisible()
        {
            // ensure content is visible on screen
            Rect rect = ScaleRect(contentRectTransform.rect, contentRectTransform.localScale.x, contentRectTransform.localScale.y);
            Rect visibleRect = containerRectTransform.rect;
            Vector2 pos = contentRectTransform.anchoredPosition;
            rect.position += pos;

            // handle x pos
            if (rect.width <= visibleRect.width)
            {
                pos.x = visibleRect.center.x;
            }
            else if (rect.xMin > visibleRect.xMin)
            {
                pos.x -= (rect.xMin - visibleRect.xMin);
            }
            else if (rect.xMax < visibleRect.xMax)
            {
                pos.x += (visibleRect.xMax - rect.xMax);
            }

            // handle y pos
            if (rect.height <= visibleRect.height)
            {
                pos.y = visibleRect.center.y;
            }
            else if (rect.yMin > visibleRect.yMin)
            {
                pos.y -= (rect.yMin - visibleRect.yMin);
            }
            else if (rect.yMax < visibleRect.yMax)
            {
                pos.y += (visibleRect.yMax - rect.yMax);
            }

            contentRectTransform.anchoredPosition = Vector2.Lerp(contentRectTransform.anchoredPosition, pos, BounceModifier);
        }

        private void Start()
        {
            contentRectTransform = ScrollContent.GetComponent<RectTransform>();
            containerRectTransform = ScrollContentContainer.GetComponent<RectTransform>();

            // create the scale, tap and pan gestures that will manage the scroll view
            ScaleGesture = new ScaleGestureRecognizer();
            ScaleGesture.StateUpdated += Scale_Updated;
            ScaleGesture.PlatformSpecificView = ScrollContentContainer;
            ScaleGesture.ThresholdUnits = 0.0f; // start zooming immediately
            FingersScript.Instance.AddGesture(ScaleGesture);

            DoubleTapGesture = new TapGestureRecognizer();
            DoubleTapGesture.NumberOfTapsRequired = 2;
            DoubleTapGesture.StateUpdated += Tap_Updated;
            DoubleTapGesture.PlatformSpecificView = ScrollContentContainer;
            FingersScript.Instance.AddGesture(DoubleTapGesture);

            PanGesture = new PanGestureRecognizer();
            PanGesture.MaximumNumberOfTouchesToTrack = 2;
            PanGesture.StateUpdated += Pan_Updated;
            PanGesture.AllowSimultaneousExecution(ScaleGesture);
            PanGesture.PlatformSpecificView = ScrollContentContainer;
            FingersScript.Instance.AddGesture(PanGesture);
        }

        private void LateUpdate()
        {
            HandleDoubleTap();
            HandlePan();
            HandleZoom();
            EnsureVisible();
        }

        private void Tap_Updated(GestureRecognizer gesture)
        {
            if (doubleTapScaleTimeSecondsRemaining == 0.0f && gesture.State == GestureRecognizerState.Ended)
            {
                doubleTapScaleStart = contentRectTransform.localScale.x;
                doubleTapScaleTimeSecondsRemaining = DoubleTapAnimationTimeSeconds;
                if (ScrollContent.transform.localScale.x >= DoubleTapZoomOutThreshold)
                {
                    doubleTapScaleEnd = Mathf.Min(MaximumScale, DoubleTapZoomOutValue);
                }
                else
                {
                    doubleTapScaleEnd = Mathf.Max(MinimumScale, DoubleTapZoomInValue);
                }
                doubleTapPosStart = contentRectTransform.anchoredPosition;
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(contentRectTransform, new Vector2(gesture.FocusX, gesture.FocusY), CanvasCamera, out localPoint);
                doubleTapPosEnd = localPoint * -doubleTapScaleEnd;
            }
        }

        private void Scale_Updated(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing)
            {
                float scale = (gesture as ScaleGestureRecognizer).ScaleMultiplier;

                if (scale >= 0.999f && scale <= 1.001f)
                {
                    return;
                }
                else if (scale > 1.0f)
                {
                    zoomSpeed += (scale * ScaleSpeed);
                }
                else if (scale < 1.0f)
                {
                    zoomSpeed -= ((1.0f / scale) * ScaleSpeed);
                }
                lastScaleFocus = new Vector2(gesture.FocusX, gesture.FocusY);
            }

            WriteDebug("Scale: {0},{1}", gesture.State, (gesture as ScaleGestureRecognizer).ScaleMultiplier);
        }

        private void Pan_Updated(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing)
            {
                contentRectTransform.anchoredPosition += new Vector2(gesture.DeltaX, gesture.DeltaY);
            }
            else if (gesture.State == GestureRecognizerState.Ended)
            {
                panVelocity = new Vector2(gesture.VelocityX, gesture.VelocityY);
                panVelocity.x = Mathf.Clamp(panVelocity.x, -MaxSpeed, MaxSpeed);
                panVelocity.y = Mathf.Clamp(panVelocity.y, -MaxSpeed, MaxSpeed);
            }
        }
    }
}