using System;

namespace Com.Bit34Games.Unity.Input
{
    public class KeyInputData
    {
        //  MEMBERS
        public readonly KeyInputGroupData group;
        public readonly int[]             modifierKeyCodes;
        public int[]                      excludedModifierKeyCodes;
        public readonly int               keyCode;
        public readonly bool              keyStateToAction;
        public readonly Action            action;

        //  CONSTRUCTIONS
        public KeyInputData(KeyInputGroupData group,
                            int[]             modifierKeyCodes,
                            int               keyCode,
                            bool              keyStateToAction,
                            Action            action)
        {
            this.group             = group;
            this.modifierKeyCodes  = modifierKeyCodes;
            this.keyCode           = keyCode;
            this.keyStateToAction  = keyStateToAction;
            this.action            = action;
        }
    }
}
