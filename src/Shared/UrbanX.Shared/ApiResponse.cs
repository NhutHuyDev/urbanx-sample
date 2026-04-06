namespace UrbanX.Shared;

/// <summary>
/// A uniform API response envelope used by all UrbanX service endpoints.
/// Wraps a result payload together with a success flag and an optional error message,
/// so clients always receive the same outer shape regardless of success or failure.
/// </summary>
/// <typeparam name="T">The type of the response payload.</typeparam>
public sealed class ApiResponse<T>
{
    /// <summary>Indicates whether the operation completed successfully.</summary>
    public bool Success { get; init; }

    /// <summary>The response data; <c>null</c> when <see cref="Success"/> is <c>false</c>.</summary>
    public T? Data { get; init; }

    /// <summary>Human-readable error message; <c>null</c> when <see cref="Success"/> is <c>true</c>.</summary>
    public string? Error { get; init; }

    private ApiResponse() { }

    /// <summary>Creates a successful response wrapping <paramref name="data"/>.</summary>
    public static ApiResponse<T> Ok(T data) =>
        new() { Success = true, Data = data };

    /// <summary>Creates a failed response with the given <paramref name="error"/> message.</summary>
    public static ApiResponse<T> Fail(string error) =>
        new() { Success = false, Error = error };
}

/// <summary>
/// A non-generic <see cref="ApiResponse{T}"/> variant for operations that return no data
/// (e.g. DELETE, void commands).
/// </summary>
public sealed class ApiResponse
{
    /// <summary>Indicates whether the operation completed successfully.</summary>
    public bool Success { get; init; }

    /// <summary>Human-readable error message; <c>null</c> when <see cref="Success"/> is <c>true</c>.</summary>
    public string? Error { get; init; }

    private ApiResponse() { }

    /// <summary>Creates a successful void response.</summary>
    public static ApiResponse Ok() => new() { Success = true };

    /// <summary>Creates a failed void response with the given <paramref name="error"/> message.</summary>
    public static ApiResponse Fail(string error) => new() { Success = false, Error = error };
}
