namespace Core.GenericRepository.Shared.BaseEntities
{
    public class FullAuditedBaseEntity<TPrimaryKey> : BaseEntity<TPrimaryKey>
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
        public bool IsDeleted { get; set; }

        /// <summary>
        /// User Id for record deleter
        /// </summary>
        public Guid? DeletedBy { get; set; }
        /// <summary>
        /// Delete datetime
        /// </summary>
        public DateTime? DeletedOn { get; set; }
    }
}
