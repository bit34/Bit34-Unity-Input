using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Com.Bit34Games.Unity.Input
{
    public class InputHandlers
    {
        //  MEMBERS
        private List<IPointerInputHandler> _pointerHandlers;
        private List<IGestureInputHandler> _gestureHandlers;

        //  CONSTRUCTORS
        public InputHandlers()
        {
            _pointerHandlers = new List<IPointerInputHandler>();
            _gestureHandlers = new List<IGestureInputHandler>();
        }

        //  METHODS
        public void AddPointerHandler(IPointerInputHandler handler)
        {
            _pointerHandlers.Add(handler);
        }

        public void RemovePointerHandler(IPointerInputHandler handler)
        {
            _pointerHandlers.Remove(handler);
        }

        public void AddGestureHandler(IGestureInputHandler handler)
        {
            _gestureHandlers.Add(handler);
        }

        public void RemoveGestureHandler(IGestureInputHandler handler)
        {
            _gestureHandlers.Remove(handler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendPointerDown(int pointerId, Vector2 position, GameObject objectUnderPointer)
        {
            for (int i = 0; i < _pointerHandlers.Count; i++)
            {
                _pointerHandlers[i].OnPointerDown(pointerId, position, objectUnderPointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendPointerMove(int pointerId, Vector2 position, GameObject objectUnderPointer)
        {
            for (int i = 0; i < _pointerHandlers.Count; i++)
            {
                _pointerHandlers[i].OnPointerMove(pointerId, position, objectUnderPointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendPointerUp(int pointerId, Vector2 position, GameObject objectUnderPointer, bool willSendClick)
        {
            for (int i = 0; i < _pointerHandlers.Count; i++)
            {
                _pointerHandlers[i].OnPointerUp(pointerId, position, objectUnderPointer, willSendClick);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendPointerClick(int pointerId, Vector2 position, GameObject objectUnderPointer)
        {
            for (int i = 0; i < _pointerHandlers.Count; i++)
            {
                _pointerHandlers[i].OnPointerClick(pointerId, position, objectUnderPointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendPointerClickCanceled(int pointerId, Vector2 position, GameObject objectUnderPointer)
        {
            for (int i = 0; i < _pointerHandlers.Count; i++)
            {
                _pointerHandlers[i].OnPointerClickCanceled(pointerId, position, objectUnderPointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendPointerEnter(int pointerId, Vector2 position, GameObject objectUnderPointer)
        {
            for (int i = 0; i < _pointerHandlers.Count; i++)
            {
                _pointerHandlers[i].OnPointerEnter(pointerId, position, objectUnderPointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendPointerLeave(int pointerId, Vector2 position, GameObject objectUnderPointer)
        {
            for (int i = 0; i < _pointerHandlers.Count; i++)
            {
                _pointerHandlers[i].OnPointerLeave(pointerId, position, objectUnderPointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendScroll(Vector2 movement)
        {
            for (int i = 0; i < _gestureHandlers.Count; i++)
            {
                _gestureHandlers[i].OnScroll(movement);
            }
        }

    }
}