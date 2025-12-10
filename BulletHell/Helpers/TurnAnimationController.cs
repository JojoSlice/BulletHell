using System;
using BulletHell.Configurations;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Helpers;

public class TurnAnimationController
{
    internal enum TurnState
    {
        None,
        TurningLeft,
        TurningRight,
        ExitingTurn,
    }

    public enum SpriteAction
    {
        NoChange,
        StartTurnLeft,
        StartTurnRight,
        StartExitSequence,
        ReturnToMain,
        CompleteExitAndTurnLeft,
        CompleteExitAndTurnRight,
    }

    private readonly ISpriteHelper _sprite;
    private TurnState CurrentTurnState { get; set; } = TurnState.None;
    private Vector2 _lastDirection = Vector2.Zero;

    internal TurnState CurrentStateForTesting
    {
        get => CurrentTurnState;
        set => CurrentTurnState = value;
    }

    internal Vector2 LastDirectionForTesting
    {
        get => _lastDirection;
        set => _lastDirection = value;
    }

    public TurnAnimationController(ISpriteHelper sprite)
    {
        _sprite = sprite ?? throw new ArgumentNullException(nameof(sprite));
    }

    public void Update(
        Vector2 currentDirection,
        Texture2D mainTexture,
        Texture2D? turnLeftTexture,
        Texture2D? turnRightTexture
    )
    {
        bool hasValidTextures = turnLeftTexture != null && turnRightTexture != null;

        var action = DetermineNextAction(currentDirection, hasValidTextures);

        ApplySpriteAction(action, mainTexture, turnLeftTexture, turnRightTexture);

        _lastDirection = currentDirection;
    }

    private TurnState DetectTurnDirection(Vector2 current, Vector2 last)
    {
        if (current == Vector2.Zero)
            return TurnState.None;

        if (
            current.X < -PlayerConfig.TurnDetectionThreshold
            && last.X >= -PlayerConfig.TurnDetectionThreshold
        )
            return TurnState.TurningLeft;

        if (
            current.X > PlayerConfig.TurnDetectionThreshold
            && last.X <= PlayerConfig.TurnDetectionThreshold
        )
            return TurnState.TurningRight;

        if (current.X < -PlayerConfig.TurnDetectionThreshold)
            return TurnState.TurningLeft;

        if (current.X > PlayerConfig.TurnDetectionThreshold)
            return TurnState.TurningRight;

        return TurnState.None;
    }

    public SpriteAction DetermineNextAction(Vector2 currentDirection, bool hasValidTextures)
    {
        if (!hasValidTextures)
        {
            return SpriteAction.NoChange;
        }

        var detectedTurn = DetectTurnDirection(currentDirection, _lastDirection);

        if (CurrentTurnState == TurnState.None)
        {
            if (detectedTurn == TurnState.TurningLeft)
            {
                return SpriteAction.StartTurnLeft;
            }
            else if (detectedTurn == TurnState.TurningRight)
            {
                return SpriteAction.StartTurnRight;
            }
        }
        else if (CurrentTurnState is TurnState.TurningLeft or TurnState.TurningRight)
        {
            if (detectedTurn == TurnState.None)
            {
                return SpriteAction.StartExitSequence;
            }
            else if (detectedTurn != CurrentTurnState)
            {
                return detectedTurn == TurnState.TurningLeft
                    ? SpriteAction.StartTurnLeft
                    : SpriteAction.StartTurnRight;
            }
        }

        if (CurrentTurnState == TurnState.ExitingTurn)
        {
            if (_sprite.IsExitComplete())
            {
                if (detectedTurn == TurnState.None)
                {
                    return SpriteAction.ReturnToMain;
                }
                else if (detectedTurn == TurnState.TurningLeft)
                {
                    return SpriteAction.CompleteExitAndTurnLeft;
                }
                else if (detectedTurn == TurnState.TurningRight)
                {
                    return SpriteAction.CompleteExitAndTurnRight;
                }
            }
            else if (detectedTurn != TurnState.None && detectedTurn != CurrentTurnState)
            {
                return detectedTurn == TurnState.TurningLeft
                    ? SpriteAction.StartTurnLeft
                    : SpriteAction.StartTurnRight;
            }
        }

        return SpriteAction.NoChange;
    }

    private void ApplySpriteAction(
        SpriteAction action,
        Texture2D mainTexture,
        Texture2D? turnLeftTexture,
        Texture2D? turnRightTexture
    )
    {
        switch (action)
        {
            case SpriteAction.NoChange:
                break;

            case SpriteAction.StartTurnLeft:
                StartTurn(TurnState.TurningLeft, turnLeftTexture!, turnRightTexture!);
                break;

            case SpriteAction.StartTurnRight:
                StartTurn(TurnState.TurningRight, turnLeftTexture!, turnRightTexture!);
                break;

            case SpriteAction.StartExitSequence:
                CurrentTurnState = TurnState.ExitingTurn;
                _sprite.StartExitSequence();
                break;

            case SpriteAction.ReturnToMain:
                ReturnToMainSprite(mainTexture);
                break;

            case SpriteAction.CompleteExitAndTurnLeft:
                StartTurn(TurnState.TurningLeft, turnLeftTexture!, turnRightTexture!);
                break;

            case SpriteAction.CompleteExitAndTurnRight:
                StartTurn(TurnState.TurningRight, turnLeftTexture!, turnRightTexture!);
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(action),
                    action,
                    "Unknown sprite action"
                );
        }
    }

    private void StartTurn(
        TurnState turnDirection,
        Texture2D turnLeftTexture,
        Texture2D turnRightTexture
    )
    {
        CurrentTurnState = turnDirection;
        var turnTexture =
            (turnDirection == TurnState.TurningLeft) ? turnLeftTexture : turnRightTexture;

        _sprite.SetTexture(turnTexture);
        _sprite.SetSequenceAnimation(
            PlayerConfig.TurnAnimationIntroEnd,
            PlayerConfig.TurnAnimationLoopStart,
            PlayerConfig.TurnAnimationLoopEnd
        );
        _sprite.ResetAnimation();
    }

    private void ReturnToMainSprite(Texture2D mainTexture)
    {
        _sprite.SetTexture(mainTexture);
        _sprite.ResetToLooping();
        _sprite.ResetAnimation();
        CurrentTurnState = TurnState.None;
    }
}