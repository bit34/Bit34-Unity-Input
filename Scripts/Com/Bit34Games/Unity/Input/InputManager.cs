using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.Bit34Games.Unity.Input
{
    public class InputManager
    {
        //  CONSTANTS
        private const int UNITY_MOUSE_LEFT_BUTTON   = 0;
        private const int UNITY_MOUSE_RIGHT_BUTTON  = 1;
        private const int UNITY_MOUSE_MIDDLE_BUTTON = 2;

        //	MEMBERS
        public static bool UsingMouse
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                return true;
#else
                return false;
#endif
            }
        }
        public static int PointerCount
        {
            get { return _pointers.Count; }
        }
        //      Internal
        private static int                          _colliderMask;
        private static bool                         _uiBlocksPointers;
        private static float                        _clickCancelMovement;
        private static float                        _clickCancelTimeout;
        private static InputManagerComponent        _component;
        private static LinkedList<PointerInputData> _pointers;
        private static List<IPointerInputHandler>   _pointerHandlers;
        private static List<IGestureInputHandler>   _gestureHandlers;
        private static PointerInputData             _mousePointerData;
        private static List<KeyInputGroupData>      _keyInputGroups;

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

            _colliderMask        = colliderMask;
            _uiBlocksPointers    = uiBlocksPointers;
            _clickCancelMovement = clickCancelMovement;
            _clickCancelTimeout  = clickCancelTimeout;

            GameObject updaterObject = new GameObject("[InputManager]");
            GameObject.DontDestroyOnLoad(updaterObject);
            _component = updaterObject.AddComponent<InputManagerComponent>();
            _component.Init(UpdateMethod);

            _pointers        = new LinkedList<PointerInputData>();
            _pointerHandlers = new List<IPointerInputHandler>();
            _gestureHandlers = new List<IGestureInputHandler>();
            _keyInputGroups  = new List<KeyInputGroupData>();

            if (UsingMouse)
            {
                InitMouse();
            }
        }

        public static void SetColliderMask(int colliderMask)
        {
            _colliderMask = colliderMask;
        }

        public static void AddPointerHandler(IPointerInputHandler handler)
        {
            _pointerHandlers.Add(handler);
        }

        public static void RemovePointerHandler(IPointerInputHandler handler)
        {
            _pointerHandlers.Remove(handler);
        }

        public static bool GetPointerPosition(int pointerId, out Vector2 startPosition, out Vector2 currentPosition)
        {
            PointerInputData pointerData = GetPointerData(pointerId);
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

        public static void AddGestureHandler(IGestureInputHandler handler)
        {
            _gestureHandlers.Add(handler);
        }

        public static void RemoveGestureHandler(IGestureInputHandler handler)
        {
            _gestureHandlers.Remove(handler);
        }

        public static void AddKeyboardInput(int groupId, KeyCode[] modifierKeyCodes, KeyCode keyCode, bool keyStateToAction, Action action)
        {
            KeyInputGroupData group = null;

            for (int g = 0; g < _keyInputGroups.Count; g++)
            {
                if (_keyInputGroups[g].groupId == groupId)
                {
                    group = _keyInputGroups[g];
                    break;
                }
            }

            if (group == null)
            {
                group = new KeyInputGroupData(KeyInputSourceTypes.Keyboard, groupId);
                _keyInputGroups.Add(group);
            }

            int[] intModifierKeyCodes = null;
            if (modifierKeyCodes != null)
            {
                intModifierKeyCodes = new int[modifierKeyCodes.Length];
                for (int i = 0; i < modifierKeyCodes.Length; i++) { intModifierKeyCodes[i] = (int)modifierKeyCodes[i]; }
            }

            group.keyInputs.Add(new KeyInputData(intModifierKeyCodes, (int)keyCode, keyStateToAction, action));
        }

        public static void RemoveKeyGroup(int groupId)
        {
            for (int g = 0; g < _keyInputGroups.Count; g++)
            {
                if (_keyInputGroups[g].groupId == groupId)
                {
                    _keyInputGroups.RemoveAt(g);
                    return;
                }
            }
        }

        public static void SetKeyGroupState(int groupId, bool state)
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

        public static void Internal_ObjectDestroyed(InputObjectComponent inputObject)
        {
            LinkedListNode<PointerInputData> pointerNode = _pointers.First;
            while(pointerNode != null)
            {
                if (pointerNode.Value.ObjectUnder == inputObject.gameObject)
                {
                    pointerNode.Value.UpdateObjectUnder(null);

                    //  Pointer leaves destroyed object
                    SendPointerLeave(pointerNode.Value.pointerId, pointerNode.Value.CurrentPosition, null);
                    
                    //  Cancel click when object destroyed??
                }
                pointerNode = pointerNode.Next;
            }
        }

        private static void UpdateMethod()
        {
            if (UsingMouse)
            {
                UpdateMouse();
            }
            UpdateKeyboard();
        }

#region Helpers

        private static GameObject GetObjectUnderPointer(Vector2 pointerScreenPosition)
        {
            GameObject hitObject = null;

            UnityEngine.Camera camera      = UnityEngine.Camera.main;
            Ray                ray         = camera.ScreenPointToRay(pointerScreenPosition);
            float              maxDistance = float.MaxValue;
            // Debug.DrawRay(ray.origin, ray.direction, Color.magenta, 1);

            RaycastHit hit3D;
            Physics.Raycast(ray.origin, ray.direction, out hit3D, float.MaxValue, _colliderMask);
            if (hit3D.collider != null)
            {
                hitObject   = hit3D.collider.gameObject;
                maxDistance = hit3D.distance;
            }

            RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, maxDistance, _colliderMask);
            if (hit2D.collider != null)
            {
                hitObject   = hit2D.collider.gameObject;
                maxDistance = hit2D.distance;
            }

            if (hitObject != null)
            {
                InputObjectComponent inputObject = hitObject.GetComponent<InputObjectComponent>();
                if (inputObject != null && inputObject.IsInputEnabled)
                {
                    return hitObject;
                }
            }

            return null;
        }

