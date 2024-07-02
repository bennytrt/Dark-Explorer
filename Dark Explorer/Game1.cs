using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Xml.Schema;
using System.Windows;
using System.Reflection.Emit;

namespace Dark_Explorer
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _playerTexture;
        AnimationPlayer AnimationPlayer;

        Texture2D walkingTexture;

        // Player Assets
        private Vector2 _playerPosition;
        private Vector2 _playerVelocity;
        private bool _isOnGround;

        // Physics Assets
        private float _playerSpeed;
        private float _gravity;
        private float _jumpStrength;

        // Shadow Assets
        private Vector2 _shadowOffset;
        private Color _shadowColor;

        // Game State
        private GameState _currentGameState;

        // Game Statistics
        private GameStats _gameStats;

        // Main Menu Assets
        private SpriteFont _font;
        private List<Button> _menuButtons;
        private List<Button> _pauseButtons;
        private List<Button> _gameoverButtons;

        // Keyboard And Mouse state
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        // Debounce Variables
        private bool _pauseKeyPressed;
        private bool _pauseKeyReleased;

        // Window Res + Deadzone Integer
        private Point GameBounds = new Point(1280, 720);
        int deadZone;

        public Game1()
        {
           
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = GameBounds.X;
            _graphics.PreferredBackBufferHeight = GameBounds.Y;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _playerPosition = new Vector2(100, 100);
            _playerSpeed = 200f;
            _playerVelocity = Vector2.Zero;
            _isOnGround = false;

            deadZone = 4096;

            _gravity = 9.8f * 100;
            _jumpStrength = -350f;

            _shadowOffset = new Vector2(10, 10);
            _shadowColor = new Color(0, 0, 0, 0.5f);

            _currentGameState = GameState.MainMenu;
            _gameStats = new GameStats();
            _pauseKeyPressed = false;
            _pauseKeyReleased = true;


            base.Initialize();
        }

        protected override void LoadContent()
        {
            walkingTexture = Content.Load<Texture2D>("Run");
            AnimationPlayer = new AnimationPlayer(walkingTexture);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //_playerTexture = Content.Load<Texture2D>("idle");
            _font = Content.Load<SpriteFont>("font");

            var buttonTexture = new Texture2D(GraphicsDevice, 1, 1);
            buttonTexture.SetData(new[] { Color.White });
            var buttonFont = Content.Load<SpriteFont>("font");
            _menuButtons = new List<Button>
            {
                new Button(buttonTexture, buttonFont, "Start Game", new Rectangle(300, 200, 200, 50), Color.DarkRed, Color.LightGray, Color.Black, 2, Color.Black * 0.5f, 5, Color.White),
                new Button(buttonTexture, buttonFont, "Quit", new Rectangle(300, 300, 200, 50), Color.DarkBlue, Color.LightGray, Color.Black, 2, Color.Black * 0.5f, 5, Color.White)
            };

            _pauseButtons = new List<Button>
            {
                new Button(buttonTexture, buttonFont, "Resume Game", new Rectangle(300, 200, 200, 50), Color.DarkSlateGray, Color.LightGray, Color.Red, 2, Color.Black * 0.5f, 5, Color.White),
                new Button(buttonTexture, buttonFont, "Quit To Menu", new Rectangle(300, 300, 200, 50), Color.DarkSlateGray, Color.LightGray, Color.Red, 2, Color.Black * 0.5f, 5, Color.White)
            };

            _gameoverButtons = new List<Button>
            {
                new Button(buttonTexture, buttonFont, "Restart Game", new Rectangle(300, 200, 200, 50), Color.DarkSlateGray, Color.LightGray, Color.Black, 2, Color.Black * 0.5f, 5, Color.White),
                new Button(buttonTexture, buttonFont, "Quit", new Rectangle(300, 300, 200, 50), Color.DarkSlateGray, Color.LightGray, Color.Black, 2, Color.Black * 0.5f, 5, Color.White)
            };
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandlePauseInput();

            switch (_currentGameState)
            {
                case GameState.MainMenu:
                    UpdateMainMenu();
                    break;

                case GameState.Playing:
                    UpdatePlaying(gameTime, deltaTime);
                    AnimationPlayer.Update(gameTime);
                    break;

                case GameState.Paused:
                    UpdatePaused();
                    break;

                case GameState.GameOver:
                    UpdateGameOver();
                    break;
            }
            if (_currentGameState == GameState.Playing)
            {
                // Update player position
                _playerPosition += _playerVelocity * deltaTime;

                // Update animation
                //_playerAnimation.Update(gameTime);
            }
            //AnimationPlayer.Update(gameTime);
            _previousKeyboardState = _currentKeyboardState;
            _previousMouseState = _currentMouseState;
            base.Update(gameTime);
        }

        private void HandlePauseInput()
        {
            if (_currentKeyboardState.IsKeyDown(Keys.P))
            {
                if (_pauseKeyReleased)
                {
                    _pauseKeyPressed = true;
                    _pauseKeyReleased = false;
                }
            }
            else
            {
                _pauseKeyReleased = true;
            }

            if (_pauseKeyPressed)
            {
                if (_currentGameState == GameState.Playing)
                {
                    _currentGameState = GameState.Paused;
                }
                else if (_currentGameState == GameState.Paused)
                {
                    _currentGameState = GameState.Playing;
                }
                _pauseKeyPressed = false;
            }
        }

        private void UpdateMainMenu()
        {
            foreach (var button in _menuButtons)
            {
                button.Update(_currentMouseState);
                if (button.IsClicked(_currentMouseState) && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    if (button == _menuButtons[0])
                    {
                        _currentGameState = GameState.Playing;
                        _playerPosition = new Vector2(100, 300);
                        _playerVelocity = Vector2.Zero; 
                        _gameStats.ResetStats();
                    }
                    else if (button == _menuButtons[1])
                    {
                        Exit();
                    }
                }
            }
        }

        private void UpdatePlaying(GameTime gameTime, float deltaTime)
        {
            var keyboardState = _currentKeyboardState;

            if (keyboardState.IsKeyDown(Keys.Left))
                _playerPosition.X -= _playerSpeed * deltaTime;
            if (keyboardState.IsKeyDown(Keys.Right))
                _playerPosition.X += _playerSpeed * deltaTime;

            if (_isOnGround && keyboardState.IsKeyDown(Keys.Up ))
            {
                _playerVelocity.Y = _jumpStrength;
                _isOnGround = false;
            }

            _playerVelocity.Y += _gravity * deltaTime;
            _playerPosition += _playerVelocity * deltaTime;

            if (_playerPosition.Y >= 400)
            {
                _playerPosition.Y = 400;
                _playerVelocity.Y = 0;
                _isOnGround = true;
            }

            if (_playerPosition.Y > GraphicsDevice.Viewport.Height)
            {
                _currentGameState = GameState.GameOver;
            }
            _gameStats.AddScore(1); 

            if (_gameStats.IsGameOver())
            {
                _currentGameState = GameState.GameOver;
            }
        }

        private void UpdatePaused()
        {
            foreach (var button in _pauseButtons)
            {
                button.Update(_currentMouseState);
                if (button.IsClicked(_currentMouseState) && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    if (button == _pauseButtons[0])
                    {
                        _currentGameState = GameState.Playing;
                    }
                    else if (button == _pauseButtons[1])
                    {
                        _currentGameState = GameState.MainMenu;
                    }
                }
            }
        }

        private void UpdateGameOver()
        {
            if (IsKeyPressed(Keys.Enter))
            {
                _currentGameState = GameState.MainMenu;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            switch (_currentGameState)
            {
                case GameState.MainMenu:
                    DrawMainMenu();
                    break;

                case GameState.Playing:
                    DrawPlaying();
                    //_playerAnimation.Draw(_spriteBatch);
                    AnimationPlayer.Draw(_spriteBatch);
                    break;

                case GameState.Paused:
                    DrawPaused();
                    break;

                case GameState.GameOver:
                    DrawGameOver();
                    break;
            }
            //AnimationPlayer.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private void DrawMainMenu()
        {
            foreach (var button in _menuButtons)
            {
                button.Draw(_spriteBatch);
            }
        }

        private void DrawPlaying()
        {
            _spriteBatch.Draw(walkingTexture, _playerPosition + _shadowOffset, _shadowColor);

            _spriteBatch.Draw(walkingTexture, _playerPosition, Color.White);
            _spriteBatch.DrawString(_font, $"Score: {_gameStats.Score}", new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(_font, $"Health: {_gameStats.Health}", new Vector2(10, 30), Color.White);
            IsMouseVisible = false;
        }

        private void DrawPaused()
        {
            foreach (var button in _pauseButtons)
            {
                button.Draw(_spriteBatch);
                IsMouseVisible = true;
            }
        }

        private void DrawGameOver()
        {
            string message = "Game Over! Press Enter to return to Main Menu";
            Vector2 size = _font.MeasureString(message);
            _spriteBatch.DrawString(_font, message, new Vector2((GraphicsDevice.Viewport.Width - size.X) / 2, (GraphicsDevice.Viewport.Height - size.Y) / 2), Color.Red);
        }

        private bool IsKeyPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }
    }
}