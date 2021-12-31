using UnityEngine;
using Com.Bit34Games.Unity.Input;

namespace Com.Bit34Games.Unity.Camera
{
    public interface ICameraController : IPointerDragController
    {
        //  MEMBERS
        bool    IsActive       { get; }
        Vector3 CameraPosition { get; }

        //  METHODS
        void SetActivate(bool state);
        void SetPosition(Vector3 cameraPosition, bool immediately);
    }
}