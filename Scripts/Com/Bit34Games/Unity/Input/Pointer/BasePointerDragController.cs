using System.Collections.Generic;
using Com.Bit34Games.Unity.Input;
using UnityEngine;

namespace Com.Bit34Games.Unity.Camera
{
    public abstract class BasePointerDragController : IPointerDragController
    {
        //  MEMBERS
        public abstract bool CanDrag                   { get; }
        public bool          IsDragging                { get; private set; }
        public int           DraggingPointerId         { get; private set; }
        public Vector2       DragStartScreenPosition   { get; private set; }
        public Vector2       DragCurrentScreenPosition { get; private set; }
        //      Internal
        private List<IPointerDragHandler> _pointerDragHandlers;
        private PointerInputHandler       _pointerInputHandler;

        //  CONSTRUCTORS
        public BasePointerDragController()
        {
            _pointerDragHandlers = new List<IPointerDragHandler>();
            _pointerInputHandler = new PointerInputHandler(DoNothing, OnPointerMove, OnPointerUp, DoNothing, DoNothing, DoNothing, DoNothing, DoNothing);
            DraggingPointerId    = PointerInputConstants.INVALID_POINTER_ID;
        }

        //  METHODS
        public void AddPointerDragHandler(IPointerDragHandler handler)
        {
            _pointerDragHandlers.Add(handler);
        }

        public void RemovePointerDragHandler(IPointerDragHandler handler)
        {
            _pointerDragHandlers.Remove(handler);
        }
        
        public void DragWithPointer(int pointerId, bool usePointerStartPosition)
        {
            if (CanDrag && !IsDragging)
            {
                InputManager.AddPointerHandler(_pointerInputHandler);
                IsDragging        = true;
                DraggingPointerId = pointerId;

                Vector2 pointerStartPosition;
                Vector2 pointerCurrentPosition;
                InputManager.GetPointerPosition(pointerId, out pointerStartPosition, out pointerCurrentPosition);

                DragStartScreenPosition   = (usePointerStartPosition) ? (pointerStartPosition) : (pointerCurrentPosition);
                DragCurrentScreenPosition = DragStartScreenPosition;

                DragStarted();
                for (int i = 0; i < _pointerDragHandlers.Count; i++) { _pointerDragHandlers[i].OnDragStarted(DragStartScreenPosition); }
            }
        }

        public void CancelDrag()
        {
            if (IsDragging)
            {
                InputManager.RemovePointerHandler(_pointerInputHandler);
                IsDragging        = false;
                DraggingPointerId = PointerInputConstants.INVALID_POINTER_ID;

                DragCancelled();
                for (int i = 0; i < _pointerDragHandlers.Count; i++) { _pointerDragHandlers[i].OnDragCancelled(DragStartScreenPosition, DragCurrentScreenPosition); }

                DragStartScreenPosition   = Vector2.zero;
                DragCurrentScreenPosition = Vector2.zero;
            }
        }

#region Abstract methods to be implemented by sub classes

        protected abstract void DragStarted();
        protected abstract void DragUpdated();
        protected abstract void DragEnded();
        protected abstract void DragCancelled();

#endregion

#region Input callbacks

        private void DoNothing(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer){}
        
        private void OnPointerMove(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer)
        {
            if (pointerId == DraggingPointerId)
            {
                DragCurrentScreenPosition = screenPosition;

                DragUpdated();
                for (int i = 0; i < _pointerDragHandlers.Count; i++) { _pointerDragHandlers[i].OnDragUpdated(DragStartScreenPosition, DragCurrentScreenPosition); }
            }
        }

        private void OnPointerUp(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer, bool willSendClick)
        {
            if (pointerId == DraggingPointerId)
            {
                InputManager.RemovePointerHandler(_pointerInputHandler);
                IsDragging        = false;
                DraggingPointerId = PointerInputConstants.INVALID_POINTER_ID;

                DragCurrentScreenPosition = screenPosition;

                DragEnded();
                for (int i = 0; i < _pointerDragHandlers.Count; i++) { _pointerDragHandlers[i].OnDragEnded(DragStartScreenPosition, DragCurrentScreenPosition); }

                DragStartScreenPosition   = Vector2.zero;
                DragCurrentScreenPosition = Vector2.zero;
            }
        }

#endregion

    }
}