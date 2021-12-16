using System;

namespace Com.Bit34Games.Unity.Input
{
    public class KeyInputData
    {
        //  MEMBERS
        public readonly int[]  modifierKeyCodes;
        public readonly int    keyCode;
        public readonly bool   keyStateToAction;
        public readonly Action action;

        //  CONSTRUCTIONS
        public KeyInputData(int[] modifierKeyCodes, int keyCode, bool keyStateToAction, Action action)
        {
            this.modifierKeyCodes = modifierKeyCodes;
            this.keyCode          = keyCode;
            this.keyStateToAction = keyStateToAction;
            this.action           = action;
        }
    }
}