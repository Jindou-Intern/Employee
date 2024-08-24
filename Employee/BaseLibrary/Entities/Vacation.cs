using System;
using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public class Vacation : OtherBaseEntity
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public int NumberOfDays { get; set; }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                NumberOfDays = (int)(_endDate - StartDate).TotalDays;
            }
        }

        // Many to one relationship with Vacation Type
        public VacationType? VacationType { get; set; }

        [Required]
        public int VacationTypeId { get; set; }

        public Vacation()
        {
            _endDate = StartDate.AddDays(NumberOfDays);
        }
    }
}
