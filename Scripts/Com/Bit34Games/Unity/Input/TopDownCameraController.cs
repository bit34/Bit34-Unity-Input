using UnityEngine;

namespace Com.Bit34Games.Unity.Input
{
    public class TopDownCameraController : BaseCameraController
    {
        //  MEMBERS
        private Vector2 _dragStartScreenPosition;
        private Vector3 _dragStartCameraPosition;
        private Vector3 _dragStartWorldPosition;
        //  CONSTRUCTOR
        public TopDownCameraController(Camera camera,
                                       float  cameraPositionFollow) : 
            base(camera,
                 cameraPositionFollow)
        {}

        //  METHODS
        public override void SetPosition(Vector3 cameraPosition, bool immediately)
        {
            Plane cameraPlane = new Plane(ActiveCamera.transform.forward, ActiveCamera.transform.position);
            _cameraPosition   = cameraPlane.ClosestPointOnPlane(cameraPosition);
            if(IsActive && immediately)
            {
                ActiveCamera.transform.position = _cameraPosition;
            }
        }

        protected override void StartPointerDrag(Vector2 screenPosition)
        {
            Plane cameraPlane = new Plane(ActiveCamera.transform.forward, ActiveCamera.transform.position);

            _dragStartScreenPosition = screenPosition;
            _dragStartCameraPosition = ActiveCamera.transform.position;
            _dragStartWorldPosition  = ActiveCamera.ScreenToWorldPoint(screenPosition);
        }

        protected override void UpdatePointerDrag(Vector2 screenPosition)
        {
            Plane cameraPlane = new Plane(ActiveCamera.transform.forward, ActiveCamera.transform.position);

            Vector3 currentWorldPosition = ActiveCamera.ScreenToWorldPoint(screenPosition);
            Vector3 worldMovement        = currentWorldPosition - _dragStartWorldPosition;

            _cameraPosition -= worldMovement;
            ActiveCamera.transform.position = _cameraPosition;
        }

    }
}