using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dark_Explorer
{
    public class Animation
    {
        private Texture2D _texture;
        private int _frameCount;
        private int _currentFrame;
        private int _frameWidth;
        private int _frameHeight;
        private float _frameTime;
        private float _timer;

        public Vector2 Position { get; set; }

        public Animation(Texture2D texture, int frameCount, int frameWidth, int frameHeight, float frameTime)
        {
            _texture = texture;
            _frameCount = frameCount;
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            _frameTime = frameTime;
            _currentFrame = 0;
            _timer = 0f;
        }

        public void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer > _frameTime)
            {
                _currentFrame++;
                if (_currentFrame >= _frameCount)
                    _currentFrame = 0;

                _timer = 0f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int column = _currentFrame % (_texture.Width / _frameWidth);
            int row = _currentFrame / (_texture.Width / _frameWidth);

            Rectangle sourceRectangle = new Rectangle(_frameWidth * column, _frameHeight * row, _frameWidth, _frameHeight);
            Rectangle destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, _frameWidth, _frameHeight);

            spriteBatch.Draw(_texture, destinationRectangle, sourceRectangle, Color.White);
        }
    }
}
