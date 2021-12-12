using System;
using UnityEngine;

namespace Com.Bit34Games.Unity.Input
{

    public class PointerInputData
    {
        //  MEMBERS
        public readonly int      pointerId;
        public readonly DateTime startTime;
        public PointerInputState State           { get; private set; }
        public Vector2           StartPosition   { get; private set; }
        public Vector2           CurrentPosition { get; private set; }
        public GameObject        ObjectUnder     { get; private set; }

        //  CONSTRUCTORS
        public PointerInputData(int pointerId, DateTime startTime, Vector2 startPosition, GameObject objectUnder)
        {
            this.pointerId  = pointerId;
            this.startTime  = startTime;
            State           = PointerInputState.DragCandidate;
            StartPosition   = startPosition;
            CurrentPosition = startPosition;
            ObjectUnder     = objectUnder;
        }

        //  METHODS
        public void Clicked()
        {
            State = PointerInputState.Click;
        }

        public void ClickCanceled()
        {
            State = PointerInputState.Drag;
        }

        public void UpdatePosition(Vector2 position)
        {
            CurrentPosition = position;
        }

        public void UpdateObjectUnder(GameObject objectUnder)
        {
            ObjectUnder = objectUnder;
        }

    }
}
