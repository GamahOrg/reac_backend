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

        private static readonly object _lock = new object();

        public async Task<BaseResponse> Log(LogDto logDto)
        {
            try
            {
                // Ensure the Logs directory exists
                string logsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(logsDirectory))
                {
                    Directory.CreateDirectory(logsDirectory);
                }

                // Create or append to the log file with today's timestamp
                string logFileName = $"{DateTime.UtcNow:yyyy-MM-dd}.log";
                string logFilePath = Path.Combine(logsDirectory, logFileName);

                lock (_lock) // Synchronize access to the file
                {
                    // Check if the file exists, create it if it doesn't
                    if (!File.Exists(logFilePath))
                    {
                        using (var fileStream = File.Create(logFilePath))
                        {
                            fileStream.Close();
                        }
                    }

                    // Write the log data to the file
                    using (var streamWriter = new StreamWriter(logFilePath, true))
                    {
                        streamWriter.WriteLineAsync("\n" + logDto.Format());
                    }
                }

                return new BaseResponse()
                {
                    IsDone = true
                };
            }
            catch (Exception exc)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = exc.ToString(),
                    Message = exc.Message
                };
            }
        }

        #endregion
    }
}


