using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class VacationRepository : IGenericRepositoryInterface<Vacation>
    {
        private readonly AppDbContext _appDbContext;

        public VacationRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await _appDbContext.Vacations.FirstOrDefaultAsync(o => o.EmployeeId == id);
            if (item is null) return NotFound();

            _appDbContext.Vacations.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<Vacation>> GetAll() => await _appDbContext
            .Vacations
            .AsNoTracking()
            .Include(t => t.VacationType)
            .ToListAsync();

        public async Task<Vacation?> GetById(int id) => await _appDbContext.Vacations.FirstOrDefaultAsync(eid => eid.EmployeeId == id);

        public async Task<GeneralResponse> Insert(Vacation item)
        {
            _appDbContext.Vacations.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Vacation item)
        {
            var obj = await _appDbContext.Vacations
                .FirstOrDefaultAsync(eid => eid.EmployeeId == item.EmployeeId);
            if (obj is null) return NotFound();
            obj.StartDate = item.StartDate;
            obj.NumberOfDays = item.NumberOfDays;
            obj.VacationTypeId = item.VacationTypeId;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry, vacation not found");

        private static GeneralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await _appDbContext.SaveChangesAsync();
    }
}
