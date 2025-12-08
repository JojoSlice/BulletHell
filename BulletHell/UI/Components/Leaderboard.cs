using System.Collections.Generic;
using System.Linq;
using BulletHell.Helpers;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.UI.Components;

public class Leaderboard
{
    private const int MaxEntries = 10;
    private const int PaddingLeft = 20;
    private const int PaddingTop = 20;
    private const int LineHeight = 30;
    private const int TitleSpacing = 40;

    private readonly SpriteFont _font;
    private readonly int _xPosition;
    private readonly int _yPosition;
    private List<HighScoreResult> _highScores = new();

    public Leaderboard(SpriteFont font, int xPosition = PaddingLeft, int yPosition = PaddingTop)
    {
        _font = font;
        _xPosition = xPosition;
        _yPosition = yPosition;
    }

    public void UpdateHighScores(List<HighScoreResult> highScores)
    {
        _highScores = highScores.OrderByDescending(hs => hs.Score).Take(MaxEntries).ToList();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_highScores == null || _highScores.Count == 0)
        {
            return;
        }

        var title = "TOP 10 HIGHSCORES";
        var titleSize = _font.MeasureString(title);
        spriteBatch.DrawString(_font, title, new Vector2(_xPosition, _yPosition), Color.Gold);

        var backgroundWidth = 280;
        var backgroundHeight = TitleSpacing + (_highScores.Count * LineHeight) + 10;
        spriteBatch.Draw(
            TextureHelper.WhitePixel(spriteBatch.GraphicsDevice),
            new Rectangle(_xPosition - 5, _yPosition - 5, backgroundWidth, backgroundHeight),
            Color.Black * 0.7f
        );

        var yOffset = _yPosition + TitleSpacing;
        for (int i = 0; i < _highScores.Count; i++)
        {
            var highScore = _highScores[i];
            var rank = i + 1;

            var userText = $"{rank}. {highScore.UserName}";
            var scoreText = $"{highScore.Score}";

            spriteBatch.DrawString(
                _font,
                userText,
                new Vector2(_xPosition, yOffset),
                i < 3 ? Color.Yellow : Color.White // Gör top 3 gula
            );

            var scoreSize = _font.MeasureString(scoreText);
            spriteBatch.DrawString(
                _font,
                scoreText,
                new Vector2(_xPosition + backgroundWidth - scoreSize.X - 10, yOffset),
                i < 3 ? Color.Yellow : Color.White
            );

            yOffset += LineHeight;
        }
    }

    public void Clear()
    {
        _highScores.Clear();
    }
}