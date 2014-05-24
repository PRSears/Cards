using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Cards.IInteractable;
using Cards.Structs;

namespace Cards.Screens.Layouts
{
    public class FluidLayoutContainer : LayoutContainer
    {
        /// <summary>
        /// Construct a new FluidLayoutContainer at the Rectangle's position.
        /// </summary>
        /// <param name="position">Position to place the rectangle at, with given dimensions.</param>
        public FluidLayoutContainer(Rectangle position)
        {
            this.Position = position;
            this.Elements = new Queue<IClickable>();
            this.DockToEdge = Edge.None;
            this.Margin = 25;
        }

        /// <summary>
        /// Construct a new FluidLayoutContainer at the Rectangle's position, with contained elements 
        /// floating to a specific edge of the container.
        /// </summary>
        /// <param name="position">Position to place the rectangle at, with given dimensions.</param>
        /// <param name="floatEdge">Which edge contained elements should float toward.</param>
        public FluidLayoutContainer(Rectangle position, Edge floatEdge) 
            : this(position)
        {
            this.DockToEdge = floatEdge;
        }

        /// <summary>
        /// Construct a new FluidLayoutContainer at the Rectangle's position, with contained elements 
        /// floating to a specific edge of the container. 
        /// </summary>
        /// <param name="position">Position to place the rectangle at, with given dimensions.</param>
        /// <param name="floatEdge">Which edge contained elements should float toward.</param>
        /// <param name="margin">Space (in pixels) to keep clear around elements. Default is 25px.</param>
        public FluidLayoutContainer(Rectangle position, Edge floatEdge, int margin)
            : this(position, floatEdge)
        {
            this.Margin = margin;
        }

        /// <summary>
        /// Movse and resizes the layout container and re-positions contained elements based on the changes.
        /// </summary>
        /// <param name="newPosition">Rectangle object representing new container position.</param>
        public override void Resize(Rectangle newPosition)
        {
            this.Position = newPosition;

            if (DockToEdge.Equals(Edge.Top))
                PositionTop(); 
            if (DockToEdge.Equals(Edge.Left))
                PositionLeft();
            if (DockToEdge.Equals(Edge.Bottom))
                PositionBottom();
            if (DockToEdge.Equals(Edge.Right))
                PositionRight();
            if (DockToEdge.Equals(Edge.None))
                PositionCenter();

        }

        private void PositionTop()
        {
            Rectangle prevElementPos = new Rectangle(this.Position.X, this.Position.Y + Margin, 0, 0);
            
            // The furthest down (in pixels) any object is from the origin
            int depthPoint = 0;

            foreach (IClickable element in Elements)
            {
                // Check if we're about to overflow the width of the container
                if ((prevElementPos.Right + element.HitBox.Width + (Margin*2)) > this.Position.Right)
                {
                    // Check if we're going to overflow the height of the container
                    if ((prevElementPos.Bottom + element.HitBox.Height) > this.Position.Bottom)
                        break; // stop trying to place elements

                    // reset into the next row
                    prevElementPos = new Rectangle(this.Position.X, depthPoint + Margin, 0, 0);
                }

                // Add the next element in the same row, to the right of the last element
                element.Position = new Vector2(
                    prevElementPos.Right + Margin,
                    prevElementPos.Y);

                if (element.Position.Y > depthPoint) depthPoint = element.HitBox.Bottom;

                prevElementPos = element.HitBox;
            }
        }

        private void PositionLeft()
        {
            Rectangle prevElementPos = this.Position;
            prevElementPos.X += Margin;
            prevElementPos.Width -= Margin;
            prevElementPos.Height = 0;

            // The right-most (x) coordinate filled by a positioned element.
            // Used to track where the next column should start.
            int rightPoint = 0; 

            foreach (IClickable element in Elements)
            {
                // Check if we've reached the bottom of the container
                if ((element.HitBox.Height + prevElementPos.Bottom + (Margin*2)) > this.Position.Bottom)
                {
                    // Make sure there's enough horizontal space left
                    if ((element.HitBox.Width + rightPoint + Margin) > this.Position.Right)
                        break;
                    
                    prevElementPos = this.Position; // reset position
                    prevElementPos.X += rightPoint + Margin; // move into next column
                    prevElementPos.Height = 0;
                }

                // Add the next element in-line below the last element
                element.Position = new Vector2(
                    prevElementPos.X,
                    prevElementPos.Y + prevElementPos.Height + Margin);
                // Track the right-most point a positioned element occupies
                if (element.HitBox.Right > rightPoint) rightPoint = element.HitBox.Right;    

                prevElementPos = element.HitBox;
            }
        }

