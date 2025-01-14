namespace Core.GenericRepository.Shared.BaseEntities
{
    public class SoftDeleteBaseEntity<TPrimaryKey> : BaseEntity<TPrimaryKey>
    {
        public bool IsDeleted { get; set; }
        /// <summary>
        /// User Id for record deleter
        /// </summary>
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }    
    }
}
