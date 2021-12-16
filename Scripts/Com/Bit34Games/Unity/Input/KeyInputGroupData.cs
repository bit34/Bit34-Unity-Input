using System.Collections.Generic;

namespace Com.Bit34Games.Unity.Input
{

    public class KeyInputGroupData
    {
        //  MEMBERS
        public readonly KeyInputSourceTypes source;
        public readonly int                 groupId;
        public bool                         state;
        public readonly List<KeyInputData>  keyInputs;

        //  CONSTRUCTIONS
        public KeyInputGroupData(KeyInputSourceTypes source, int groupId)
        {
            this.source  = source;
            this.groupId = groupId;
            keyInputs    = new List<KeyInputData>();
        }
    }
}