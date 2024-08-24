using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class EmployeeRepository : IGenericRepositoryInterface<Employee>
    {
        private readonly AppDbContext _appDbContext;

        public EmployeeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await _appDbContext.Employees.FindAsync(id);
            if (item is null) return NotFound();

            _appDbContext.Employees.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<Employee>> GetAll()
        {
            var employees = await _appDbContext.Employees
                .AsNoTracking()
                .Include(t => t.Town)
                    .ThenInclude(b => b.City)
                    .ThenInclude(c => c.Country)
                .Include(b => b.Branch)
                    .ThenInclude(d => d.Department)
                    .ThenInclude(gd => gd.GeneralDepartment)
                .ToListAsync();
            return employees;
        }

        public async Task<Employee?> GetById(int id)
        {
            var employee = await _appDbContext.Employees
                .Include(t => t.Town)
                    .ThenInclude(b => b.City)
                    .ThenInclude(c => c.Country)
                .Include(b => b.Branch)
                    .ThenInclude(d => d.Department)
                    .ThenInclude(gd => gd.GeneralDepartment)
                .FirstOrDefaultAsync(ei => ei.Id == id);
            return employee;
        }

        public async Task<GeneralResponse> Insert(Employee item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Employee already added");

            _appDbContext.Employees.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Employee employee)
        {
            var findUser = await _appDbContext.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id);
            if (findUser is null) return new GeneralResponse(false, "Employee does not exist");

            findUser.Name = employee.Name;
            findUser.Other = employee.Other;
            findUser.Address = employee.Address;
            findUser.TelephoneNumber = employee.TelephoneNumber;
            findUser.BranchId = employee.BranchId;
            findUser.TownId = employee.TownId;
            findUser.CivilId = employee.CivilId;
            findUser.FileNumber = employee.FileNumber;
            findUser.JobName = employee.JobName;
            findUser.Photo = employee.Photo;

            await Commit();
            return Success();
        }

        private async Task Commit() => await _appDbContext.SaveChangesAsync();

        private static GeneralResponse NotFound() => new(false, "Sorry, employee not found");

        private static GeneralResponse Success() => new(true, "Process completed");

        private async Task<bool> CheckName(string name)
        {
            var item = await _appDbContext.Employees.FirstOrDefaultAsync(x => x.Name!.ToLower() == name.ToLower());
            return item is null;
        }
    }
}
