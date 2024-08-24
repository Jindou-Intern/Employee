using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class OvertimeRepository : IGenericRepositoryInterface<Overtime>
    {
        private readonly AppDbContext _appDbContext;

        public OvertimeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await _appDbContext.Overtimes.FirstOrDefaultAsync(o => o.EmployeeId == id);
            if (item is null) return NotFound();

            _appDbContext.Overtimes.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<Overtime>> GetAll() => await _appDbContext
            .Overtimes
            .AsNoTracking()
            .Include(t => t.OvertimeType)
            .ToListAsync();

        public async Task<Overtime?> GetById(int id) => await _appDbContext.Overtimes.FirstOrDefaultAsync(eid => eid.EmployeeId == id);

        public async Task<GeneralResponse> Insert(Overtime item)
        {
            _appDbContext.Overtimes.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Overtime item)
        {
            var obj = await _appDbContext.Overtimes
                .FirstOrDefaultAsync(eid => eid.EmployeeId == item.EmployeeId);
            if (obj is null) return NotFound();
            obj.StartDate = item.StartDate;
            obj.EndDate = item.EndDate;
            obj.OvertimeTypeId = item.OvertimeTypeId;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry, overtime not found");

        private static GeneralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await _appDbContext.SaveChangesAsync();
    }
}
