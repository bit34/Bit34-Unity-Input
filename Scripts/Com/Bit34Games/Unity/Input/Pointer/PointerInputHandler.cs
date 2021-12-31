using UnityEngine;


namespace Com.Bit34Games.Unity.Input
{
    public class PointerInputHandler : IPointerInputHandler
    {
        //  DELEGATES
        public delegate void PointerDelegate(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer);
        public delegate void PointerDelegate2(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer, bool willSendClick);


        //  MEMBERS
        private PointerDelegate _onPointerDown;
        private PointerDelegate _onPointerMove;
        private PointerDelegate2 _onPointerUp;
        private PointerDelegate _onPointerClick;
        private PointerDelegate _onPointerClickCanceled;
        private PointerDelegate _onPointerEnter;
        private PointerDelegate _onPointerLeave;
        private PointerDelegate _onPointerCancel;


        //  CONSTRUCTORS
        public PointerInputHandler(PointerDelegate onPointerDown,
                                   PointerDelegate onPointerMove,
                                   PointerDelegate2 onPointerUp,
                                   PointerDelegate onPointerClick,
                                   PointerDelegate onPointerClickCanceled,
                                   PointerDelegate onPointerEnter,
                                   PointerDelegate onPointerLeave,
                                   PointerDelegate onPointerCancel)
        {
            _onPointerDown          = onPointerDown;
            _onPointerMove          = onPointerMove;
            _onPointerUp            = onPointerUp;
            _onPointerClick         = onPointerClick;
            _onPointerClickCanceled = onPointerClickCanceled;
            _onPointerEnter         = onPointerEnter;
            _onPointerLeave         = onPointerLeave;
            _onPointerCancel        = onPointerCancel;
        }


        //  METHODS
        public void OnPointerDown         (int pointerId, Vector2 screenPosition, GameObject objectUnderPointer)                    { _onPointerDown         (pointerId, screenPosition, objectUnderPointer); }
        public void OnPointerMove         (int pointerId, Vector2 screenPosition, GameObject objectUnderPointer)                    { _onPointerMove         (pointerId, screenPosition, objectUnderPointer); }
        public void OnPointerUp           (int pointerId, Vector2 screenPosition, GameObject objectUnderPointer, bool willSendClick){ _onPointerUp           (pointerId, screenPosition, objectUnderPointer, willSendClick); }
        public void OnPointerClick        (int pointerId, Vector2 screenPosition, GameObject objectUnderPointer)                    { _onPointerClick        (pointerId, screenPosition, objectUnderPointer); }
        public void OnPointerClickCanceled(int pointerId, Vector2 screenPosition, GameObject objectUnderPointer)                    { _onPointerClickCanceled(pointerId, screenPosition, objectUnderPointer); }
        public void OnPointerEnter        (int pointerId, Vector2 screenPosition, GameObject objectUnderPointer)                    { _onPointerEnter        (pointerId, screenPosition, objectUnderPointer); }
        public void OnPointerLeave        (int pointerId, Vector2 screenPosition, GameObject objectUnderPointer)                    { _onPointerLeave        (pointerId, screenPosition, objectUnderPointer); }
        public void OnPointerCancel       (int pointerId, Vector2 screenPosition, GameObject objectUnderPointer)                    { _onPointerCancel       (pointerId, screenPosition, objectUnderPointer); }
    }
}
