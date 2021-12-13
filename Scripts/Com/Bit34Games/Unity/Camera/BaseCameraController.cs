using UnityEngine;
using Com.Bit34Games.Unity.Update;
using Com.Bit34Games.Unity.Input;

namespace Com.Bit34Games.Unity.Camera
{
    public abstract class BaseCameraController : ICameraController
    {

        //  MEMBERS
        public bool               IsActive        { get; private set; }
        public bool               IsDragging      { get; private set; }
        public bool               UseScrollToZoom { get; protected set; }
        public UnityEngine.Camera ActiveCamera    { get { return (_camera != null) ? _camera : UnityEngine.Camera.main; } }
        //      Shared
        protected Vector3           _cameraPosition;
        protected float             _cameraPositionFollow;
        //      Internal
        private UnityEngine.Camera  _camera;
        private PointerInputHandler _pointerInputHandler;
        private GestureInputHandler _gestureInputHandler;
        private int                 _draggingPointerId;

        //  CONSTRUCTOR
        public BaseCameraController(UnityEngine.Camera camera)
        {
            _camera               = camera;
            _cameraPosition       = ActiveCamera.transform.position;
            _cameraPositionFollow = 0;
            _pointerInputHandler  = new PointerInputHandler(DoNothing,
                                                            OnPointerMove,
                                                            OnPointerUp,
                                                            DoNothing,
                                                            DoNothing,
                                                            DoNothing,
                                                            DoNothing,
                                                            DoNothing);
            _gestureInputHandler  = new GestureInputHandler(OnScroll);
            _draggingPointerId    = PointerInputConstants.INVALID_POINTER_ID;
        }

        //  METHODS
        public void SetActivate(bool state)
        {
            if (IsActive != state)
            {
                IsActive = state;

                if (IsActive)
                {
                    UpdateManager.Add(UpdateCallback, this, null, UpdateTimeTypes.Utc, UpdateCallbackTypes.MonoBehaviourLateUpdate);
                    InputManager.AddPointerHandler(_pointerInputHandler);
                    InputManager.AddGestureHandler(_gestureInputHandler);
                }
                else
                {
                    UpdateManager.RemoveAllFrom(this);
                    InputManager.RemovePointerHandler(_pointerInputHandler);
                    InputManager.RemoveGestureHandler(_gestureInputHandler);

                    IsDragging = false;
                    _draggingPointerId = PointerInputConstants.INVALID_POINTER_ID;
                }
            }
        }

        private void UpdateCallback()
        {
            InterpolateZoom();
            InterpolatePosition();
        }

#region Movement

        public abstract void SetPosition(Vector3 cameraPosition, bool immediately);

        public void DragWithPointer(int pointerId, Vector2 screenPosition)
        {
            if (IsActive)
            {
                IsDragging = true;
                _draggingPointerId = pointerId;
                StartPointerDrag(screenPosition);
            }
        }

        protected abstract void ConstraintPosition();

        protected virtual void  InterpolatePosition()
        {
            ActiveCamera.transform.position = Vector3.Lerp(ActiveCamera.transform.position, _cameraPosition, _cameraPositionFollow);
        }

        protected abstract void StartPointerDrag(Vector2 screenPosition);

        protected abstract void UpdatePointerDrag(Vector2 screenPosition);

#endregion

#region Zoom

        protected abstract void ApplyZoomFromScroll(Vector2 movement);

        protected abstract void InterpolateZoom();

        protected abstract void ConstraintZoom();
     
#endregion

#region Input callbacks

        private void DoNothing(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer){}
        
        private void OnPointerMove(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer)
        {
            if (pointerId == _draggingPointerId)
            {
                UpdatePointerDrag(screenPosition);
            }
        }

        private void OnPointerUp(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer)
        {
            if (pointerId == _draggingPointerId)
            {
                IsDragging = false;
                _draggingPointerId = PointerInputConstants.INVALID_POINTER_ID;
            }
        }

        private void OnScroll(Vector2 movement)
        {
            if (UseScrollToZoom)
            {
                ApplyZoomFromScroll(movement);
            }
        }
     
#endregion


    }
}