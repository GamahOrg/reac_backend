using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Common;
using Core.GenericRepository.Shared.BaseEntities;
using Core.Localizations;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts;
using REAC.Application.Contracts.Dtos.Shared;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.Domain.Shared.Helpers;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using System.Net.Http.Headers;

namespace REAC.Application.AppServices.System
{
    public class SysFileManagerService : BasicService<SysFileManager, Guid>, ISysFileManagerService
    {

        public SysFileManagerService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }
        public async Task<BaseResponse> AddFile(SysFileManagerCreateUpdateDto model)
        {
            return await base.AddAsync(model);
        }
        public async Task<Guid> GetLatestFile()
        {
            var queryable = await GetQueryableAsync();
            Guid id = queryable.Count() > 0 ? queryable.OrderByDescending(x => x.CreatedOn).First().Id : Guid.NewGuid();
            return id;
        }
        public async Task<List<SysFileManager>> GetAsync(List<Guid>? ids)
        {
            if (ids == null) return new List<SysFileManager>();
            var queryable = await GetQueryableAsync();
            return await queryable
                .Where(file => ids.Contains(file.Id))
                .ToListAsync();
        }
        public async Task RemoveAsync(Guid id)
        {
            var queryable = await GetQueryableAsync();
            var attachment = await queryable.FirstOrDefaultAsync(a => a.Id == id);
            //if (attachment == null)
            // throw new ExceptionResponse("TicketFilesNotFound");

            var folderName = Path.Combine(SharedConstants.RootFolder);
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var d = new DirectoryInfo(pathToSave);
            var file = d.GetFiles().FirstOrDefault(x => x.Name.Contains(id.ToString()));
            //if (file == null)
            //    throw new ExceptionResponse("TicketFilesNotFound");

            if (File.Exists(file.FullName))
            {
                File.Delete(file.FullName);
                _repository.Remove<SysFileManager, Guid>(attachment);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task<BaseResponse> UploadAsync(IFormFileCollection files)
        {
            if (files == null)
                throw new ArgumentNullException(nameof(files));

            var allowedExtensions = new List<string> { ".jpg", ".png", ".pdf", ".docx", ".xlsx", ".xls",".jpeg" };
            var pathToSave = CreateUploadRootFolder();
            List<FileDto> uploadedFiles = new();
            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var extension = ExtractExtension(file, out var contentDisposition);

                var fileName = contentDisposition!.FileName!.Trim('"');
                // Validate file extension 
                if (!allowedExtensions.Contains(extension.ToLower()))
                {
                    throw new Exception();
                    // throw new ExceptionResponse(Localizations.ThisFileTypeNotAllowed);
                }

                //if(extension.ToLower() == ".pdf")
                //{
                //    var pdfMaliciousResult = await FileHelper.CheckMaliciousPdf(file);

                //    if (!pdfMaliciousResult.IsDone)
                //        return pdfMaliciousResult;
                //}

                var maliciousResult = await FileHelper.CheckMaliciousFile(file);

                if (!maliciousResult.IsDone)
                    return maliciousResult;


                var id = Guid.NewGuid();
                var path = Path.Combine(SharedConstants.RootFolder, id + extension);
                path = path.Replace(@"\", @"/");
                var uploadedFile = new FileDto(fileName, id, path, "");
                await CreateNewFile(uploadedFile, extension, file);
                await OpenFileStreamAsync(pathToSave, uploadedFile, extension, file);
                uploadedFiles.Add(uploadedFile);
            }

            await _repository.SaveChangesAsync();
            return new DataResponse<List<FileDto>> { IsDone = true, Message = Localizations.UploadSuccefully, Data = uploadedFiles };
        }

        private static string CreateUploadRootFolder()
        {
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), SharedConstants.RootFolder);
            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(pathToSave);
            return pathToSave;
        }


        #region Private Methods

        private static string ExtractExtension(IFormFile file, out ContentDispositionHeaderValue? contentDisposition)
        {
            var extension = Path.GetExtension(file.FileName);
            contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            if (contentDisposition == null || string.IsNullOrEmpty(contentDisposition.FileName))
                throw new ArgumentNullException(nameof(extension));
            return extension;
        }

        private async Task CreateNewFile(FileDto uploadedFile, string extension, IFormFile file)
        {
            var record = new SysFileManager
            {
                Id = uploadedFile.Id,
                FileDescription = uploadedFile.FileName,
                FilePath = uploadedFile.Path,
                CreatedOn = DateTime.UtcNow,
                FileType = extension.Replace(".", "")
            };
            if (extension.Contains(MimeTypes.Pdf))
            {
                // var bytes = await file.GetBytes();
                // record.NumberOfPages = FormFileExtensions.PageCount(bytes);
            }

            //record.Size = file.Length;
            await _repository.AddAsync<SysFileManager, Guid>(record);
            uploadedFile.FileType = extension;
        }

        private static async Task OpenFileStreamAsync(string pathToSave, FileDto uploadedFile, string extension,
            IFormFile file)
        {
            var combinedPath = Path.Combine(pathToSave, uploadedFile.Id + extension);
            await using var stream = new FileStream(combinedPath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        #endregion
    }
}
