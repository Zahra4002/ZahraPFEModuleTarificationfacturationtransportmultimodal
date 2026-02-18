using Application.Interfaces;
using Domain.Interfaces;

namespace Persistance.Data
{
    public class CleanArchitectureUnitOfWork : UnitOfWork<CleanArchitecturContext>, ICleanArchitecturUnitOfWork
    {
        public CleanArchitectureUnitOfWork(IContext context) : base(context)
        {
        }
    }
}
