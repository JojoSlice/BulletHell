using System;
using BulletHell.Configurations;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Helpers;

public class TurnAnimationController
{
    private enum TurnState
    {
        None,
        TurningLeft,
        TurningRight,
        ExitingTurn,
    }

    private readonly ISpriteHelper _sprite;
    private TurnState _turnState = TurnState.None;
    private Vector2 _lastDirection = Vector2.Zero;

    public TurnAnimationController(ISpriteHelper sprite)
    {
        _sprite = sprite ?? throw new ArgumentNullException(nameof(sprite));
    }

    public void Update(
        Vector2 currentDirection,
        Texture2D mainTexture,
        Texture2D? turnLeftTexture,
        Texture2D? turnRightTexture)
    {
        if (turnLeftTexture == null || turnRightTexture == null)
        {
            _lastDirection = currentDirection;
            return;
        }

        var detectedTurn = DetectTurnDirection(currentDirection, _lastDirection);

        if (_turnState == TurnState.None || _turnState == TurnState.ExitingTurn)
        {
            if (detectedTurn == TurnState.TurningLeft || detectedTurn == TurnState.TurningRight)
            {
                StartTurn(detectedTurn, turnLeftTexture, turnRightTexture);
            }
        }
        else if (_turnState == TurnState.TurningLeft || _turnState == TurnState.TurningRight)
        {
            if (detectedTurn == TurnState.None)
            {
                _turnState = TurnState.ExitingTurn;
                _sprite.StartExitSequence();
            }
            else if (detectedTurn != _turnState)
            {
                StartTurn(detectedTurn, turnLeftTexture, turnRightTexture);
            }
        }

        if (_turnState == TurnState.ExitingTurn)
        {
            if (_sprite.IsExitComplete())
            {
                if (detectedTurn == TurnState.None)
                {
                    ReturnToMainSprite(mainTexture);
                }
                else
                {
                    StartTurn(detectedTurn, turnLeftTexture, turnRightTexture);
                }
            }
            else if (detectedTurn != TurnState.None && detectedTurn != _turnState)
            {
                StartTurn(detectedTurn, turnLeftTexture, turnRightTexture);
            }
        }

        _lastDirection = currentDirection;
    }

    private TurnState DetectTurnDirection(Vector2 current, Vector2 last)
    {
        if (current == Vector2.Zero)
            return TurnState.None;

        if (current.X < -PlayerConfig.TurnDetectionThreshold &&
            last.X >= -PlayerConfig.TurnDetectionThreshold)
            return TurnState.TurningLeft;

        if (current.X > PlayerConfig.TurnDetectionThreshold &&
            last.X <= PlayerConfig.TurnDetectionThreshold)
            return TurnState.TurningRight;

        if (current.X < -PlayerConfig.TurnDetectionThreshold)
            return TurnState.TurningLeft;

        if (current.X > PlayerConfig.TurnDetectionThreshold)
            return TurnState.TurningRight;

        return TurnState.None;
    }

    private void StartTurn(TurnState turnDirection, Texture2D turnLeftTexture, Texture2D turnRightTexture)
    {
        _turnState = turnDirection;
        var turnTexture = (turnDirection == TurnState.TurningLeft)
            ? turnLeftTexture
            : turnRightTexture;

        _sprite.SetTexture(turnTexture);
        _sprite.SetSequenceAnimation(
            PlayerConfig.TurnAnimationIntroEnd,
            PlayerConfig.TurnAnimationLoopStart,
            PlayerConfig.TurnAnimationLoopEnd);
        _sprite.ResetAnimation();
    }

    private void ReturnToMainSprite(Texture2D mainTexture)
    {
        _sprite.SetTexture(mainTexture);
        _sprite.ResetToLooping();
        _sprite.ResetAnimation();
        _turnState = TurnState.None;
    }
}
