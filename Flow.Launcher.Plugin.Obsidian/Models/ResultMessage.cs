using System;
using System.Diagnostics.CodeAnalysis;

namespace Flow.Launcher.Plugin.Obsidian.Models;

/// <summary>
///     Bool result with an error message
/// </summary>
public readonly struct ResultMessage
{
    private ResultMessage(bool isSuccess, string? errorMessage)
    {
        switch (isSuccess)
        {
            case true when errorMessage is not null:
                throw new ArgumentException("ErrorMessage must be null on success.", nameof(errorMessage));
            case false when string.IsNullOrWhiteSpace(errorMessage):
                throw new ArgumentException("ErrorMessage must be provided on failure.", nameof(errorMessage));
            default:
                IsSuccess = isSuccess;
                ErrorMessage = errorMessage;
                break;
        }
    }

    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; }

    public string? ErrorMessage { get; }

    public static ResultMessage Success() => new(true, null);
    public static ResultMessage Fail(string message) => new(false, message);
}
