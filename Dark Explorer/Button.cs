using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dark_Explorer
{
    public class Button
    {
        private readonly Texture2D _texture;
        private readonly SpriteFont _font;
        private readonly string _text;
        private readonly Rectangle _rectangle;
        private readonly Color _color;
        private readonly Color _hoverColor;
        private readonly Color _borderColor;
        private readonly int _borderThickness;
        private readonly Color _shadowColor;
        private readonly int _shadowOffset;
        private readonly Color _textColor; 
        private bool _isHovering;

        public Button(Texture2D texture, SpriteFont font, string text, Rectangle rectangle, Color color, Color hoverColor, Color borderColor, int borderThickness, Color shadowColor, int shadowOffset, Color textColor)
        {
            _texture = texture;
            _font = font;
            _text = text;
            _rectangle = rectangle;
            _color = color;
            _hoverColor = hoverColor;
            _borderColor = borderColor;
            _borderThickness = borderThickness;
            _shadowColor = shadowColor;
            _shadowOffset = shadowOffset;
            _textColor = textColor; 
        }

        public void Update(MouseState mouseState)
        {
            var mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            _isHovering = mouseRectangle.Intersects(_rectangle);
        }

        public bool IsClicked(MouseState mouseState)
        {
            return _isHovering && mouseState.LeftButton == ButtonState.Pressed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var drawColor = _isHovering ? _hoverColor : _color;

            var shadowRectangle = new Rectangle(_rectangle.X + _shadowOffset, _rectangle.Y + _shadowOffset, _rectangle.Width, _rectangle.Height);
            spriteBatch.Draw(_texture, shadowRectangle, _shadowColor);

            DrawBorder(spriteBatch, _rectangle, _borderThickness, _borderColor);

            DrawGradientBackground(spriteBatch, _rectangle, drawColor);

            if (!string.IsNullOrEmpty(_text))
            {
                var textSize = _font.MeasureString(_text);
                var x = _rectangle.X + (_rectangle.Width - textSize.X) / 2;
                var y = _rectangle.Y + (_rectangle.Height - textSize.Y) / 2;
                spriteBatch.DrawString(_font, _text, new Vector2(x, y), _textColor);
            }
        }

        private void DrawBorder(SpriteBatch spriteBatch, Rectangle rectangle, int thickness, Color color)
        {
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
        }

        private void DrawGradientBackground(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            var gradientTexture = CreateGradientTexture(spriteBatch.GraphicsDevice, rectangle.Width, rectangle.Height, color, Color.White);
            spriteBatch.Draw(gradientTexture, rectangle, color);
        }

        private Texture2D CreateGradientTexture(GraphicsDevice graphicsDevice, int width, int height, Color startColor, Color endColor)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height);
            Color[] data = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                Color color = Color.Lerp(startColor, endColor, y / (float)height);
                for (int x = 0; x < width; x++)
                {
                    data[y * width + x] = color;
                }
            }

            texture.SetData(data);
            return texture;
        }
    }
}
