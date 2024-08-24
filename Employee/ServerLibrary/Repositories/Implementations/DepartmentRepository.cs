using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class DepartmentRepository : IGenericRepositoryInterface<Department>
    {
        private readonly AppDbContext _appDbContext;

        public DepartmentRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await _appDbContext.Departments.FindAsync(id);
            if (dep is null) return NotFound();

            _appDbContext.Departments.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Department>> GetAll() => await _appDbContext
            .Departments.AsNoTracking()
            .Include(gd => gd.GeneralDepartment)
            .ToListAsync();

        public async Task<Department?> GetById(int id) => await _appDbContext.Departments.FindAsync(id);

        public async Task<GeneralResponse> Insert(Department item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Department already added");
            _appDbContext.Departments.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Department item)
        {
            var dep = await _appDbContext.Departments.FindAsync(item.Id);
            if (dep is null) return NotFound();
            dep.GeneralDepartmentId = item.GeneralDepartmentId;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry, department not found");

        private static GeneralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await _appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await _appDbContext.Departments.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}

