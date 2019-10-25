using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts;
using UnityEngine;

namespace Scripts
{
    public class MELTLogoAnimationsController : MonoBehaviour
    {
        [SerializeField] private Animator _meltLogo;

        [SerializeField] private ARButton _buttonMechanical;
        [SerializeField] private ARButton _buttonThermal;
        [SerializeField] private ARButton _buttonLogical;
        [SerializeField] private ARButton _buttonElectrical;

        [SerializeField] private GameObject _buttonsContainer;
        [SerializeField] private GameObject _electricalTextImage;
        [SerializeField] private GameObject _logicalTextImage;
        [SerializeField] private GameObject _mechanicalTextImage;
        [SerializeField] private GameObject _thermalTextImage;

        [SerializeField] private ARButton _backButton;

        public enum State
        {
            Logo,
            Electrical,
            Mechanical,
            Logical,
            Thermal
        }

        private State _currentState = State.Logo;

        private Dictionary<State, string> _stateColorMap = new Dictionary<State, string>()
        {
            {State.Electrical, "darkBlue"},
            {State.Logical, "brightBlue"},
            {State.Mechanical, "brightRed"},
            {State.Thermal, "darkRed"}
        };

        void Awake()
        {
            _buttonMechanical.OnTouch.AddListener(ShowMechanical);
            _buttonThermal.OnTouch.AddListener(ShowThermal);
            _buttonLogical.OnTouch.AddListener(ShowLogical);
            _buttonElectrical.OnTouch.AddListener(ShowElectrical);

            _backButton.OnTouch.AddListener(ReturnToMeltLogo);

            var animationEventRelayer = _meltLogo.GetComponent<AnimationEventRelayer>();
            animationEventRelayer.Event1.AddListener(ShowCurrentTextImage);
            animationEventRelayer.Event2.AddListener(HideCurrentTextImage);
            animationEventRelayer.Event3.AddListener(EnableMeltButtons);
        }

        void OnDisable()
        {
            _buttonMechanical.OnTouch.RemoveListener(ShowMechanical);
            _buttonThermal.OnTouch.RemoveListener(ShowThermal);
            _buttonLogical.OnTouch.RemoveListener(ShowLogical);
            _buttonElectrical.OnTouch.RemoveListener(ShowElectrical);

            _backButton.OnTouch.RemoveListener(ReturnToMeltLogo);

            var animationEventRelayer = _meltLogo.GetComponent<AnimationEventRelayer>();
            animationEventRelayer.Event1.RemoveListener(ShowCurrentTextImage);
            animationEventRelayer.Event2.RemoveListener(HideCurrentTextImage);
            animationEventRelayer.Event3.RemoveListener(EnableMeltButtons);
        }

        private void ShowElectrical()
        {
            SetState(State.Electrical);
        }

        private void ShowLogical()
        {
            SetState(State.Logical);
        }

        private void ShowThermal()
        {
            SetState(State.Thermal);
        }

        private void ShowMechanical()
        {
            SetState(State.Mechanical);
        }

        private void ReturnToMeltLogo()
        {
            SetState(State.Logo);
        }

        private void SetState(State state)
        {
            if (state == State.Logo)
            {
                SetLogoAnimationOut();
                HideBackButton();
            }
            else
            {
                SetLogoAnimationIn(state);
                _currentState = state;
            }
        }

        private void ShowCurrentTextImage()
        {
            ShowTextImage(_currentState);
            DisableMeltButtons();
            ShowBackButton();
        }

        private void HideCurrentTextImage()
        {
            _electricalTextImage.SetActive(false);
            _logicalTextImage.SetActive(false);
            _mechanicalTextImage.SetActive(false);
            _thermalTextImage.SetActive(false);
        }

        private void ShowTextImage(State state)
        {
            _electricalTextImage.SetActive(state == State.Electrical);
            _logicalTextImage.SetActive(state == State.Logical);
            _mechanicalTextImage.SetActive(state == State.Mechanical);
            _thermalTextImage.SetActive(state == State.Thermal);
        }

        private void SetLogoAnimationIn(State state)
        {
            _meltLogo.SetBool(_stateColorMap[state], true);
        }

        private void SetLogoAnimationOut()
        {
            _meltLogo.SetBool(_stateColorMap[_currentState], false);
        }

        private void DisableMeltButtons()
        {
            _buttonsContainer.SetActive(false);
        }

        private void EnableMeltButtons()
        {
            _buttonsContainer.SetActive(true);
        }

        private void ShowBackButton()
        {
            _backButton.gameObject.SetActive(true);
        }

        private void HideBackButton()
        {
            _backButton.gameObject.SetActive(false);
        }
    }
}