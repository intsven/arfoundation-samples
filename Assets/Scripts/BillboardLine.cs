using UnityEngine;

namespace Scripts
{
    [RequireComponent(typeof(LineRenderer))]
    public class BillboardLine : MonoBehaviour
    {
        [SerializeField] private Transform _handleLeft;
        [SerializeField] private Transform _handleRight;
        [SerializeField] private Transform _handleTop;
        [SerializeField] private Transform _handleBottom;
        [SerializeField] private Transform _target;
        [SerializeField] private string _sortingLayerName = "Default";
        [SerializeField] private int _sortingOrder = 0;
        [SerializeField] private bool _directLines;

        private LineRenderer _lineRenderer;
        private bool _isInvisible;

        void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.sortingLayerName = _sortingLayerName;
            _lineRenderer.sortingOrder = _sortingOrder;
        }

        void LateUpdate()
        {
            var camera = Camera.main;
            var ownPos = camera.WorldToScreenPoint(_handleLeft.transform.position);
            var targetPos = camera.WorldToScreenPoint(_target.transform.position);
            var isHorizontal = true;

            if (ownPos.x < targetPos.x)
            {
                ownPos = camera.WorldToScreenPoint(_handleRight.transform.position);
                if (ownPos.x > targetPos.x)
                {
                    isHorizontal = false;
                    ownPos = camera.WorldToScreenPoint(_handleBottom.transform.position);
                    if (ownPos.y < targetPos.y)
                    {
                        ownPos = camera.WorldToScreenPoint(_handleTop.transform.position);
                        if (ownPos.y > targetPos.y)
                        {
                            HideLine();
                            return;
                        }
                    }        
                }
            }

            if (_isInvisible)
                ShowLine();

            if (_directLines)
                _lineRenderer.SetPositions(new[] { camera.ScreenToWorldPoint(ownPos), camera.ScreenToWorldPoint(targetPos), camera.ScreenToWorldPoint(targetPos) });
            else
            {
                var inbetweenPoint = isHorizontal ? new Vector3(targetPos.x, ownPos.y, ownPos.z) : new Vector3(ownPos.x, targetPos.y, ownPos.z);
                _lineRenderer.SetPositions(new[] { camera.ScreenToWorldPoint(ownPos), camera.ScreenToWorldPoint(inbetweenPoint), camera.ScreenToWorldPoint(targetPos) });
            } 
        }

        public void SetAlpha(float alpha)
        {
            var color = new Color(_lineRenderer.startColor.r, _lineRenderer.startColor.g, _lineRenderer.startColor.b, alpha);
            _lineRenderer.startColor = color;
            _lineRenderer.endColor = color;
        }

        private void HideLine()
        {
            _lineRenderer.enabled = false;
            _isInvisible = true;
        }

        private void ShowLine()
        {
            _lineRenderer.enabled = true;
            _isInvisible = false;
        }
    }
}
