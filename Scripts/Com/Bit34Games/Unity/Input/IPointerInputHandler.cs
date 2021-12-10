using UnityEngine;

namespace Com.Bit34Games.Unity.Input
{
    public interface IPointerInputHandler
    {
        //  METHODS
        void OnPointerDown(int id, Vector2 screenPosition, GameObject objectUnderPointer);
        void OnPointerMove(int id, Vector2 screenPosition, GameObject objectUnderPointer);
        void OnPointerUp(int id, Vector2 screenPosition, GameObject objectUnderPointer);
        void OnPointerClick(int id, Vector2 screenPosition, GameObject objectUnderPointer);
        void OnPointerClickCanceled(int id, Vector2 screenPosition, GameObject objectUnderPointer);
        void OnPointerEnter(int id, Vector2 screenPosition, GameObject objectUnderPointer);
        void OnPointerLeave(int id, Vector2 screenPosition, GameObject objectUnderPointer);
        void OnPointerCancel(int id, Vector2 screenPosition, GameObject objectUnderPointer);
    }
}