        private void PositionBottom()
        {
            Rectangle prevElementPos = new Rectangle(this.Position.X, this.Position.Bottom - Margin, 0, 0);

            // The highest (y) coordinate filled by a positioned element
            // Used to track where the next row should start
            int heightPoint = this.Position.Bottom;

            foreach (IClickable element in Elements)
            {
                // Check if we've reached the right side of the container
                if ((prevElementPos.Right + element.HitBox.Width + (Margin*2)) > this.Position.Right)
                {
                    // Check if we've reached the top of the container
                    if ((prevElementPos.Top - element.HitBox.Height - (Margin*2)) < this.Position.Top)
                        break;

                    // Reset position into the next row
                    prevElementPos = new Rectangle(this.Position.X, heightPoint - Margin, 0, 0);
                }

                // Add the next element to the same row, to the right of the last element
                element.Position = new Vector2(
                    prevElementPos.Right + Margin, 
                    prevElementPos.Bottom - element.HitBox.Height);
                // Track the top-most point a positioned element occupies
                if (element.HitBox.Top < heightPoint) heightPoint = element.HitBox.Top;

                prevElementPos = element.HitBox;
            }
        }

        private void PositionRight()
        {
            Rectangle prevElementPos = this.Position;
            prevElementPos.Width -= Margin;
            prevElementPos.Height = 0;

            // The left-most (x) coordinate filled by a positioned element.
            // Used to track where the next column should start.
            int leftPoint = this.Position.Right;

            foreach (IClickable element in Elements)
            {
                // Check if we've reached the bottom of the container
                if ((element.HitBox.Height + prevElementPos.Bottom + (Margin*2)) > this.Position.Bottom)
                {
                    // Make sure there's enough horizontal space left
                    if ((leftPoint - (Margin + element.HitBox.Width)) < this.Position.Left)
                        break;

                    prevElementPos = this.Position; // reset position
                    prevElementPos.Width -= (prevElementPos.Right - leftPoint) + Margin; // move into next column
                    prevElementPos.Height = 0;
                }

                // Add the next element in-line below the last element
                element.Position = new Vector2(
                    prevElementPos.Right - element.HitBox.Width,
                    prevElementPos.Y + prevElementPos.Height + Margin);

                // Track the right-most point a positioned element occupies
                if (element.Position.X < leftPoint) leftPoint = (int)element.Position.X;

                prevElementPos = element.HitBox;
            }
        }

        private void PositionCenter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the IClickable object to the container and calculates 
        /// </summary>
        /// <param name="element"></param>
        public override void AddElement(IClickable element)
        {
            this.Elements.Enqueue(element);
            ReCalculate(); // recalculate element positions
        }

        /// <summary>
        /// Removes all objects in Elements with a position matching elementToRemove. Preserves Queue order.
        /// </summary>
        /// <param name="elementToRemove">The IClickable element to try to remove.</param>
        public override void RemoveElement(IClickable elementToRemove)
        {
            foreach (IClickable e in Elements)
            {
                Elements.Dequeue();
                if (e.Position.Equals(elementToRemove.Position))
                    continue; // If it's a match we don't want to add it back to the queue. skip forward
                else
                    Elements.Enqueue(e); // Not the element we're looking for, add it back to the queue
            }
            Elements.Reverse(); // By dequeing and enqueing the order gets reversed. We need to fix that.

            ReCalculate(); // recalculate element positions
        }

        //TODO consider adding support for Edge + Edge.
        //     ie: Top + Left. Float toward the top left corner, Left + center centers in a left justified column, etc.
        // This might not belong in this type of layout though.
        // Would require adding operators to Edge struct. Not too difficult. (Famous last words).
    }
}
