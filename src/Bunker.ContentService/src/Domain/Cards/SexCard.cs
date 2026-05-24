namespace Bunker.ContentService.Domain;

public record SexCard : Card
{
    public enum SexEnum : byte { Male = 0, Female = 1 }

    public SexEnum Sex { get; init; }

    internal SexCard(Card.Id PublicId, SexEnum Sex) : base(PublicId)
        => (this.PublicId, this.Sex) = (PublicId, Sex);

    public static SexCard CreateNew(SexEnum Sex) => CreateValid(Id.New(), Sex);
    public static SexCard Restore(Id Id, SexEnum Sex) => CreateValid(Id, Sex);

    private static SexCard CreateValid(Id Id, SexEnum Sex) => new SexCard(Id, Sex);
};

public static class SexCardExtensions
{
    extension(SexCard.SexEnum)
    {
        public static SexCard.SexEnum ParseString(ReadOnlySpan<char> @string) 
            => Enum.TryParse<SexCard.SexEnum>(@string, out var parsed) ?
            parsed
            : throw new ArgumentException("Sex must be either Male or Female.");
    }
    
    extension(SexCard card)
    {
        public SexCard WithSex(SexCard.SexEnum Sex) => card with {Sex = Sex};
        public SexCard WithSex(ReadOnlySpan<char> Sex) => card with {Sex = SexCard.SexEnum.ParseString(Sex)};
    }
}
