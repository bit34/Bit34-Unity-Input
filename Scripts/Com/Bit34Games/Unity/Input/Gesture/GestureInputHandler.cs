using UnityEngine;


namespace Com.Bit34Games.Unity.Input
{
    public class GestureInputHandler : IGestureInputHandler
    {
        //  DELEGATES
        public delegate void ScrollDelegate(Vector2 movement);


        //  MEMBERS
        private ScrollDelegate _onScroll;


        //  CONSTRUCTORS
        public GestureInputHandler(ScrollDelegate onScroll)
        {
            _onScroll = onScroll;
        }


        //  METHODS
        public void OnScroll(Vector2 movement){ _onScroll(movement); }
    }
}
