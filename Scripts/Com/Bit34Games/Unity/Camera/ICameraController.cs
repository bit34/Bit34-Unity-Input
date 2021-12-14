using UnityEngine;

namespace Com.Bit34Games.Unity.Camera
{
    public interface ICameraController
    {
        //  MEMBERS
        bool    IsActive       { get; }
        bool    IsDragging     { get; }
        Vector3 CameraPosition { get; }

        //  METHODS
        void SetActivate(bool state);
        void SetPosition(Vector3 cameraPosition, bool immediately);
        void DragWithPointer(int pointerId, Vector2 screenPosition);
    }
}