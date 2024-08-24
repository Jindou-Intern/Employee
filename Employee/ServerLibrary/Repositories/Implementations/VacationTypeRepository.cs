using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class VacationTypeRepository : IGenericRepositoryInterface<VacationType>
    {
        private readonly AppDbContext _appDbContext;

        public VacationTypeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await _appDbContext.VacationTypes.FindAsync(id);
            if (item is null) return NotFound();

            _appDbContext.VacationTypes.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<VacationType>> GetAll() => await _appDbContext
            .VacationTypes
            .AsNoTracking()
            .ToListAsync();

        public async Task<VacationType?> GetById(int id) => await _appDbContext.VacationTypes.FindAsync(id);

        public async Task<GeneralResponse> Insert(VacationType item)
        {
            if (!await CheckName(item.Name)) return new GeneralResponse(false, "Vacation Type already added");
            _appDbContext.VacationTypes.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(VacationType item)
        {
            var obj = await _appDbContext.VacationTypes.FindAsync(item.Id);
            if (obj is null) return NotFound();
            obj.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry Vacation Type not found");

        private static GeneralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await _appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await _appDbContext.OvertimeTypes.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
