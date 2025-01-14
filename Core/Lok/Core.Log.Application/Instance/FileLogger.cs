using Core.Log.Application.Contracts.Dto;
using Core.Shared.Responses;
using Microsoft.Extensions.Options;
using ILogger = Core.Log.Application.Contracts.IAppServices.ILogger;


namespace Core.Log.Application.Instance
{
    public class FileLogger : ILogger
    {
        public string CurrentFileName { get; set; }
        public string settingPath { get; set; }
        private readonly FileLoggerOptions options;
        public FileLogger(IOptions<FileLoggerOptions> options)
        {
            //this.options = options.Value;
            //settingPath = Path.Combine(Directory.GetCurrentDirectory()) + this.options.Path;
            //Intilaization();
        }

        #region Intilaization Functions
        private void Intilaization()
        {
            //GetCurrentFile();
            //CreateLogDirectory();
            //CreateLogFile();
        }
        private ILogger CreateLogFile()
        {
            if (string.IsNullOrEmpty(CurrentFileName))
            {
                //CreateNewFile();
            }
            return this;
        }
        private void CreateLogDirectory()
        {
            if (!Directory.Exists(settingPath))
            {
                //Directory.CreateDirectory(settingPath);
            }
        }
        private void CheckFileSize()
        {
            try
            {
                //var fullFilePath = settingPath + "\\" + CurrentFileName;
                //var length = new FileInfo(fullFilePath).Length / 1024 / 1024;
                //int MaxSize = options.MaxSize;
                //if (length > MaxSize)
                //{
                //    // CreateNewFile();     
                //}

            }
            catch (Exception)
            {

                throw;
            }


        }
        private void CreateNewFile()
        {
            try
            {

                //CurrentFileName = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss") + ".log";
                //var fullFilePath = settingPath + CurrentFileName;
                //var MyFile = File.Create(fullFilePath);
                //MyFile.Dispose();
                //MyFile.Close();
            }
            catch (Exception)
            {
                //TODO: Exception 
                throw;
            }

        }
        public void GetCurrentFile()
        {
            //CreateLogDirectory();
            //if (Directory.GetFiles(settingPath, "*.log", SearchOption.AllDirectories).Count() != 0)
            //{
            //    string[] fileArray = Directory.GetFiles(settingPath, "*.log", SearchOption.AllDirectories);
            //    var Files = new List<string>();
            //    foreach (var item in fileArray)
            //    {
            //        Files.Add(Path.GetFileName(item));
            //    }
            //    Files = Files.OrderByDescending(filename => filename).ToList();
            //    CurrentFileName = Files.FirstOrDefault();
            //}

        }
        private string GetCurrentFilePath()
        {
            return null;
            //try
            //{
            //    return settingPath + CurrentFileName;
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
        }
        #endregion

        #region Business Functions

        public async Task<BaseResponse> Log(LogDto logDto)
        {
            //try
            //{

            //    CheckFileSize();
            //    using (var streamWriter = new StreamWriter(GetCurrentFilePath(), true))
            //    {
            //        streamWriter.WriteLine(logDto.Format());
            //    }
            //}
            //catch (Exception exc)
            //{
            //    return new ExceptionResponse()
            //    {
            //        IsDone = true,
            //        Exception = exc.ToString(),
            //        Message = exc.Message
            //    };
            //}
            return new BaseResponse()
            {
                IsDone = true
            };
        }

        #endregion
    }
}


