using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Shared
{
    public class FileDto
    {
        public FileDto()
        {

        }
        public FileDto(string fileName, Guid id, string path, string fileType)
        {
            FileName = fileName;
            Id = id;
            Path = path;
            FileType = fileType;
        }
        public string FileName { get; set; } = null!;
        public Guid Id { get; set; }
        public string Path { get; set; } = null!;
        public string FileType { get; set; } = null!;
    }
}
