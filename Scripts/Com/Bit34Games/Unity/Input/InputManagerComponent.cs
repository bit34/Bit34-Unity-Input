using System;
using UnityEngine;


namespace Com.Bit34Games.Unity.Input
{
    public class InputManagerComponent : MonoBehaviour
    {
        //	MEMBERS
#pragma warning disable 0649
        [SerializeField] private string _info;
#pragma warning restore 0649
        //      Internal
        private Action _updateCallback;


        //	METHODS
        public void Init(Action updateCallback)
        {
            if (_updateCallback != null)
            {
                throw new Exception("Input Manager already initialized");
            }

            _updateCallback = updateCallback;
        }

        void Update()
        {
            _updateCallback();
//            _info = "PointerCount:"+InputManager.ActivePointerCount;
        }
    }
}