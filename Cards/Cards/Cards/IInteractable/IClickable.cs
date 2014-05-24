using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;

namespace Cards.IInteractable
{
    public interface IClickable : IComparable<IClickable>
    {
        Vector2 Position { get; set; }
        Rectangle HitBox { get; }
        float ZIndex { get; set; }
        bool Hovering { get; }

        void LoadContent(ContentManager content);
        void Draw(SpriteBatch spriteBatch);

        bool IsMouseOver(MouseState mouse);
        void Hover();
        int CompareTo(IClickable other);
    }
}
