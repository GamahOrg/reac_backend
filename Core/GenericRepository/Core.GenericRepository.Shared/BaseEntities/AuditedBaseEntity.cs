namespace Core.GenericRepository.Shared.BaseEntities
{
    public class AuditedBaseEntity<TPrimaryKey> : BaseEntity<TPrimaryKey>
    {
        /// <summary>
        /// User Id for record creator
        /// </summary>
        public Guid? CreatedBy { get; set; }
        /// <summary>
        /// Creation datetime
        /// </summary>
        public DateTime CreatedOn { get; set; }
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
