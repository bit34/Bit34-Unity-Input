using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.Bit34Games.Unity.Input
{
    public class KeyInputManager
    {
        //  MEMBERS
        private static List<KeyInputGroupData>                _keyInputGroups;
        private static Dictionary<int, KeyInputsByKeyData> _keyInputsByKey;

        //  CONSTRUCTORS
        public KeyInputManager()
        {
            _keyInputGroups  = new List<KeyInputGroupData>();
            _keyInputsByKey  = new Dictionary<int, KeyInputsByKeyData>();
        }

        //  METHODS
        public void AddKeyInput(int groupId, KeyInputSourceTypes groupSource, int[] modifierKeyCodes, int keyCode, bool keyStateToAction, Action action)
        {
            KeyInputGroupData     keyInputGroup  = GetOrCreateKeyInputGroup(groupId, groupSource);
            KeyInputsByKeyData keyInputsByKey = GetOrCreateKeyInputsByKey(keyCode);

            KeyInputData keyInput = new KeyInputData(keyInputGroup, modifierKeyCodes, keyCode, keyStateToAction, action);

            keyInputGroup.keyInputs.Add(keyInput);
            keyInputsByKey.AddKeyInput(keyInput);
        }
        
        public void RemoveKeyGroup(int groupId)
        {
            for (int g = 0; g < _keyInputGroups.Count; g++)
            {
                KeyInputGroupData keyInputGroup = _keyInputGroups[g];
                if (keyInputGroup.groupId == groupId)
                {
                    for (int k = 0; k <keyInputGroup.keyInputs.Count ; k++)
                    {
                        KeyInputData keyInput = keyInputGroup.keyInputs[k];
                        _keyInputsByKey[keyInput.keyCode].RemoveKeyInput(keyInput);
                    }
                    _keyInputGroups.RemoveAt(g);
                    return;
                }
            }
        }

        public void SetKeyGroupState(int groupId, bool state)
        {
            for (int g = 0; g < _keyInputGroups.Count; g++)
            {
                if (_keyInputGroups[g].groupId == groupId)
                {
                    _keyInputGroups[g].state = state;
                    return;
                }
            }
        }

        public void UpdateKeyboard()
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                return;
            }

            for (int g = 0; g < _keyInputGroups.Count; g++)
            {
                KeyInputGroupData keyInputGroup = _keyInputGroups[g];
                if (keyInputGroup.source == KeyInputSourceTypes.Keyboard &&
                    keyInputGroup.state == true)
                {
                    for (int k = 0; k < keyInputGroup.keyInputs.Count; k++)
                    {
                        KeyInputData keyInput = keyInputGroup.keyInputs[k];
                        if (keyInput.keyStateToAction == true)
                        {
                            if (UnityEngine.Input.GetKeyDown((KeyCode)keyInput.keyCode) &&
                                CheckModifierKeys(keyInput.modifierKeyCodes) &&
                                CheckExcludedModifierKeys(keyInput.excludedModifierKeyCodes))
                            {
                                keyInput.action();
                            }
                        }
                        else
                        if (keyInput.keyStateToAction == false)
                        {
                            if (UnityEngine.Input.GetKeyUp((KeyCode)keyInput.keyCode) &&
                                CheckModifierKeys(keyInput.modifierKeyCodes) &&
                                CheckExcludedModifierKeys(keyInput.excludedModifierKeyCodes))
                            {
                                keyInput.action();
                            }
                        }
                    }
                }
            }
        }

        private KeyInputGroupData GetOrCreateKeyInputGroup(int groupId, KeyInputSourceTypes groupSource)
        {
            KeyInputGroupData keyInputGroup = null;

            for (int g = 0; g < _keyInputGroups.Count; g++)
            {
                if (_keyInputGroups[g].groupId == groupId)
                {
                    keyInputGroup = _keyInputGroups[g];
                    break;
                }
            }

            if (keyInputGroup == null)
            {
                keyInputGroup = new KeyInputGroupData(groupSource, groupId);
                _keyInputGroups.Add(keyInputGroup);
            }

            return keyInputGroup;
        }

        private KeyInputsByKeyData GetOrCreateKeyInputsByKey(int keyCode)
        {
            KeyInputsByKeyData keyInputListByKey;
            if (!_keyInputsByKey.TryGetValue(keyCode, out keyInputListByKey))
            {
                keyInputListByKey = new KeyInputsByKeyData(keyCode);
                _keyInputsByKey.Add(keyCode, keyInputListByKey);
            }
            return keyInputListByKey;
        }

        private bool CheckModifierKeys(int[] modifierKeyCodes)
        {
            if (modifierKeyCodes != null)
            {
                for (int k = 0; k < modifierKeyCodes.Length; k++)
                {
                    if (UnityEngine.Input.GetKey((KeyCode)modifierKeyCodes[k]) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckExcludedModifierKeys(int[] excludedModifierKeyCodes)
        {
            if (excludedModifierKeyCodes != null)
            {
                for (int k = 0; k < excludedModifierKeyCodes.Length; k++)
                {
                    if (UnityEngine.Input.GetKey((KeyCode)excludedModifierKeyCodes[k]) == true)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
     
    }
}