using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class SanctionRepository : IGenericRepositoryInterface<Sanction>
    {
        private readonly AppDbContext _appDbContext;

        public SanctionRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await _appDbContext.Sanctions.FirstOrDefaultAsync(o => o.EmployeeId == id);
            if (item is null) return NotFound();

            _appDbContext.Sanctions.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<Sanction>> GetAll() => await _appDbContext
            .Sanctions
            .AsNoTracking()
            .Include(t => t.SanctionType)
            .ToListAsync();

        public async Task<Sanction> GetById(int id)
        {
            var sanction = await _appDbContext.Sanctions.FirstOrDefaultAsync(eid => eid.EmployeeId == id);
            if (sanction is null) throw new KeyNotFoundException("Sanction not found");
            return sanction;
        }

        public async Task<GeneralResponse> Insert(Sanction item)
        {
            _appDbContext.Sanctions.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Sanction item)
        {
            var obj = await _appDbContext.Sanctions
               .FirstOrDefaultAsync(eid => eid.EmployeeId == item.EmployeeId);
            if (obj is null) return NotFound();
            obj.PunishmentDate = item.PunishmentDate;
            obj.Punishment = item.Punishment;
            obj.Date = item.Date;
            obj.SanctionType = item.SanctionType;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry, sanction not found");

        private static GeneralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await _appDbContext.SaveChangesAsync();
    }
}
