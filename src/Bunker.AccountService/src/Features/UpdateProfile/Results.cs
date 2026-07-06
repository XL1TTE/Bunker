using Bunker.AccountService.Features.GetProfile;

namespace Bunker.AccountService.Features.UpdateProfile;

public abstract record UpdateProfileResult;
public record UpdateProfileSuccess(PlayerProfileResponse Profile) : UpdateProfileResult;
public record UpdateProfileNotFound : UpdateProfileResult;

public static class UpdateProfileResultFactory
{
    extension(UpdateProfileResult)
    {
        public static UpdateProfileResult Success(PlayerProfileResponse profile) => new UpdateProfileSuccess(profile);
        public static UpdateProfileResult NotFound() => new UpdateProfileNotFound();
    }
}