﻿using UnityEngine;
using System.Collections;

namespace DigitalRubyShared
{
    public class DemoScriptZoomableScrollView : MonoBehaviour
    {
        public GameObject ScrollContent;
        public UnityEngine.UI.Text DebugText;
        public Canvas Canvas;

        private RectTransform rt;
        private float scaleStart;
        private float scaleEnd;
        private float scaleTime;
        private float elapsedScaleTime;
        private Vector2 scalePosStart;
        private Vector2 scalePosEnd;
        private Vector2 panVelocity;

        private void WriteDebug(string text, params object[] args)
        {
            /*
            if (DebugText != null)
            {
                if (DebugText.text.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.None).Length > 38)
                {
                    DebugText.text = string.Empty;
                }
                DebugText.text += string.Format(text + System.Environment.NewLine, args);
            }
            */
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
            Rect rect = ScaleRect(rt.rect, rt.localScale.x, rt.localScale.y);
            Rect visibleRect = Canvas.GetComponent<RectTransform>().rect;
            Vector2 pos = rt.anchoredPosition;
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

            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, pos, 0.1f);
        }

        private void Start()
        {
            rt = ScrollContent.GetComponent<RectTransform>();

            ScaleGestureRecognizer scale = new ScaleGestureRecognizer();
            scale.StateUpdated += Scale_Updated;
            scale.PlatformSpecificView = ScrollContent.gameObject;
            scale.ThresholdUnits = 0.0f; // start zooming immediately
            FingersScript.Instance.AddGesture(scale);

            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.NumberOfTapsRequired = 2;
            tap.StateUpdated += Tap_Updated;
            tap.PlatformSpecificView = ScrollContent.gameObject;
            FingersScript.Instance.AddGesture(tap);

            PanGestureRecognizer pan = new PanGestureRecognizer();
            pan.MaximumNumberOfTouchesToTrack = 2;
            pan.StateUpdated += Pan_Updated;
            pan.AllowSimultaneousExecution(scale);
            pan.PlatformSpecificView = ScrollContent.gameObject;
            FingersScript.Instance.AddGesture(pan);
        }

        private void LateUpdate()
        {
            // handle double tap
            if (scaleEnd > 0.0f)
            {
                elapsedScaleTime += Time.deltaTime;
                float lerp = Mathf.Min(1.0f, elapsedScaleTime / scaleTime);
                float scaleValue = Mathf.Lerp(scaleStart, scaleEnd, lerp);
                rt.localScale = new Vector3(scaleValue, scaleValue, 1.0f);
                rt.anchoredPosition = Vector2.Lerp(scalePosStart, scalePosEnd, lerp);
                if (lerp >= 0.99f)
                {
                    scaleEnd = 0.0f;
                }
                return;
            }

            rt.anchoredPosition += (panVelocity * Time.deltaTime);
            panVelocity *= 0.95f;

            EnsureVisible();
        }

        private void Tap_Updated(GestureRecognizer gesture)
        {
            if (scaleEnd == 0.0f && gesture.State == GestureRecognizerState.Ended)
            {
                scaleStart = rt.localScale.x;
                scaleTime = 0.5f;
                elapsedScaleTime = 0.0f;
                if (ScrollContent.transform.localScale.x >= 2.5f)
                {
                    // zoom out
                    scaleEnd = 1.0f;
                }
                else
                {
                    // zoom in
                    scaleEnd = 4.0f;
                }
                scalePosStart = rt.anchoredPosition;
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, new Vector2(gesture.FocusX, gesture.FocusY), null, out localPoint);
                scalePosEnd = localPoint * -scaleEnd;
            }
        }

        private void Scale_Updated(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Began)
            {
                scalePosStart = rt.anchoredPosition;
            }
            else if (gesture.State == GestureRecognizerState.Executing)
            {
                float scale = (gesture as ScaleGestureRecognizer).ScaleMultiplier;

                if (scale >= 0.999f && scale <= 1.001f)
                {
                    return;
                }

                // make Vector3 for scale
                Vector3 newScale = ScrollContent.transform.localScale * scale;

                // don't mess with z scale
                newScale.z = 1.0f;

                // zoom and move towards the scaling position
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, new Vector2(gesture.FocusX, gesture.FocusY), null, out localPoint);
                //rt.anchoredPosition = Vector2.Lerp(scalePosStart, localPoint * -newScale.x, 0.1f);
                rt.localScale = newScale;
            }

            WriteDebug("Scale: {0},{1}", gesture.State, (gesture as ScaleGestureRecognizer).ScaleMultiplier);
        }

        private void Pan_Updated(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing)
            {
                rt.anchoredPosition += new Vector2(gesture.DeltaX, gesture.DeltaY);
            }
            else if (gesture.State == GestureRecognizerState.Ended)
            {
                panVelocity = new Vector2(gesture.VelocityX, gesture.VelocityY);
                panVelocity.x = Mathf.Clamp(panVelocity.x, -1024.0f, 1024.0f);
                panVelocity.y = Mathf.Clamp(panVelocity.y, -1024.0f, 1024.0f);
            }
        }
    }
}