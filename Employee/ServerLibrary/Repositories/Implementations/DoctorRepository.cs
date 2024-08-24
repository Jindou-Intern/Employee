using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;


namespace ServerLibrary.Repositories.Implementations
{
    public class DoctorRepository : IGenericRepositoryInterface<Doctor>
    {
        private readonly AppDbContext _appDbContext;

        public DoctorRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await _appDbContext.Doctors.FirstOrDefaultAsync(eid => eid.EmployeeId == id);
            if (item is null) return NotFound();

            _appDbContext.Doctors.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<Doctor>> GetAll() => await _appDbContext
            .Doctors
            .AsNoTracking()
            .ToListAsync();

        public async Task<Doctor?> GetById(int id) =>
            await _appDbContext.Doctors.FirstOrDefaultAsync(eid => eid.EmployeeId == id);

        public async Task<GeneralResponse> Insert(Doctor item)
        {
            _appDbContext.Doctors.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Doctor item)
        {
            var obj = await _appDbContext.Doctors.FirstOrDefaultAsync(eid => eid.EmployeeId == item.EmployeeId);
            if (obj is null) return NotFound();

            obj.MedicalRecommendation = item.MedicalRecommendation;
            obj.MedicalDiagnose = item.MedicalDiagnose;
            obj.Date = item.Date;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry, doctor not found");

        private static GeneralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await _appDbContext.SaveChangesAsync();
    }
}
