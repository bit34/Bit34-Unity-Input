using UnityEngine;
using Com.Bit34Games.Unity.Update;
using Com.Bit34Games.Unity.Input;

namespace Com.Bit34Games.Unity.Camera
{
    public abstract class BaseCameraController : BasePointerDragController, ICameraController
    {
        //  MEMBERS
        public override bool      CanDrag         { get { return IsActive; } }
        public bool               IsActive        { get; private set; }
        public bool               UseScrollToZoom { get; protected set; }
        public UnityEngine.Camera ActiveCamera    { get { return (_camera != null) ? _camera : UnityEngine.Camera.main; } }
        public Vector3            CameraPosition  { get { return _cameraPosition; } }
        //      Shared
        protected Vector3 _cameraPosition;
        protected float   _cameraPositionFollow;
        //      Internal
        private UnityEngine.Camera  _camera;
        private GestureInputHandler _gestureInputHandler;

        //  CONSTRUCTOR
        public BaseCameraController(UnityEngine.Camera camera)
        {
            _camera               = camera;
            _cameraPosition       = ActiveCamera.transform.position;
            _cameraPositionFollow = 0;
            _gestureInputHandler  = new GestureInputHandler(OnScroll);
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
                    
                    InputManager.AddGestureHandler(_gestureInputHandler);
                }
                else
                {
                    UpdateManager.RemoveAllFrom(this);
                    
                    InputManager.RemoveGestureHandler(_gestureInputHandler);

                    if (IsDragging)
                    {
                        CancelDrag();
                    }
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

        protected abstract void ConstraintPosition();

        protected virtual void  InterpolatePosition()
        {
            ActiveCamera.transform.position = Vector3.Lerp(ActiveCamera.transform.position, _cameraPosition, _cameraPositionFollow);
        }


#endregion

#region Zoom

        protected abstract void ApplyZoomFromScroll(Vector2 movement);

        protected abstract void InterpolateZoom();

        protected abstract void ConstraintZoom();
     
#endregion

#region Input callbacks

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