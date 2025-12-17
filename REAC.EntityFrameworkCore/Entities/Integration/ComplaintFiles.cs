using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Integration
{
    [Table("ComplaintFile")] 
    public class ComplaintFile : BaseEntity<Guid>
    {
       
        [Required]
        [Column("NoteId")]
        public Guid NoteId { get; set; }


        [Required]
        [Column("CaseId")]
        public Guid CaseId { get; set; }


        
        [Column("FileManagerId")]
        public Guid? FileManagerId { get; set; }


        [Column("CreatedBy")]
        public Guid? CreatedBy { get; set; } 

        [Required]
        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [Column("IsDeleted")]
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(FileManagerId))]
        public virtual SysFileManager? FileManager { get; set; }

       
    }
}
