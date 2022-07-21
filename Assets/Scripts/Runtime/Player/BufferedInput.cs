using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MovementGame.Input
{
    /// <summary>
    /// A simple class to buffer input. Can be implicitly cast to bool, or explicitly used via TryConsume() (the same thing but less syntax sugar)
    /// </summary>
    [Serializable] //needs to be serializable for Unity.
    public class BufferedInput
    {
        [SerializeField, Range(0.0f, 0.4f)]
        private float bufferDuration;

        private bool active = false;
        private float activeTime = 0.0f;

        /// <summary>
        /// Activates this input buffer.
        /// </summary>
        public void Set()
        {
            active = true;
            activeTime = Time.time;
        }

        /// <summary>
        /// Activates this input buffer. Uses time as a parameter in case it's used already since Time.time has wonky execution time. Not really worth using tho.
        /// </summary>
        public void Set(float time)
        {
            active = true;
            activeTime = time;
        }

        /// <summary>
        /// checks: active && Time.time - activeTime <= bufferDuration;
        /// </summary>
        public bool Get() => active && Time.time - activeTime <= bufferDuration;

        /// <summary>
        /// Consumes the buffered input.
        /// </summary>
        public void Consume() => active = false;

        /// <summary>
        /// Try to consume the input. Always resets activeInput to false when requirements are met.
        /// </summary>
        /// <returns>true when input is activated and in an acceptable timeframe.</returns>
        public bool TryConsume()
        {
            if (active && Time.time - activeTime <= bufferDuration)
            {
                active = false;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(BufferedInput input)
        {
            return input.Get();
        }
    }
}