namespace Com.Bit34Games.Unity.Input
{
    public static class PointerInputConstants
    {
        //  CONFIG
        public const int   DEFAULT_COLLIDER_MASK = 0xFFFF;
        public const bool  DEFAULT_UI_BLOCKS_POINTERS = true;
        public const float DEFAULT_CLICK_CANCEL_MOVEMENT_PIXELS = 10;
        public const float DEFAULT_CLICK_CANCEL_TIMEOUT_SECONDS = 0.2f;

        public const int MOUSE_LEFT_BUTTON   = 0;
        public const int MOUSE_RIGHT_BUTTON  = 1;
        public const int MOUSE_MIDDLE_BUTTON = 2;

        public const int    MOUSE_POINTER_ID             = -1;
        public const int    MOUSE_LEFT_DRAG_POINTER_ID   = -2;
        public const int    MOUSE_RIGHT_DRAG_POINTER_ID  = -3;
        public const int    MOUSE_MIDDLE_DRAG_POINTER_ID = -4;
    }
}