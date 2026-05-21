namespace Bunker.AccountService.Messages;

/// <summary>
/// Message to notify about account updates.
/// </summary>
/// <param name="id">Identifier of user.</param>
/// <param name="nickname">User's preferred nickname.</param>
/// <param name="email">User's email.</param>
public readonly record struct AccountUpdated(string id, string nickname, string email);
