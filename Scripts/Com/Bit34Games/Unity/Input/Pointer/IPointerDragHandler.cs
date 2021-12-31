using UnityEngine;

namespace Com.Bit34Games.Unity.Input
{
    public interface IPointerDragHandler
    {
        //  METHODS
        void OnDragStarted(Vector2 startPosition);
        void OnDragUpdated(Vector2 startPosition, Vector2 currentPosition);
        void OnDragEnded(Vector2 startPosition, Vector2 currentPosition);
        void OnDragCancelled(Vector2 startPosition, Vector2 currentPosition);
    }
}
