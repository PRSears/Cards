using System;
using Cards.IInteractable;

namespace Cards.Structs
{
    public struct InputEvent
    {
        private string EventName;
        private IClickable _eventObject;

        public IClickable EventObject { get { return _eventObject; } }
        public static InputEvent Dragging { get { return new InputEvent("dragging", (IMovable)null); } }
        public static InputEvent LeftClicking { get { return new InputEvent("leftclicking", (IClickable)null); } }
        public static InputEvent RightClicking { get { return new InputEvent("rightclicking", (IClickable)null); } }
        public static InputEvent MiddleClicking { get { return new InputEvent("middleclicking", (IClickable)null); } }
        public static InputEvent Hovering { get { return new InputEvent("hovering", (IClickable)null); } }
        public static InputEvent Zooming { get { return new InputEvent("zooming", (IClickable)null); } }
        public static InputEvent ShiftingCard { get { return new InputEvent("shifting", (IMovable)null); } }
        public static InputEvent AttachingCard { get { return new InputEvent("attaching", (IMovable)null); } }
        public static InputEvent None { get { return new InputEvent("none", (IClickable)null); } }

        private InputEvent(string eventName, IClickable movable)
        {
            EventName = eventName;
            _eventObject = movable;
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            InputEvent otherInputEvent = (InputEvent)obj;

            return (this.EventName.Equals(otherInputEvent.EventName));
        }

        public override string ToString()
        {
            return (EventName + " " + EventObject.ToString());
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static Boolean operator ==(InputEvent a, InputEvent b)
        {
            return (a.Equals(b));
        }

        public static Boolean operator !=(InputEvent a, InputEvent b)
        {
            return !(a == b);
        }

        public static InputEvent operator +(InputEvent a, IMovable b)
        {
            a._eventObject = b;
            return a;
        }

        public static InputEvent operator +(IMovable a, InputEvent b)
        {
            b._eventObject = a;
            return b;
        }

        public static InputEvent operator +(InputEvent a, IClickable b)
        {
            a._eventObject = b;
            return a;
        }

        
        public static InputEvent operator +(IClickable a, InputEvent b)
        {
            b._eventObject = a;
            return b;
        }         
    }
}
