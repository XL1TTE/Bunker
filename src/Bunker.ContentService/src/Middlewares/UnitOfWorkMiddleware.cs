using Bunker.ContentService.Persistence.Contracts;
using Microsoft.EntityFrameworkCore;
using Wolverine.Runtime;

namespace Bunker.ContentService.Middlewares;

public class UnitOfWorkMiddleware
{
    public static async Task AfterAsync(IUnitOfWork uow, CancellationToken cancellationToken)
    {
        if(uow is not DbContext)
        {
            await uow.SaveChangesAsync(cancellationToken);
            return;
        }
    }
}
