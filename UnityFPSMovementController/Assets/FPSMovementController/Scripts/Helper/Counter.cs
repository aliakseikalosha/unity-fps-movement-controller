using System;
using UnityEngine;

namespace FPSMovementController
{
    /// <summary>
    /// Count down timed event
    /// </summary>
    public class Counter
    {
        private readonly Func<bool> checkForReset;
        private readonly float time;
        private float endTime;
        private bool wasStoped = false;
        private bool wasStarted = false;
        /// <summary>
        /// Return true if count down ended 
        /// </summary>
        public bool Ended => endTime < Time.time;
        /// <summary>
        /// Return true while count down is running 
        /// </summary>
        public bool Running => !Ended && wasStarted;
        /// <summary>
        /// Create instance of a <c>Counter</c>
        /// </summary>
        /// <param name="checkForReset">function that would be used to check for reset during update</param>
        /// <param name="time">time to wait, before counter will end</param>
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
            wasStarted = true;
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
            wasStarted = false;
        }
    }
}
