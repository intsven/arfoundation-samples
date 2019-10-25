using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    [RequireComponent(typeof(RawImage))]
    public class Turntable : MonoBehaviour
    {
        [SerializeField] private string _resourcePath = "turntable";

        private Texture2D[] _frames;
        private RawImage _rawImage;
        private int _currentFrame;
        private int _framesCount;

        void Awake()
        {
            _rawImage = GetComponent<RawImage>();

            _frames = Resources.LoadAll<Texture2D>(_resourcePath);
            _framesCount = _frames.Length;
            _rawImage.texture = _frames[0];
        }

        void Update()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                var delta = touch.deltaPosition;
                var framesOffset = Mathf.RoundToInt(delta.x / 6f);

                if (framesOffset != 0)
                {
                    _currentFrame += framesOffset;
                    if (_currentFrame >= _framesCount)
                        _currentFrame = _currentFrame - _framesCount;
                    else if (_currentFrame < 0)
                        _currentFrame = _framesCount + _currentFrame;

                    _rawImage.texture = _frames[_currentFrame];
                }      
            }
        }

        void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
        }
    }
}