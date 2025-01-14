namespace Core.GenericRepository.Shared.BaseEntities
{
    public class ModificationBaseEntity<TPrimaryKey> : BaseEntity<TPrimaryKey>
    {
        /// <summary>
        /// User Id for record updater
        /// </summary>
        public Guid? UpdatedBy { get; set; }
        /// <summary>
        /// Update datetime
        /// </summary>
        public DateTime? UpdatedOn { get; set; }
    }
}
