using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Common;
using Core.GenericRepository.Shared.BaseEntities;
using Core.Localizations;
using Core.Log.Application.Contracts.Dto;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts;
using REAC.Application.Contracts.Dtos.Integration.Rent;
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
using System.Runtime.CompilerServices;

namespace REAC.Application.AppServices.System
{
    public class SysFileManagerService : BasicService<SysFileManager, Guid>, ISysFileManagerService
    {
        public readonly ILoggerManager _loggerManager;
        public SysFileManagerService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {
            _loggerManager = loggerManager;
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

        public async Task<DataResponse<List<FileDto>>> UploadAsync(IFormFileCollection files)
        {
            try
            {
                if (files == null)
                    throw new ArgumentNullException(nameof(files));

                var allowedExtensions = new List<string> { ".jpg", ".png", ".pdf", ".docx", ".xlsx", ".xls", ".jpeg" };
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
                        _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database, new LogDto()
                        {
                            Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                            Message = Localizations.ThisFileTypeNotAllowed.ToString()
                        });
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
                        return new DataResponse<List<FileDto>> { IsDone = false, Message = Localizations.FailedUploadFile, Data = null };


                    var id = Guid.NewGuid();
                    var path = Path.Combine(SharedConstants.RootFolder, id + extension);
                    path = path.Replace(@"\", @"/");
                    var uploadedFile = new FileDto(fileName, id, path, "");
                    await CreateNewFile(uploadedFile, extension, file);
                    // Check if file was saved successfully
                    var saveResult = await OpenFileStreamAsync(pathToSave, uploadedFile, extension, file, _loggerManager);
                    if (!saveResult)
                    {
                        return new DataResponse<List<FileDto>> { IsDone = false, Message = Localizations.FailedUploadFile, Data = null };
                    }

                    uploadedFiles.Add(uploadedFile);
                }

                await _repository.SaveChangesAsync();
                return new DataResponse<List<FileDto>> { IsDone = true, Message = Localizations.UploadSuccefully, Data = uploadedFiles };
            }
            catch (Exception ex)
            {
                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database, new LogDto()
                {
                    Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                    Message = "upload file exception=>" + ex.ToString()
                });
                return new DataResponse<List<FileDto>> { IsDone = false, Message = Localizations.FailedUploadFile, Data = null };
            }
        }

