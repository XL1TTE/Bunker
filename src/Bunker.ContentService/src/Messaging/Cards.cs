namespace Bunker.ContentService.Messages;

public record ProfessionCardUpdated(Guid PublicId, string Profession);
public record HobbiesCardUpdated(Guid PublicId, string Hobbies);
public record AgeCardUpdated(Guid PublicId, int Age);
public record SexCardUpdated(Guid PublicId, byte Sex);
public record FactCardUpdated(Guid PublicId, string Fact);

public record CardDeleted(Guid PublicId);
