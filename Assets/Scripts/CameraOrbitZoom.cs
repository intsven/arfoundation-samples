using System.Collections;
using DigitalRubyShared;
using UnityEngine;

namespace Scripts
{
    public class CameraOrbitZoom : FingersPanOrbitComponentScript
    {
        private bool _isTweening;

        void OnTriggerEnter()
        {
            if (!IsTweening())
                TweenZoomOut();

            _isTweening = true;
        }

        protected override void LateUpdate()
        {
            if (!IsTweening())
                base.LateUpdate();
        }

        protected override void PanGesture_Updated(GestureRecognizer gesture)
        {
            if (!_isTweening)
                base.PanGesture_Updated(gesture);
        }

        private void TweenZoomOut()
        {
            Vector3 targetPos = OrbitTarget.transform.position;
            Vector3 orbitPos = Orbiter.transform.position;
            Vector3 dirFromTarget = (orbitPos - targetPos).normalized;
            Vector3 tweenTarget = targetPos + dirFromTarget * MaximumDistance;
            LeanTween.move(gameObject, tweenTarget, 1f).setEaseOutCubic().setOnComplete(x => { _isTweening = false; });

            _isTweening = true;
            panVelocity = Vector2.zero;
            zoomSpeed = 0f;
        }

        private bool IsTweening()
        {
            return _isTweening;
        }
    }
}
