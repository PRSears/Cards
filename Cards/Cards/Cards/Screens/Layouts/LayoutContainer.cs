using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Cards.IInteractable;
using Cards.Structs;

namespace Cards.Screens.Layouts
{
    public abstract class LayoutContainer
    {
        public Rectangle Position;
        public Queue<IClickable> Elements;
        public Edge DockToEdge;
        public int Margin;

        /// <summary>
        /// Resizes the container and recalculates the position of all Elements contained within.
        /// If the position stays the same the Element positions will simply be recalculated. 
        /// </summary>
        /// <param name="Position">Rectangle object specifying the new coordinates, width or height of the container.</param>
        public abstract void Resize(Rectangle Position);
        /// <summary>
        /// // Recalculates positions of contained elements without changing position. 
        /// </summary>
        public virtual void ReCalculate()
        {
            Resize(this.Position);
        }
        /// <summary>
        /// Call this to preserve layout management when adding an element to the container.
        /// </summary>
        /// <param name="element">The new IClickable element to add to the container.</param>
        public abstract void AddElement(IClickable element);
        /// <summary>
        /// Call this to preserve layout management when removing an element from the container.
        /// </summary>
        /// <param name="element">The existing IClickable element to remove from the container.</param>
        public abstract void RemoveElement(IClickable element);
    }
}
