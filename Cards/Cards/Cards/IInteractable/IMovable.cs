using System.Collections.Generic;

namespace Cards.IInteractable
{
    public interface IMovable : IClickable
    {
        int Width { get; }
        int Height { get; }
        bool Dragging { get; set; }
        IMovable AttachedNext { get; }
        IMovable AttachedPrev { get; }

        bool IsIntersecting(IMovable other);
        void AttachToFront(IMovable obj);
        void AttachToBack(IMovable obj);
        IMovable RemoveFromFront(IMovable obj);
        IMovable RemoveFromBack(IMovable obj);
    }
}
