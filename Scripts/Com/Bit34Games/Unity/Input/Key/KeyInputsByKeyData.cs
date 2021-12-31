using System;
using System.Collections.Generic;

namespace Com.Bit34Games.Unity.Input
{
    public class KeyInputsByKeyData
    {
        //  MEMBERS
        public readonly int keyCode;
        //      Internal
        private List<KeyInputData> _keyInputList;

        //  CONSTRUCTORS
        public KeyInputsByKeyData(int keyCode)
        {
            this.keyCode  = keyCode;
            _keyInputList = new List<KeyInputData>();
        }

        //  METHODS
        public void AddKeyInput(KeyInputData keyInput)
        {
            _keyInputList.Add(keyInput);
            UpdateExcludedModifierKeys();
        }

        public void RemoveKeyInput(KeyInputData keyInput)
        {
            _keyInputList.Remove(keyInput);
            UpdateExcludedModifierKeys();
        }

        private void UpdateExcludedModifierKeys()
        {
            HashSet<int> allModifierKeyCodes = new HashSet<int>();

            for (int k = 0; k < _keyInputList.Count; k++)
            {
                KeyInputData keyInput = _keyInputList[k];
                if (keyInput.modifierKeyCodes != null)
                {
                    for (int m = 0; m < keyInput.modifierKeyCodes.Length; m++)
                    {
                        allModifierKeyCodes.Add(keyInput.modifierKeyCodes[m]);
                    }
                }
            }

            for (int k = 0; k < _keyInputList.Count; k++)
            {
                KeyInputData keyInput = _keyInputList[k];
                List<int>    excludedModifierKeyCodes = new List<int>();

                foreach (int modifierKeyCode in allModifierKeyCodes)
                {
                    if (keyInput.modifierKeyCodes == null ||
                        Array.IndexOf(keyInput.modifierKeyCodes, modifierKeyCode) == -1)
                    {
                        excludedModifierKeyCodes.Add(modifierKeyCode);
                    }
                }

                keyInput.excludedModifierKeyCodes = excludedModifierKeyCodes.ToArray();
            }
        }
        
    }
}