using System;
using UnityEngine;

namespace FPSMovementController
{
    public class Counter
    {
        private readonly Func<bool> checkForReset;
        private readonly float time;
        private float endTime;
        private bool wasStoped = false;

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
            wasStoped = false;
        }

        public void Update()
        {
            if (checkForReset())
            {
                Stop();
            }
            else if(Ended && wasStoped)
            {
                Set();
            }
        }

        public void Stop()
        {
            endTime = Time.time;
            wasStoped = true;
        }
    }
}
