using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class SanctionTypeRepository : IGenericRepositoryInterface<SanctionType>
    {
        private readonly AppDbContext _appDbContext;

        public SanctionTypeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await _appDbContext.SanctionTypes.FindAsync(id);
            if (item is null) return NotFound();

            _appDbContext.SanctionTypes.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<SanctionType>> GetAll() => await _appDbContext
            .SanctionTypes
            .AsNoTracking()
            .ToListAsync();

        public async Task<SanctionType?> GetById(int id) => await _appDbContext.SanctionTypes.FindAsync(id);

        public async Task<GeneralResponse> Insert(SanctionType item)
        {
            if (!await CheckName(item.Name)) return new GeneralResponse(false, "Sanction Type already added");
            _appDbContext.SanctionTypes.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(SanctionType item)
        {
            var obj = await _appDbContext.SanctionTypes.FindAsync(item.Id);
            if (obj is null) return NotFound();
            obj.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry, Sanction Type not found");

        private static GeneralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await _appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await _appDbContext.SanctionTypes.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
