using System;
using UnityEngine;

namespace FPSMovmentController
{
    public abstract class UserInputProxy : MonoBehaviour
    {
        public event Action OnToggleCursor;
        public abstract Vector2 Move { get; protected set; }
        public abstract Vector2 Look { get; protected set; }
        public abstract bool Jump { get; protected set; }
        public abstract bool Crouch { get; protected set; }
        public abstract bool IsSprint { get; protected set; }

        protected void RaiseOnToggleCursor()
        {
            OnToggleCursor?.Invoke();
        }
    }
}
