using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.Bit34Games.Unity.Input
{
    public class InputManager2
    {
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
        //      Internal
        private static int                           _colliderMask;
        private static bool                          _uiBlocksPointers;
        private static float                         _clickCancelMovement;
        private static float                         _clickCancelTimeout;
        private static InputManagerComponent         _component;
        private static LinkedList<PointerInputData2> _pointers;
        private static List<IPointerInputHandler>    _pointerHandlers;
        private static PointerInputData2             _mousePointerData;

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

            GameObject updaterObject = new GameObject("[InputManager2]");
            GameObject.DontDestroyOnLoad(updaterObject);
            _component = updaterObject.AddComponent<InputManagerComponent>();
            _component.Init(UpdateMethod);

            _pointers        = new LinkedList<PointerInputData2>();
            _pointerHandlers = new List<IPointerInputHandler>();

            if (UsingMouse)
            {
                InitMouse();
            }
        }

        public static void AddHandler(IPointerInputHandler pointerHandler)
        {
            _pointerHandlers.Add(pointerHandler);
        }

        public static void RemoveHandler(IPointerInputHandler inputHandler)
        {
            _pointerHandlers.Remove(inputHandler);
        }

        public static void Internal_ObjectDestroyed(InputObjectComponent inputObject)
        {
            LinkedListNode<PointerInputData2> pointerNode = _pointers.First;
            while(pointerNode != null)
            {
                if (pointerNode.Value.ObjectUnder == inputObject.gameObject)
                {
                    pointerNode.Value.UpdateObjectUnder(null);
                    SendPointerLeave(pointerNode.Value.pointerId, pointerNode.Value.CurrentPosition, null);
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
        }

#region Helpers

        private static GameObject GetObjectUnderPointer(Vector2 pointerScreenPosition)
        {
            GameObject hitObject = null;

            Camera camera      = Camera.main;
            Ray    ray         = camera.ScreenPointToRay(pointerScreenPosition);
            float  maxDistance = float.MaxValue;
//            Debug.DrawRay(ray.origin, ray.direction, Color.magenta, 1);

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
            _mousePointerData = new PointerInputData2(PointerInputConstants.MOUSE_POINTER_ID, DateTime.UtcNow, Vector2.zero, null);
            _mousePointerData.ClickCanceled();
            _pointers.AddLast(_mousePointerData);
        }

        private static void UpdateMouse()
        {
            Vector2    newMousePosition      = UnityEngine.Input.mousePosition;
            GameObject newObjectUnderPointer = GetObjectUnderPointer(UnityEngine.Input.mousePosition);

            UpdatePointer(_mousePointerData, newMousePosition, newObjectUnderPointer);

            CheckMouseButton(PointerInputConstants.MOUSE_LEFT_BUTTON,   PointerInputConstants.MOUSE_LEFT_DRAG_POINTER_ID,   newMousePosition, newObjectUnderPointer);
            CheckMouseButton(PointerInputConstants.MOUSE_RIGHT_BUTTON,  PointerInputConstants.MOUSE_RIGHT_DRAG_POINTER_ID,  newMousePosition, newObjectUnderPointer);
            CheckMouseButton(PointerInputConstants.MOUSE_MIDDLE_BUTTON, PointerInputConstants.MOUSE_MIDDLE_DRAG_POINTER_ID, newMousePosition, newObjectUnderPointer);
        }

        private static void CheckMouseButton(int buttonId, int pointerId, Vector2 newPosition, GameObject newObjectUnderPointer)
        {
            //  Mouse button just pressed
            if (UnityEngine.Input.GetMouseButtonDown(buttonId))
            {
                //  Should not be tracked?
                if (_uiBlocksPointers && 
                    (EventSystem.current!=null && EventSystem.current.IsPointerOverGameObject(buttonId)))
                {
                    return;
                }

                CreatePointer(pointerId, newPosition, newObjectUnderPointer);
            }
            else 
            if (UnityEngine.Input.GetMouseButtonUp(buttonId))
            {
                PointerInputData2 pointerData = GetPointerData(pointerId);
                if (pointerData != null)
                {
                    RemovePointer(pointerData, newPosition, newObjectUnderPointer);
                }
            }
            else
            {
                PointerInputData2 pointerData = GetPointerData(pointerId);
                if (pointerData != null)
                {
                    UpdatePointer(pointerData, newPosition, newObjectUnderPointer);
                }
            }
        }

#endregion

#region Pointer Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PointerInputData2 GetPointerData(int pointerId)
        {
            LinkedListNode<PointerInputData2> pointerNode = _pointers.First;
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
            PointerInputData2 pointerData = new PointerInputData2(pointerId, DateTime.UtcNow, newPosition, newObjectUnderPointer);
            _pointers.AddLast(pointerData);

            //  Pointer started on an object
            if(newObjectUnderPointer != null)
            {
//XX                SendPointerEnter(pointerId, newPosition, newObjectUnderPointer);
                SendPointerDown(pointerId, newPosition, newObjectUnderPointer);
            }
        }

        private static void RemovePointer(PointerInputData2 pointerData, Vector2 newPosition, GameObject newObjectUnderPointer)
        {
//XX            if (pointerData.ObjectUnder != null)
//XX            {
//XX                SendPointerLeave(pointerData.pointerId, newPosition, pointerData.ObjectUnder);
//XX            }

            if (pointerData.State == PointerInputState.DragCandidate)
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

        private static void UpdatePointer(PointerInputData2 pointerData, Vector2 newPosition, GameObject newObjectUnderPointer)
        {
            //  Pointer leaves object
            if (pointerData.ObjectUnder != null && pointerData.ObjectUnder != newObjectUnderPointer)
            {
                SendPointerLeave(pointerData.pointerId, newPosition, pointerData.ObjectUnder);
                pointerData.UpdateObjectUnder(null);
            }

            //  Pointer enters a new object
            if (pointerData.ObjectUnder == null && newObjectUnderPointer != null)
            {
                pointerData.UpdateObjectUnder(newObjectUnderPointer);
                SendPointerEnter(pointerData.pointerId, newPosition, newObjectUnderPointer);
            }

            if (pointerData.State == PointerInputState.DragCandidate)
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

            if (pointerData.CurrentPosition != newPosition)
            {
                pointerData.UpdatePosition(newPosition);
                SendPointerMove(pointerData.pointerId, newPosition, newObjectUnderPointer);
            }
        }

#endregion

#region Pointer callbacks

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

#endregion

    }
}