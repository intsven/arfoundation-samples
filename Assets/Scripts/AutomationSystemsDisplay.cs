using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Video;

namespace Scripts
{
    public class AutomationSystemsDisplay : MonoBehaviour
    {
        [SerializeField] private ARButton _buttonBasicVideo;
        [SerializeField] private ARButton _buttonAdvancedVideo;
        [SerializeField] private VideoPlayer _screen;
        [SerializeField] private VideoClip _basicVideo;
        [SerializeField] private VideoClip _advancedVideo;

        //private Material _defaultScreenMaterial;

        void Awake()
        {
            //_defaultScreenMaterial = _screen.GetComponent<Renderer>().material;
            _buttonBasicVideo.OnTouch.AddListener(PlayBasicVideo);
            _buttonAdvancedVideo.OnTouch.AddListener(PlayAdvancedVideo);
            //_screen.loopPointReached += ResetScreenTexture;
        }

        /*private void ResetScreenTexture(VideoPlayer source)
        {
            source.GetComponent<Renderer>().material = _defaultScreenMaterial;
        }*/

        private void PlayBasicVideo()
        {
            _screen.clip = _basicVideo;
            _screen.Play();
        }

        private void PlayAdvancedVideo()
        {
            _screen.clip = _advancedVideo;
            _screen.Play();
        }
    }
}
