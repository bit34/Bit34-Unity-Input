using UnityEngine;
using Com.Bit34Games.Unity.Update;

namespace Com.Bit34Games.Unity.Input
{
    public abstract class BaseCameraController : ICameraController
    {

        //  MEMBERS
        public bool   IsActive     { get; private set; }
        public bool   IsDragging   { get; private set; }
        public Camera ActiveCamera { get { return (_camera != null) ? _camera : Camera.main; } }
        //      Shared
        protected Vector3           _cameraPosition;
        //      Internal
        private Camera              _camera;
        private float               _cameraPositionFollow;
        private PointerInputHandler _pointerInputHandler;
        private int                 _draggingPointerId;

        //  CONSTRUCTOR
        public BaseCameraController(Camera camera, float cameraPositionFollow)
        {
            _camera               = camera;
            _cameraPosition       = ActiveCamera.transform.position;
            _cameraPositionFollow = cameraPositionFollow;
            _pointerInputHandler  = new PointerInputHandler(DoNothing,
                                                            OnPointerMove,
                                                            OnPointerUp,
                                                            DoNothing,
                                                            DoNothing,
                                                            DoNothing,
                                                            DoNothing,
                                                            DoNothing);
            _draggingPointerId = PointerInputConstants.INVALID_POINTER_ID;
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
                    InputManager.AddHandler(_pointerInputHandler);
                }
                else
                {
                    UpdateManager.RemoveAllFrom(this);
                    InputManager.RemoveHandler(_pointerInputHandler);

                    IsDragging = false;
                    _draggingPointerId = PointerInputConstants.INVALID_POINTER_ID;
                }
            }
        }

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

        protected abstract void StartPointerDrag(Vector2 screenPosition);
        protected abstract void UpdatePointerDrag(Vector2 screenPosition);

        private void UpdateCallback()
        {
            ActiveCamera.transform.position = Vector3.Lerp(ActiveCamera.transform.position, _cameraPosition, _cameraPositionFollow);
        }

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
    }
}