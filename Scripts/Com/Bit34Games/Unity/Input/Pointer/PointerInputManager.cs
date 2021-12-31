using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.Bit34Games.Unity.Input
{
    public class PointerInputManager
    {
        //  CONSTANTS
        private const int UNITY_MOUSE_LEFT_BUTTON   = 0;
        private const int UNITY_MOUSE_RIGHT_BUTTON  = 1;
        private const int UNITY_MOUSE_MIDDLE_BUTTON = 2;

        //  MEMBERS
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
        public int PointerCount { get { return _pointers.Count; } }
        //      Internal
        private int                          _colliderMask;
        private bool                         _uiBlocksPointers;
        private float                        _clickCancelMovement;
        private float                        _clickCancelTimeout;
        private InputHandlers                _handlers;
        private LinkedList<PointerInputData> _pointers;
        private PointerInputData             _mousePointerData;

        //  CONSTRUCTORS
        public PointerInputManager(InputHandlers handlers)
        {
            _handlers = handlers;
            _pointers = new LinkedList<PointerInputData>();

            if (UsingMouse)
            {
                InitMouse();
            }
        }

        //  METHODS
        public void SetOptions(int   colliderMask,
                               bool  uiBlocksPointers,
                               float clickCancelMovement,
                               float clickCancelTimeout)
        {
            _colliderMask        = colliderMask;
            _uiBlocksPointers    = uiBlocksPointers;
            _clickCancelMovement = clickCancelMovement;
            _clickCancelTimeout  = clickCancelTimeout;
        }
        public void SetColliderMask(int colliderMask)
        {
            _colliderMask = colliderMask;
        }

        public void UpdatePointers()
        {
            if (UsingMouse)
            {
                UpdateMouse();
            }
        }

        public void ObjectDestroyed(InputObjectComponent inputObject)
        {
            LinkedListNode<PointerInputData> pointerNode = _pointers.First;
            while(pointerNode != null)
            {
                if (pointerNode.Value.ObjectUnder == inputObject.gameObject)
                {
                    pointerNode.Value.UpdateObjectUnder(null);

                    //  Pointer leaves destroyed object
                    _handlers.SendPointerLeave(pointerNode.Value.pointerId, pointerNode.Value.CurrentPosition, null);
                    
                    //  Cancel click when object destroyed??
                }
                pointerNode = pointerNode.Next;
            }
        }

#region Helpers

        private GameObject GetObjectUnderPointer(Vector2 pointerScreenPosition)
        {
            if (_uiBlocksPointers && (EventSystem.current!=null && EventSystem.current.IsPointerOverGameObject()))
            {
                return null;
            }

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


#region Internal Mouse Methods

        private void InitMouse()
        {
            _mousePointerData = new PointerInputData(PointerInputConstants.MOUSE_POINTER_ID, DateTime.UtcNow, Vector2.zero, null);
            _mousePointerData.ClickCanceled();
            _pointers.AddLast(_mousePointerData);
        }

        private void UpdateMouse()
        {
            Vector2    newMousePosition      = UnityEngine.Input.mousePosition;
            GameObject newObjectUnderPointer = GetObjectUnderPointer(UnityEngine.Input.mousePosition);

            UpdatePointer(_mousePointerData, newMousePosition, newObjectUnderPointer);

            CheckMouseButton(UNITY_MOUSE_LEFT_BUTTON,   PointerInputConstants.MOUSE_LEFT_DRAG_POINTER_ID,   newMousePosition, newObjectUnderPointer);
            CheckMouseButton(UNITY_MOUSE_RIGHT_BUTTON,  PointerInputConstants.MOUSE_RIGHT_DRAG_POINTER_ID,  newMousePosition, newObjectUnderPointer);
            CheckMouseButton(UNITY_MOUSE_MIDDLE_BUTTON, PointerInputConstants.MOUSE_MIDDLE_DRAG_POINTER_ID, newMousePosition, newObjectUnderPointer);

            CheckMouseWheel();
        }

        private void CheckMouseButton(int buttonId, int pointerId, Vector2 newPosition, GameObject newObjectUnderPointer)
        {
            //  Mouse button just pressed
            if (UnityEngine.Input.GetMouseButtonDown(buttonId))
            {
                //  Handle mouse presses in consequtive frames
                PointerInputData pointerData = GetPointerData(pointerId);
                if (pointerData != null)
                {
                    RemovePointer(pointerData, newPosition, newObjectUnderPointer);
                }
                
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

        private void CheckMouseWheel()
        {
            //  Should not be tracked?
            if (_uiBlocksPointers && 
                (EventSystem.current!=null && EventSystem.current.IsPointerOverGameObject()))
            {
                return;
            }

            if (UnityEngine.Input.mouseScrollDelta.magnitude != 0)
            {
                _handlers.SendScroll(UnityEngine.Input.mouseScrollDelta);
            }
            
        }

#endregion


#region Internal Pointer Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PointerInputData GetPointerData(int pointerId)
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

        private void CreatePointer(int pointerId, Vector2 newPosition, GameObject newObjectUnderPointer)
        {
            PointerInputData pointerData = new PointerInputData(pointerId, DateTime.UtcNow, newPosition, newObjectUnderPointer);
            _pointers.AddLast(pointerData);

            _handlers.SendPointerDown(pointerId, newPosition, newObjectUnderPointer);

            //  Pointer started on an object
            if(newObjectUnderPointer != null)
            {
                _handlers.SendPointerEnter(pointerId, newPosition, newObjectUnderPointer);
            }
        }

        private void RemovePointer(PointerInputData pointerData, Vector2 newPosition, GameObject newObjectUnderPointer)
        {
            _pointers.Remove(pointerData);
            if (pointerData.ObjectUnder != null)
            {
                _handlers.SendPointerLeave(pointerData.pointerId, newPosition, pointerData.ObjectUnder);
            }

            bool willSendClick = pointerData.State == PointerInputStates.ClickCandidate;

            _handlers.SendPointerUp(pointerData.pointerId, newPosition, pointerData.ObjectUnder, willSendClick);

            if (willSendClick)
            {
                pointerData.Clicked();
                _handlers.SendPointerClick(pointerData.pointerId, newPosition, pointerData.ObjectUnder);
            }
        }

        private void UpdatePointer(PointerInputData pointerData, Vector2 newPosition, GameObject newObjectUnderPointer)
        {
            //  Pointer moves
            if (pointerData.CurrentPosition != newPosition)
            {
                pointerData.UpdatePosition(newPosition);
                _handlers.SendPointerMove(pointerData.pointerId, pointerData.CurrentPosition, pointerData.ObjectUnder);
            }

            //  Pointer leaves object
            if (pointerData.ObjectUnder != null && pointerData.ObjectUnder != newObjectUnderPointer)
            {
                GameObject oldObject = pointerData.ObjectUnder;
                pointerData.UpdateObjectUnder(null);
                _handlers.SendPointerLeave(pointerData.pointerId, pointerData.CurrentPosition, oldObject);
            }

            //  Pointer enters a new object
            if (pointerData.ObjectUnder == null && newObjectUnderPointer != null)
            {
                pointerData.UpdateObjectUnder(newObjectUnderPointer);
                _handlers.SendPointerEnter(pointerData.pointerId, pointerData.CurrentPosition, pointerData.ObjectUnder);
            }

            //  Check click cancel
            if (pointerData.State == PointerInputStates.ClickCandidate)
            {
                Vector2  pointerMovement    = newPosition     - pointerData.StartPosition;
                TimeSpan pointerElapsedTime = DateTime.UtcNow - pointerData.startTime;
                
                if (pointerMovement.magnitude       >= _clickCancelMovement ||
                    pointerElapsedTime.TotalSeconds >= _clickCancelTimeout)
                {
                    pointerData.ClickCanceled();
                    _handlers.SendPointerClickCanceled(pointerData.pointerId, newPosition, newObjectUnderPointer);
                }
            }
        }

#endregion
    }
}