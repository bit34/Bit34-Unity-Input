using System;
using UnityEngine;

namespace Com.Bit34Games.Unity.Input
{
    public class InputManager
    {
        //	MEMBERS
        public static int PointerCount { get { return _pointerInputManager.PointerCount; } }
        //      Internal
        private static InputManagerComponent _component;
        private static InputHandlers        _handlers;
        private static PointerInputManager  _pointerInputManager;
        private static KeyInputManager      _keyInputManager;

        //  METHODS
        public static void Initialize()
        {
            Initialize(PointerInputConstants.DEFAULT_COLLIDER_MASK,
                       PointerInputConstants.DEFAULT_UI_BLOCKS_POINTERS,
                       PointerInputConstants.DEFAULT_CLICK_CANCEL_MOVEMENT_PIXELS,
                       PointerInputConstants.DEFAULT_CLICK_CANCEL_TIMEOUT_SECONDS);
        }

        public static void Initialize(int colliderMask, bool uiBlocksPointers, float clickCancelMovement, float clickCancelTimeout)
        {
            if (_component != null)
            {
                throw new Exception("Input Manager already initialized");
            }

            GameObject updaterObject = new GameObject("[InputManager]");
            GameObject.DontDestroyOnLoad(updaterObject);
            _component = updaterObject.AddComponent<InputManagerComponent>();
            _component.Init(UpdateMethod);

            _handlers            = new InputHandlers();
            _pointerInputManager = new PointerInputManager(_handlers);
            _keyInputManager     = new KeyInputManager();

            _pointerInputManager.SetOptions(colliderMask, uiBlocksPointers, clickCancelMovement, clickCancelTimeout);
        }

        public static void SetColliderMask(int colliderMask)
        {
            _pointerInputManager.SetColliderMask(colliderMask);
        }

        public static void AddPointerHandler(IPointerInputHandler handler)
        {
            _handlers.AddPointerHandler(handler);
        }

        public static void RemovePointerHandler(IPointerInputHandler handler)
        {
            _handlers.RemovePointerHandler(handler);
        }

        public static bool GetPointerPosition(int pointerId, out Vector2 startPosition, out Vector2 currentPosition)
        {
            PointerInputData pointerData = _pointerInputManager.GetPointerData(pointerId);
            if (pointerData != null)
            {
                startPosition  = pointerData.StartPosition;
                currentPosition = pointerData.CurrentPosition;
                return true;
            }
            startPosition = Vector2.zero;
            currentPosition = Vector2.zero;
            return false;
        }
        
        public static GameObject GetObjectUnderPointer(int pointerId)
        {
            PointerInputData pointerData = _pointerInputManager.GetPointerData(pointerId);
            if (pointerData != null)
            {
                return pointerData.ObjectUnder;
            }
            return null;
        }

        public static void AddGestureHandler(IGestureInputHandler handler)
        {
            _handlers.AddGestureHandler(handler);
        }

        public static void RemoveGestureHandler(IGestureInputHandler handler)
        {
            _handlers.RemoveGestureHandler(handler);
        }

#region Keyboard Methods
     
        public static void AddKeyboardInput(int groupId, KeyCode[] modifierKeyCodes, KeyCode keyCode, bool keyStateToAction, Action action)
        {
            int   intKeyCode          = (int)keyCode;
            int[] intModifierKeyCodes = null;
            if (modifierKeyCodes != null)
            {
                intModifierKeyCodes = new int[modifierKeyCodes.Length];
                for (int i = 0; i < modifierKeyCodes.Length; i++) { intModifierKeyCodes[i] = (int)modifierKeyCodes[i]; }
            }

            _keyInputManager.AddKeyInput(groupId, KeyInputSourceTypes.Keyboard, intModifierKeyCodes, intKeyCode, keyStateToAction, action);
        }

        public static void RemoveKeyGroup(int groupId)
        {
            _keyInputManager.RemoveKeyGroup(groupId);
        }

        public static void SetKeyGroupState(int groupId, bool state)
        {
            _keyInputManager.SetKeyGroupState(groupId, state);
        }

#endregion

        public static void Internal_ObjectDestroyed(InputObjectComponent inputObject)
        {
            _pointerInputManager.ObjectDestroyed(inputObject);
        }

        private static void UpdateMethod()
        {
            _pointerInputManager.UpdatePointers();
            _keyInputManager.UpdateKeyboard();
        }

    }
}