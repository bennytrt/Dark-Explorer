using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dark_Explorer
{
    public class AnimationPlayer
    {
        private Animation _walkingAnimation;
        private Vector2 _position;

        public AnimationPlayer(Texture2D walkingTexture)
        {
            _position = Vector2.Zero;

            _walkingAnimation = new Animation(walkingTexture, 8, 64, 64, 0.1f); 
            _walkingAnimation.Position = _position;
        }

        public void Update(GameTime gameTime)
        {
            _walkingAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _walkingAnimation.Draw(spriteBatch);
        }
    }
}
