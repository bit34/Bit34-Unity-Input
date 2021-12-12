using UnityEngine;

namespace Com.Bit34Games.Unity.Input
{
    public interface IPointerInputHandler
    {
        //  METHODS
        void OnPointerDown(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer);
        void OnPointerMove(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer);
        void OnPointerUp(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer);
        void OnPointerClick(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer);
        void OnPointerClickCanceled(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer);
        void OnPointerEnter(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer);
        void OnPointerLeave(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer);
        void OnPointerCancel(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer);
    }
}
