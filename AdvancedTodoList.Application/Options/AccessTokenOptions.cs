﻿namespace AdvancedTodoList.Application.Options;

/// <summary>
/// A class that contains access token options.
/// </summary>
public class AccessTokenOptions
{
    /// <summary>
    /// Valid audience of tokens.
    /// </summary>
    public string ValidAudience { get; set; } = null!;

    /// <summary>
    /// Valid issuer of tokens.
    /// </summary>
    public string ValidIssuer { get; set; } = null!;

    /// <summary>
    /// Seconds before token expires.
    /// </summary>
    public int ExpirationSeconds { get; set; }

    /// <summary>
    /// A secret key used for signing access tokens.
    /// </summary>
    public string SecretKey { get; set; } = null!;
}
