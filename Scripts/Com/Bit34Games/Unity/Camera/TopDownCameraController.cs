using UnityEngine;

namespace Com.Bit34Games.Unity.Camera
{
    public class TopDownCameraController : BaseCameraController
    {
        //  MEMBERS
        public float    CameraMaxSize { get; private set; }
        public float    CameraMinSize { get; private set; }
        //      Internal
        private float   _worldMinX;
        private float   _worldMaxX;
        private float   _worldMinZ;
        private float   _worldMaxZ;
        private Vector3 _dragStartWorldPosition;
        private float   _cameraSize;
        private float   _cameraZoomFollow;
        private float   _scrollZoomMultiplier;

        //  CONSTRUCTOR
        public TopDownCameraController(UnityEngine.Camera camera = null) : base(camera)
        {
            _cameraSize = ActiveCamera.orthographicSize;

            SetupMovement(1);
            SetMovementLimits();
            SetupZoom(1, _cameraSize, _cameraSize, false, 1);
        }

        //  METHODS
        public void SetupMovement(float cameraPositionFollow)
        {
            _cameraPositionFollow = cameraPositionFollow;
        }

        public void SetMovementLimits(float worldMinX=float.MinValue, float worldMaxX=float.MaxValue,
                                      float worldMinZ=float.MinValue, float worldMaxZ=float.MaxValue)
        {
            _worldMinX = worldMinX;
            _worldMaxX = worldMaxX;
            _worldMinZ = worldMinZ;
            _worldMaxZ = worldMaxZ;
        }

        public void SetupZoom(float cameraZoomFollow,
                              float cameraMinimumSize,
                              float cameraMaximumSize,
                              bool  useScrollToZoom,
                              float scrollZoomMultiplier)
        {
            _cameraZoomFollow     = cameraZoomFollow;
            CameraMinSize         = cameraMinimumSize;
            CameraMaxSize         = cameraMaximumSize;
            UseScrollToZoom       = useScrollToZoom;
            _scrollZoomMultiplier = scrollZoomMultiplier;
        }

        public void SetSize(float cameraSize)
        {
            _cameraSize = cameraSize;

            if (IsActive)
            {
                ConstraintZoom();
                ConstraintPosition();
            }
        }

#region BasePointerDragController implementations
    
        protected override void DragStarted()
        {
            _dragStartWorldPosition = ActiveCamera.ScreenToWorldPoint(DragStartScreenPosition);
        }

        protected override void DragUpdated()
        {
            Vector3 currentWorldPosition = ActiveCamera.ScreenToWorldPoint(DragCurrentScreenPosition);
            Vector3 worldMovement        = currentWorldPosition - _dragStartWorldPosition;

            _cameraPosition -= worldMovement;
            ConstraintPosition();
            ActiveCamera.transform.position = _cameraPosition;
        }

        protected override void DragEnded() { }
        protected override void DragCancelled() { }

#endregion

#region BaseCameraController implementations
     
        public override void SetPosition(Vector3 cameraPosition, bool immediately)
        {
            Plane cameraPlane = new Plane(ActiveCamera.transform.forward, ActiveCamera.transform.position);
            _cameraPosition = cameraPlane.ClosestPointOnPlane(cameraPosition);
            
            if(IsActive)
            {
                ConstraintPosition();
                if (immediately)
                {
                    ActiveCamera.transform.position = _cameraPosition;
                }
            }
        }

        protected override void ConstraintPosition()
        {
            float cameraHalfHeight = _cameraSize;
            float cameraHeight     = cameraHalfHeight * 0.5f;

            float cameraHalfWidth = cameraHalfHeight * ActiveCamera.aspect;
            float cameraWidth     = cameraHalfWidth * 0.5f;

            float worldWidth  = _worldMaxX - _worldMinX;
            float worldHeight = _worldMaxZ - _worldMinZ;

            if (worldWidth<=cameraWidth)
            {
                _cameraPosition.x = _worldMinX + worldWidth * 0.5f;
            }
            else
            {
                if ((_cameraPosition.x - cameraHalfWidth ) < _worldMinX) {   _cameraPosition.x = _worldMinX + cameraHalfWidth;  }
                else
                if ((_cameraPosition.x + cameraHalfWidth ) > _worldMaxX) {   _cameraPosition.x = _worldMaxX - cameraHalfWidth;  }
            }

            if (worldHeight <= cameraHeight)
            {
                _cameraPosition.z = _worldMinZ + worldHeight * 0.5f;
            }
            else
            {
                if ((_cameraPosition.z - cameraHalfHeight) < _worldMinZ) {   _cameraPosition.z = _worldMinZ + cameraHalfHeight; }
                else
                if ((_cameraPosition.z + cameraHalfHeight) > _worldMaxZ) {   _cameraPosition.z = _worldMaxZ - cameraHalfHeight; }
            }
        }

        protected override void ApplyZoomFromScroll(Vector2 movement)
        {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            _cameraSize += movement.y * _scrollZoomMultiplier;
#endif

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            _cameraSize += movement.y * -_scrollZoomMultiplier * (_cameraSize / 4);
#endif
            ConstraintZoom();
            ConstraintPosition();
        }

        protected override void ConstraintZoom()
        {
            _cameraSize = Mathf.Min(_cameraSize, CameraMaxSize);
            _cameraSize = Mathf.Max(_cameraSize, CameraMinSize);

            float worldHalfWidth  = (_worldMaxX - _worldMinX) * 0.5f;
            float worldHalfHeight = (_worldMaxZ - _worldMinZ) * 0.5f;

            _cameraSize = Mathf.Min(_cameraSize, worldHalfHeight);
            _cameraSize = Mathf.Min(_cameraSize, worldHalfWidth / ActiveCamera.aspect);
        }

        protected override void InterpolateZoom()
        {
            ActiveCamera.orthographicSize = Mathf.Lerp(ActiveCamera.orthographicSize, _cameraSize, _cameraZoomFollow);
        }

#endregion

    }
}