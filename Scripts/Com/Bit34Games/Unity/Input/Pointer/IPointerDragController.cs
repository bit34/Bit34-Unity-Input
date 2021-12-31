namespace Com.Bit34Games.Unity.Input
{
    public interface IPointerDragController
    {
        //  MEMBERS
        bool IsDragging        { get; }
        int  DraggingPointerId { get; }

        //  METHODS
        void AddPointerDragHandler(IPointerDragHandler handler);
        void RemovePointerDragHandler(IPointerDragHandler handler);
        void DragWithPointer(int pointerId, bool usePointerStartPosition);
        void CancelDrag();
    }
}
