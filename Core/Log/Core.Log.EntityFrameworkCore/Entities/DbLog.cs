using Core.GenericRepository.Shared.BaseEntities;

namespace Core.Log.EntityFrameworkCore.Entities
{
    public class DbLog : BaseEntity<Guid>
    {
        public string Type { get; set; }
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
    }
}
