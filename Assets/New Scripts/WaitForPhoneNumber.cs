using System;
using UnityEngine;

namespace CustomYields
{
    public class WaitForPhoneNumber : CustomYieldInstruction
    {
        private string target;
        private int numberIndex = 0;
        private float currentTimer;
        private float timer;
        private bool hasTimer = false;
        private bool success = true;

        private Action<WaitForPhoneNumber> onResult;

        public WaitForPhoneNumber(string target, float timer = 0f, Action<WaitForPhoneNumber> onResult = null)
        {
            this.target = target;
            this.timer = timer;
            this.onResult = onResult;

            currentTimer = timer;
            hasTimer = timer > 0f;
        }

        private void OnKeyDown(string key)
        {
            string targetKey = target[NumberIndex].ToString();
            bool isCorrect = key.Equals(targetKey);
            if (isCorrect) numberIndex = NumberIndex + 1;
        }

        public override bool keepWaiting => CheckForWaiting();

        public int NumberIndex => numberIndex;

        public string CurrentProgress => target.Substring(0, numberIndex);

        public void Clear() => numberIndex = 0;

        public bool Success => success;

        public float Timer => timer;
        public float CurrentTimer => CurrentTimer;

        private bool CheckForWaiting()
        {
            if (hasTimer)
            {
                currentTimer -= Time.deltaTime;
                if (currentTimer < 0f)
                {
                    success = false;
                    onResult?.Invoke(this);
                    Debug.Log("TIMER EXPIRED!!!!");
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(Input.inputString)) OnKeyDown(Input.inputString);
            var wait = NumberIndex < target.Length;
            
            if (!wait) onResult?.Invoke(this);
            
            return wait;
        }
    }
}