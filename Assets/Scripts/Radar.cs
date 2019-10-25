using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Radar
{
    public class Radar : MonoBehaviour
    {
        public GameObject Ring;

        public float Interval = 5.0f;
        public float StartSeconds = 0.0f;

        private Options Opts = new Options();

        public int Segments = 64;
        public float Width = 0.1f;
        public float StartRadius = 0.0f;
        public float EndRadius = 10.0f;
        public float Rate = 5.0f;

        private float nextEvent;

        public bool IsActive { get; set; }

        void Start()
        {
            this.nextEvent = this.StartSeconds;

            this.Opts.Segments = Segments;
            this.Opts.Width = Width;
            this.Opts.StartRadius = StartRadius;
            this.Opts.EndRadius = EndRadius;
            this.Opts.Rate = Rate;
        }

        void Update()
        {
            if (Time.time < this.nextEvent)
            {
                return;
            }

            this.nextEvent += this.Interval;
            if (IsActive)
            {
                GameObject r = (GameObject)Instantiate(this.Ring, this.transform);
                r.GetComponent<Ring>().Opts = this.Opts;
            }
        }

        public void Clear()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        }
    }

    public class Options
    {
        public int Segments;
        public float Width;
        public float StartRadius;
        public float EndRadius;
        public float Rate;
    }
}