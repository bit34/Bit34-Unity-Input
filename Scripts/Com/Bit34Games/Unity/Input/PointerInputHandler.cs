using UnityEngine;


namespace Com.Bit34Games.Unity.Input
{
    public class PointerInputHandler : IPointerInputHandler
    {
        //  DELEGATES
        public delegate void PointerDelegate(int id, Vector2 screenPosition, GameObject objectUnderPointer);


        //  MEMBERS
        private PointerDelegate _onPointerDown;
        private PointerDelegate _onPointerMove;
        private PointerDelegate _onPointerUp;
        private PointerDelegate _onPointerClick;
        private PointerDelegate _onPointerClickCanceled;
        private PointerDelegate _onPointerEnter;
        private PointerDelegate _onPointerLeave;
        private PointerDelegate _onPointerCancel;


        //  CONSTRUCTORS
        public PointerInputHandler(
            PointerDelegate onPointerDown,
            PointerDelegate onPointerMove,
            PointerDelegate onPointerUp,
            PointerDelegate onPointerClick,
            PointerDelegate onPointerClickCanceled,
            PointerDelegate onPointerEnter,
            PointerDelegate onPointerLeave,
            PointerDelegate onPointerCancel
        )
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
        public void OnPointerDown         (int id, Vector2 screenPosition, GameObject objectUnderPointer){ _onPointerDown         (id, screenPosition, objectUnderPointer); }
        public void OnPointerMove         (int id, Vector2 screenPosition, GameObject objectUnderPointer){ _onPointerMove         (id, screenPosition, objectUnderPointer); }
        public void OnPointerUp           (int id, Vector2 screenPosition, GameObject objectUnderPointer){ _onPointerUp           (id, screenPosition, objectUnderPointer); }
        public void OnPointerClick        (int id, Vector2 screenPosition, GameObject objectUnderPointer){ _onPointerClick        (id, screenPosition, objectUnderPointer); }
        public void OnPointerClickCanceled(int id, Vector2 screenPosition, GameObject objectUnderPointer){ _onPointerClickCanceled(id, screenPosition, objectUnderPointer); }
        public void OnPointerEnter        (int id, Vector2 screenPosition, GameObject objectUnderPointer){ _onPointerEnter        (id, screenPosition, objectUnderPointer); }
        public void OnPointerLeave        (int id, Vector2 screenPosition, GameObject objectUnderPointer){ _onPointerLeave        (id, screenPosition, objectUnderPointer); }
        public void OnPointerCancel       (int id, Vector2 screenPosition, GameObject objectUnderPointer){ _onPointerCancel       (id, screenPosition, objectUnderPointer); }
    }
}
