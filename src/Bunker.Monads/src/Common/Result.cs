
using System.Diagnostics;

namespace Shared.Monads.Result;
public record Result<TSuccess, TError>
{
    internal TSuccess? _result;
    internal TError? _error;
    
    internal Result(TSuccess success) => _result = success;
    internal Result(TError error) => _error = error;
    
     public TSuccess Value => _result ?? throw new AccessViolationException();   

    public bool IsFailure => _error is not null;
    public bool IsSuccess => _result is not null;
    
    public static Result<TSuccess, TError> Success(TSuccess result) => new(result);
    public static Result<TSuccess, TError> Failure(TError error) => new(error);
}

public static class ResultExtensions
{
    public static TMatch Match<TSuccess, TError, TMatch>(this Result<TSuccess, TError> result, Func<TSuccess, TMatch> onSuccess, Func<TError, TMatch> onFailure)
    {
        if (result._result is not null) return onSuccess(result._result);
        if (result._error is not null) return onFailure(result._error);
        throw new UnreachableException();
    }


    public static Result<TMapped, TError> MapSuccess<TSuccess, TError, TMapped>(this Result<TSuccess, TError> result, Func<TSuccess, TMapped> map)
    {
        return result.Match(
            onSuccess: (success) => Result<TMapped, TError>.Success(map(success)),
            onFailure: Result<TMapped, TError>.Failure
        );
    }
    public static Result<TSuccess, TError> OnSuccess<TSuccess, TError>(this Result<TSuccess, TError> result, Action<TSuccess> action)
    => result.Match(success => { action(success); return result; }, onFailure: fail => result);

    public static Result<TSuccess, TErrorMapped> MapError<TSuccess, TError, TErrorMapped>(this Result<TSuccess, TError> result, Func<TError, TErrorMapped> map)
    => result.Match(
            onSuccess: Result<TSuccess, TErrorMapped>.Success,
            onFailure: (fail) => Result<TSuccess, TErrorMapped>.Failure(map(fail))
    );
    public static Result<TSuccess, TError> OnError<TSuccess, TError>(this Result<TSuccess, TError> result, Action<TError> action)
    => result.Match(success => result, onFailure: (fail) => {action(fail); return result;});

    public static Result<T2, TError> Bind<TSuccess, TError, T2>(this Result<TSuccess, TError> result, Func<TSuccess, Result<T2, TError>> bind)
    => result.Match(
            onSuccess: (success) => bind(success),
            onFailure: Result<T2, TError>.Failure
    );

}

public static class AsyncResultExtensions
{
    public static async Task<TMatch> MatchAsync<TSuccess, TError, TMatch>(
        this Task<Result<TSuccess, TError>> task,
        Func<TSuccess, TMatch> onSuccess,
        Func<TError, TMatch> onFailure)
    {
        var result = await task;
        if (result._result is not null) return onSuccess(result._result);
        if (result._error is not null) return onFailure(result._error);
        throw new UnreachableException();
    }

    public static async Task<TMatch> MatchAsync<TSuccess, TError, TMatch>(
        this Task<Result<TSuccess, TError>> task,
        Func<TSuccess, Task<TMatch>> onSuccess,
        Func<TError, Task<TMatch>> onFailure)
    {
        var result = await task;
        if (result._result is not null) return await onSuccess(result._result);
        if (result._error is not null) return await onFailure(result._error);
        throw new UnreachableException();
    }

    public static async Task<Result<TMapped, TError>> MapSuccessAsync<TSuccess, TError, TMapped>(
        this Task<Result<TSuccess, TError>> task,
        Func<TSuccess, TMapped> map)
    {
        return await task.MatchAsync(
            onSuccess: (success) => Result<TMapped, TError>.Success(map(success)),
            onFailure: (fail) => Result<TMapped, TError>.Failure(fail)
        );
    }

    public static async Task<Result<TMapped, TError>> MapSuccessAsync<TSuccess, TError, TMapped>(
        this Task<Result<TSuccess, TError>> task,
        Func<TSuccess, Task<TMapped>> map)
    {
        return await task.MatchAsync(
            onSuccess: async success => Result<TMapped, TError>.Success(await map(success)),
            onFailure: async fail => Result<TMapped, TError>.Failure(fail)
        );
    }

    public static async Task<Result<TSuccess, TErrorMapped>> MapErrorAsync<TSuccess, TError, TErrorMapped>(
        this Task<Result<TSuccess, TError>> task,
        Func<TError, TErrorMapped> map)
    {
        return await task.MatchAsync(
            onSuccess: success => Result<TSuccess, TErrorMapped>.Success(success),
            onFailure: fail => Result<TSuccess, TErrorMapped>.Failure(map(fail))
        );
    }

    public static async Task<Result<TSuccess, TErrorMapped>> MapErrorAsync<TSuccess, TError, TErrorMapped>(
        this Task<Result<TSuccess, TError>> task,
        Func<TError, Task<TErrorMapped>> map)
    {
        return await task.MatchAsync(
            onSuccess: async success => Result<TSuccess, TErrorMapped>.Success(success),
            onFailure: async fail => Result<TSuccess, TErrorMapped>.Failure(await map(fail))
        );
    }

    public static async Task<Result<T2, TError>> BindAsync<TSuccess, TError, T2>(
        this Task<Result<TSuccess, TError>> task,
        Func<TSuccess, Task<Result<T2, TError>>> bind)
    {
        return await task.MatchAsync(
            onSuccess: async success => await bind(success),
            onFailure: async fail => Result<T2, TError>.Failure(fail)
        );
    }
}
