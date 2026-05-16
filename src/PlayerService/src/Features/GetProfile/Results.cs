namespace PlayerService.Features.GetProfile;

public abstract record GetProfileResult;
public record GetProfileSuccess(PlayerProfileResponse Profile) : GetProfileResult;
public record GetProfileNotFound : GetProfileResult;

public static class GetProfileResultFactory
{
    extension(GetProfileResult)
    {
        public static GetProfileResult Success(PlayerProfileResponse profile) => new GetProfileSuccess(profile);
        public static GetProfileResult NotFound() => new GetProfileNotFound();
    }
}
