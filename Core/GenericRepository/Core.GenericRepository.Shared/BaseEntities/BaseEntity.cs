using System.ComponentModel.DataAnnotations;

namespace Core.GenericRepository.Shared.BaseEntities
{
    public class BaseEntity<TPrimaryKey>  
    {
        /// <summary>
        /// Record Id
        /// </summary>
        [Key]
        [Required]
        public TPrimaryKey Id { get; set; }
    }
}
