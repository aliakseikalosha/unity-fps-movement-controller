using System;
using UnityEngine;

namespace FPSMovmentController
{
    public class Counter
    {
        private readonly Func<bool> checkForReset;
        private readonly float time;
        private float endTime;
        public bool Ended => endTime < Time.time;
        public bool Running => !Ended;

        public Counter(Func<bool> checkForReset, float time)
        {
            this.checkForReset = checkForReset;
            this.time = time;
            endTime = Time.time;
        }

        public void Set()
        {
            endTime = Time.time + time;
        }

        public void Update()
        {
            if (checkForReset())
            {
                Stop();
            }
            else if(Ended)
            {
                Set();
            }
        }

        public void Stop()
        {
            endTime = Time.time;
        }
    }
}
