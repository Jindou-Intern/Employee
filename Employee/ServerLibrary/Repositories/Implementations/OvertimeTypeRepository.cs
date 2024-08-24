using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class OvertimeTypeRepository : IGenericRepositoryInterface<OvertimeType>
    {
        private readonly AppDbContext _appDbContext;

        public OvertimeTypeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await _appDbContext.OvertimeTypes.FindAsync(id);
            if (item is null) return NotFound();

            _appDbContext.OvertimeTypes.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<OvertimeType>> GetAll() => await _appDbContext
            .OvertimeTypes
            .AsNoTracking()
            .ToListAsync();

        public async Task<OvertimeType?> GetById(int id) => await _appDbContext.OvertimeTypes.FindAsync(id);

        public async Task<GeneralResponse> Insert(OvertimeType item)
        {
            if (!await CheckName(item.Name)) return new GeneralResponse(false, "Overtime Type already added");
            _appDbContext.OvertimeTypes.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(OvertimeType item)
        {
            var obj = await _appDbContext.OvertimeTypes.FindAsync(item.Id);
            if (obj is null) return NotFound();
            obj.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "OvertimeType not found");

        private static GeneralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await _appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await _appDbContext.OvertimeTypes.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
