using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class BranchRepository : IGenericRepositoryInterface<Branch>
    {
        private readonly AppDbContext _appDbContext;

        public BranchRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await _appDbContext.Branches.FindAsync(id);
            if (dep is null) return NotFound();

            _appDbContext.Branches.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Branch>> GetAll() => await _appDbContext
            .Branches
            .AsNoTracking()
            .Include(d => d.Department)
            .ToListAsync();

        public async Task<Branch?> GetById(int id) => await _appDbContext.Branches.FindAsync(id);

        public async Task<GeneralResponse> Insert(Branch item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Branch already added");
            _appDbContext.Branches.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Branch item)
        {
            var branch = await _appDbContext.Branches.FindAsync(item.Id);
            if (branch is null) return NotFound();
            branch.Name = item.Name;
            branch.DepartmentId = item.DepartmentId;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry, branch not found");

        private static GeneralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await _appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await _appDbContext.Branches.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
