using UnityEngine;


namespace Com.Bit34Games.Unity.Input
{
    public class InputObjectComponent : MonoBehaviour
    {
        //  MEMBERS
        public int  Category       { get; private set; }
        public int  Id             { get; private set; }
        public bool IsInputEnabled { get; private set; }
        //      Internal
        private bool _isQuiting;

        //  METHODS
        protected void Initialize(int category, int id)
        {
            Category = category;
            Id       = id;
        }

        public void SetState(bool state)
        {
            IsInputEnabled = state;
            InputStateChanged();
        }

        private void OnDestroy()
        {
            PreDestroy();
            if (!_isQuiting)
            {
                InputManager2.Internal_ObjectDestroyed(this);
            }
        }

        private void OnApplicationQuit()
        {
            _isQuiting = true;
        }

        virtual protected void InputStateChanged(){}
        virtual protected void PreDestroy(){}
        
    }
}
