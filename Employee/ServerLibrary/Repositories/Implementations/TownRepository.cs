using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class TownRepository : IGenericRepositoryInterface<Town>
    {
        private readonly AppDbContext _appDbContext;

        public TownRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await _appDbContext.Towns.FindAsync(id);
            if (dep is null) return NotFound();

            _appDbContext.Towns.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Town>> GetAll() => await _appDbContext
            .Towns
            .AsNoTracking()
            .Include(t => t.City)
            .ToListAsync();

        public async Task<Town?> GetById(int id) => await _appDbContext.Towns.FindAsync(id);

        public async Task<GeneralResponse> Insert(Town item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Town already added");
            _appDbContext.Towns.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Town item)
        {
            var town = await _appDbContext.Towns.FindAsync(item.Id);
            if (town is null) return NotFound();
            town.Name = item.Name;
            town.CityId = item.CityId;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry, town not found");

        private static GeneralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await _appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await _appDbContext.Towns.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
