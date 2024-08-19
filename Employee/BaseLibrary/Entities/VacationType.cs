
namespace BaseLibrary.Entities
{
    public class VacationType:BaseEntity
    {
        //Manay to one relationship with Vacation
        public List<Vacation>? Vacations { get; set; }
    }
}
