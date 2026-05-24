
namespace Bunker.ContentService.Persistence.Contracts;

public interface ICardQueries
{
    /// <summary>
    /// Gets profession cards with pagination.
    /// </summary>
    /// <param name="skip">Cards to skip.</param>
    /// <param name="take">Cards to take.</param>
    /// <returns>Tuple with total cards in the database and cards after pagination applied.</returns>
    Task<(int, IReadOnlyCollection<Domain.ProfessionCard>)> GetProfessionCardsAsync(int skip, int take);
    
    /// <summary>
    /// Gets hobbies cards with pagination.
    /// </summary>
    /// <param name="skip">Cards to skip.</param>
    /// <param name="take">Cards to take.</param>
    /// <returns>Tuple with total cards in the database and cards after pagination applied.</returns>
    Task<(int, IReadOnlyCollection<Domain.HobbiesCard>)> GetHobbiesCardsAsync(int skip, int take);
    
    /// <summary>
    /// Gets age cards with pagination.
    /// </summary>
    /// <param name="skip">Cards to skip.</param>
    /// <param name="take">Cards to take.</param>
    /// <returns>Tuple with total cards in the database and cards after pagination applied.</returns>
    Task<(int, IReadOnlyCollection<Domain.AgeCard>)> GetAgeCardsAsync(int skip, int take);
    
    /// <summary>
    /// Gets sex cards with pagination.
    /// </summary>
    /// <param name="skip">Cards to skip.</param>
    /// <param name="take">Cards to take.</param>
    /// <returns>Tuple with total cards in the database and cards after pagination applied.</returns>
    Task<(int, IReadOnlyCollection<Domain.SexCard>)> GetSexCardsAsync(int skip, int take);
    
    /// <summary>
    /// Gets fact cards with pagination.
    /// </summary>
    /// <param name="skip">Cards to skip.</param>
    /// <param name="take">Cards to take.</param>
    /// <returns>Tuple with total cards in the database and cards after pagination applied.</returns>
    Task<(int, IReadOnlyCollection<Domain.FactCard>)> GetFactCardsAsync(int skip, int take);
}