#endregion

#region Mouse Methods
     
        private static void InitMouse()
        {
            _mousePointerData = new PointerInputData(PointerInputConstants.MOUSE_POINTER_ID, DateTime.UtcNow, Vector2.zero, null);
            _mousePointerData.ClickCanceled();
            _pointers.AddLast(_mousePointerData);
        }

        private static void UpdateMouse()
        {
            Vector2    newMousePosition      = UnityEngine.Input.mousePosition;
            GameObject newObjectUnderPointer = GetObjectUnderPointer(UnityEngine.Input.mousePosition);

            UpdatePointer(_mousePointerData, newMousePosition, newObjectUnderPointer);

            CheckMouseButton(UNITY_MOUSE_LEFT_BUTTON,   PointerInputConstants.MOUSE_LEFT_DRAG_POINTER_ID,   newMousePosition, newObjectUnderPointer);
            CheckMouseButton(UNITY_MOUSE_RIGHT_BUTTON,  PointerInputConstants.MOUSE_RIGHT_DRAG_POINTER_ID,  newMousePosition, newObjectUnderPointer);
            CheckMouseButton(UNITY_MOUSE_MIDDLE_BUTTON, PointerInputConstants.MOUSE_MIDDLE_DRAG_POINTER_ID, newMousePosition, newObjectUnderPointer);

            CheckMouseWheel();
        }

        private static void CheckMouseButton(int buttonId, int pointerId, Vector2 newPosition, GameObject newObjectUnderPointer)
        {
            //  Mouse button just pressed
            if (UnityEngine.Input.GetMouseButtonDown(buttonId))
            {
                //  Should not be tracked?
                if (_uiBlocksPointers && 
                    (EventSystem.current!=null && EventSystem.current.IsPointerOverGameObject()))
                {
                    return;
                }

                CreatePointer(pointerId, newPosition, newObjectUnderPointer);
            }
            else 
            if (UnityEngine.Input.GetMouseButtonUp(buttonId))
            {
                PointerInputData pointerData = GetPointerData(pointerId);
                if (pointerData != null)
                {
                    RemovePointer(pointerData, newPosition, newObjectUnderPointer);
                }
            }
            else
            {
                PointerInputData pointerData = GetPointerData(pointerId);
                if (pointerData != null)
                {
                    UpdatePointer(pointerData, newPosition, newObjectUnderPointer);
                }
            }
        }

        private static void CheckMouseWheel()
        {
            //  Should not be tracked?
            if (_uiBlocksPointers && 
                (EventSystem.current!=null && EventSystem.current.IsPointerOverGameObject()))
            {
                return;
            }

            if (UnityEngine.Input.mouseScrollDelta.magnitude != 0)
            {
                SendScroll(UnityEngine.Input.mouseScrollDelta);
            }
            
        }

#endregion

