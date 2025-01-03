﻿using System;
using UnityEngine;

namespace FPSMovementController
{
    public abstract class UserInputProxy : MonoBehaviour
    {
        public event Action OnToggleCursor;
        public abstract Vector2 Move { get; protected set; }
        public abstract Vector2 Look { get; protected set; }
        public abstract bool Jump { get; protected set; }
        public abstract bool Crouch { get; protected set; }
        public abstract bool Sprint { get; protected set; }

        protected void RaiseOnToggleCursor()
        {
            OnToggleCursor?.Invoke();
        }
    }
}
