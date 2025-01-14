namespace Core.GenericRepository.Shared.BaseEntities
{
    public class CreationBaseEntity<TPrimaryKey> : BaseEntity<TPrimaryKey>
    {
        /// <summary>
        /// User Id for record creator
        /// </summary>
        public Guid? CreatedBy { get; set; }
        /// <summary>
        /// Creation datetime
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}