#region Pointer Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PointerInputData GetPointerData(int pointerId)
        {
            LinkedListNode<PointerInputData> pointerNode = _pointers.First;
            while (pointerNode != null)
            {
                if (pointerNode.Value.pointerId == pointerId)
                {
                    return pointerNode.Value;
                }
                pointerNode = pointerNode.Next;
            }
            return null;
        }

        private static void CreatePointer(int pointerId, Vector2 newPosition, GameObject newObjectUnderPointer)
        {
            PointerInputData pointerData = new PointerInputData(pointerId, DateTime.UtcNow, newPosition, newObjectUnderPointer);
            _pointers.AddLast(pointerData);

            //  Pointer started on an object
            if(newObjectUnderPointer != null)
            {
                SendPointerEnter(pointerId, newPosition, newObjectUnderPointer);
            }
            SendPointerDown(pointerId, newPosition, newObjectUnderPointer);
        }

        private static void RemovePointer(PointerInputData pointerData, Vector2 newPosition, GameObject newObjectUnderPointer)
        {
            if (pointerData.ObjectUnder != null)
            {
                SendPointerLeave(pointerData.pointerId, newPosition, pointerData.ObjectUnder);
            }

            if (pointerData.State == PointerInputStates.DragCandidate)
            {
                pointerData.Clicked();
                SendPointerClick(pointerData.pointerId, newPosition, pointerData.ObjectUnder);
            }
            else
            {
                SendPointerUp(pointerData.pointerId, newPosition, pointerData.ObjectUnder);
            }

            _pointers.Remove(pointerData);
        }

        private static void UpdatePointer(PointerInputData pointerData, Vector2 newPosition, GameObject newObjectUnderPointer)
        {
            //  Pointer moves
            if (pointerData.CurrentPosition != newPosition)
            {
                pointerData.UpdatePosition(newPosition);
                SendPointerMove(pointerData.pointerId, pointerData.CurrentPosition, pointerData.ObjectUnder);
            }

            //  Pointer leaves object
            if (pointerData.ObjectUnder != null && pointerData.ObjectUnder != newObjectUnderPointer)
            {
                GameObject oldObject = pointerData.ObjectUnder;
                pointerData.UpdateObjectUnder(null);
                SendPointerLeave(pointerData.pointerId, pointerData.CurrentPosition, oldObject);
            }

            //  Pointer enters a new object
            if (pointerData.ObjectUnder == null && newObjectUnderPointer != null)
            {
                pointerData.UpdateObjectUnder(newObjectUnderPointer);
                SendPointerEnter(pointerData.pointerId, pointerData.CurrentPosition, pointerData.ObjectUnder);
            }

            //  Check click cancel
            if (pointerData.State == PointerInputStates.DragCandidate)
            {
                Vector2  pointerMovement    = newPosition     - pointerData.StartPosition;
                TimeSpan pointerElapsedTime = DateTime.UtcNow - pointerData.startTime;
                
                if (pointerMovement.magnitude       >= _clickCancelMovement ||
                    pointerElapsedTime.TotalSeconds >= _clickCancelTimeout)
                {
                    pointerData.ClickCanceled();
                    SendPointerClickCanceled(pointerData.pointerId, newPosition, newObjectUnderPointer);
                }
            }
        }

#endregion

#region Handlers call helpers

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SendPointerDown(int pointerId, Vector2 position, GameObject objectUnderPointer)
        {
            for (int i = 0; i < _pointerHandlers.Count; i++)
            {
                _pointerHandlers[i].OnPointerDown(pointerId, position, objectUnderPointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SendPointerMove(int pointerId, Vector2 position, GameObject objectUnderPointer)
        {
            for (int i = 0; i < _pointerHandlers.Count; i++)
            {
                _pointerHandlers[i].OnPointerMove(pointerId, position, objectUnderPointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SendPointerUp(int pointerId, Vector2 position, GameObject objectUnderPointer)
        {
            for (int i = 0; i < _pointerHandlers.Count; i++)
            {
                _pointerHandlers[i].OnPointerUp(pointerId, position, objectUnderPointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SendPointerClick(int pointerId, Vector2 position, GameObject objectUnderPointer)
        {
            for (int i = 0; i < _pointerHandlers.Count; i++)
            {
                _pointerHandlers[i].OnPointerClick(pointerId, position, objectUnderPointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SendPointerClickCanceled(int pointerId, Vector2 position, GameObject objectUnderPointer)
        {
            for (int i = 0; i < _pointerHandlers.Count; i++)
            {
                _pointerHandlers[i].OnPointerClickCanceled(pointerId, position, objectUnderPointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SendPointerEnter(int pointerId, Vector2 position, GameObject objectUnderPointer)
        {
            for (int i = 0; i < _pointerHandlers.Count; i++)
            {
                _pointerHandlers[i].OnPointerEnter(pointerId, position, objectUnderPointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SendPointerLeave(int pointerId, Vector2 position, GameObject objectUnderPointer)
        {
            for (int i = 0; i < _pointerHandlers.Count; i++)
            {
                _pointerHandlers[i].OnPointerLeave(pointerId, position, objectUnderPointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SendScroll(Vector2 movement)
        {
            for (int i = 0; i < _gestureHandlers.Count; i++)
            {
                _gestureHandlers[i].OnScroll(movement);
            }
        }

#endregion

#region Keyboard Methods

    public static void UpdateKeyboard()
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
                            CheckKeyboardModifierKeys(keyInput.modifierKeyCodes))
                        {
                            keyInput.action();
                        }
                    }
                    else
                    if (keyInput.keyStateToAction == false)
                    {
                        if (UnityEngine.Input.GetKeyUp((KeyCode)keyInput.keyCode) &&
                            CheckKeyboardModifierKeys(keyInput.modifierKeyCodes))
                        {
                            keyInput.action();
                        }
                    }
                }
            }
        }
    }

    private static bool CheckKeyboardModifierKeys(int[] modifierKeyCodes)
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
     
#endregion
    }
}