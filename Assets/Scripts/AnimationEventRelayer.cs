using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class AnimationEventRelayer : MonoBehaviour
    {
        public UnityEvent Event1;
        public UnityEvent Event2;
        public UnityEvent Event3;

        public void DispatchEvent1()
        {
            Event1.Invoke();
        }

        public void DispatchEvent2()
        {
            Event2.Invoke();
        }

        public void DispatchEvent3()
        {
            Event3.Invoke();
        }
    }
}