        private static string CreateUploadRootFolder()
        {
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), SharedConstants.RootFolder);
            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(pathToSave);
            return pathToSave;
        }


        #region Private Methods
        private bool IsFileSaved(string filePath)
        {
            return File.Exists(filePath);
        }

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

        //private static async Task OpenFileStreamAsync(string pathToSave, FileDto uploadedFile, string extension,
        //    IFormFile file)
        //{
        //    var combinedPath = Path.Combine(pathToSave, uploadedFile.Id + extension);
        //    await using var stream = new FileStream(combinedPath, FileMode.Create);
        //    await file.CopyToAsync(stream);
        //}

        // Modified OpenFileStreamAsync method
        private static async Task<bool> OpenFileStreamAsync(string pathToSave, FileDto uploadedFile, string extension, IFormFile file, ILoggerManager loggerManager)
        {
            var combinedPath = Path.Combine(pathToSave, uploadedFile.Id + extension);

            try
            {
                await using var stream = new FileStream(combinedPath, FileMode.Create);
                await file.CopyToAsync(stream);

                // Ensure stream is properly closed before checking  
                await stream.FlushAsync();
            }
            catch (Exception ex)
            {
                // Log error and return false  
                loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database, new LogDto()
                {
                    Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                    Message = "upload file exception=>" + ex.ToString()
                });
                return false;
            }

            // Check if file was saved after stream is closed  
            return IsFileSavedSuccessfully(combinedPath, file.Length, loggerManager);
        }

        private static bool IsFileSavedSuccessfully(string filePath, long expectedSize, ILoggerManager loggerManager)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                var fileInfo = new FileInfo(filePath);
                return fileInfo.Length == expectedSize && fileInfo.Length > 0;
            }
            catch (Exception ex)
            {
                loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database, new LogDto()
                {
                    Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                    Message = "upload file exception=>" + ex.ToString()
                });
                return false;
            }
        }

        #endregion

        #region Complaints
        public async Task<DataResponse<FileDto>> UploadComplaintFileAsync(string base64File, string fileName)
        {
            // The DataResponse signature is changed to return a single FileDto, 
            // as we are now processing a single file instead of a collection.

            try
            {
                if (string.IsNullOrEmpty(base64File))
                    throw new ArgumentNullException(nameof(base64File));

                if (string.IsNullOrEmpty(fileName))
                    throw new ArgumentNullException(nameof(fileName));

                // 1. Convert Base64 string to a byte array and get the file extension
                (byte[] fileBytes, string extension) = ConvertBase64ToBytesAndGetExtension(base64File, fileName);

                if (fileBytes.Length == 0)
                {
                    // Handle case where base64 might decode to an empty file
                    return new DataResponse<FileDto> { IsDone = false, Message = "File content is empty.", Data = null };
                }

                var pathToSave = CreateUploadComplaintFilesRootFolder();

                // 2. Prepare File DTO and Path
                var id = Guid.NewGuid();
                var path = Path.Combine(SharedConstants.ComplaintFilesFolder, id + extension);
                path = path.Replace(@"\", @"/");

                var uploadedFile = new FileDto(fileName, id, path, "");

                // 3. Create file entry (assuming this method interacts with the database)
                await CreateNewComplaintFile(uploadedFile, extension); 

                // 4. Save the file content to disk
                var saveResult = await OpenFileStreamAsync(pathToSave, uploadedFile, fileBytes, _loggerManager);

                if (!saveResult)
                {
                    // Note: In a robust system, you might want to delete the DB entry created by CreateNewFile here.
                    return new DataResponse<FileDto> { IsDone = false, Message = Localizations.FailedUploadFile, Data = null };
                }

                await _repository.SaveChangesAsync();
                return new DataResponse<FileDto> { IsDone = true, Message = Localizations.UploadSuccefully, Data = uploadedFile };
            }
            catch (FormatException)
            {
                // Specifically catch FormatException if the base64 string is invalid
                return new DataResponse<FileDto> { IsDone = false, Message = "Invalid Base64 file format.", Data = null };
            }
            catch (Exception ex)
            {
                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database, new LogDto()
                {
                    Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                    Message = "upload file exception=>" + ex.ToString()
                });
                return new DataResponse<FileDto> { IsDone = false, Message = Localizations.FailedUploadFile, Data = null };
            }
        }

        private static string CreateUploadComplaintFilesRootFolder()
        {
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), SharedConstants.ComplaintFilesFolder);
            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(pathToSave);
            return pathToSave;
        }
        private (byte[] FileBytes, string Extension) ConvertBase64ToBytesAndGetExtension(string base64File, string fileName)
        {
            // Clean up potential Data URI prefix (e.g., "data:image/png;base64,")
            var base64Data = base64File;
            if (base64File.Contains(","))
            {
                base64Data = base64File.Split(',').Last();
            }

            // Decode the Base64 string into a byte array
            byte[] fileBytes = Convert.FromBase64String(base64Data);

            // Get the file extension
            var extension = Path.GetExtension(fileName);

            return (fileBytes, extension);
        }


        private async Task<bool> OpenFileStreamAsync(
    string pathToSave,
    FileDto uploadedFile,
    byte[] fileBytes,
    ILoggerManager loggerManager)
        {
            var fullPath = Path.Combine(pathToSave, uploadedFile.Id + Path.GetExtension(uploadedFile.FileName));

            try
            {
                // Write the byte array to the file path asynchronously
                await File.WriteAllBytesAsync(fullPath, fileBytes);
                return true;
            }
            catch (Exception ex)
            {
                loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database, new LogDto()
                {
                    Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                    Message = $"Error saving file {fullPath} => {ex.ToString()}"
                });
                return false;
            }
        }

        private async Task CreateNewComplaintFile(FileDto uploadedFile, string extension)
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

        #endregion
    }
}
