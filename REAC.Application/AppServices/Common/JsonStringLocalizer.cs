using Core.Shared.Responses;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using REAC.Application.Contracts.Dtos.Common;
using REAC.Application.Contracts.IAppServices.Common;

namespace REAC.Application.AppServices.Common
{
    public class JsonStringLocalizer : IJsonStringLocalizer
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializer _serializer = new();
        private const string LocalizationPath = "Resources/Localization/";
        public JsonStringLocalizer(IDistributedCache cache)
        {
            _cache = cache;
        }
        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value);
            }
        }
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var actualValue = this[name];
                return !actualValue.ResourceNotFound
                    ? new LocalizedString(name, string.Format(actualValue.Value, arguments))
                    : actualValue;
            }
        }



        /// <summary>
        /// Get All String Resources Built in function
        /// </summary>
        /// <param name="includeParentCultures"></param>
        /// <returns></returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var filePath = $"{LocalizationPath}{Thread.CurrentThread.CurrentCulture.Name}.json";

            using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using StreamReader streamReader = new(stream);
            using JsonTextReader reader = new(streamReader);

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    continue;

                var key = reader.Value as string;
                reader.Read();
                var value = _serializer.Deserialize<string>(reader);
                yield return new LocalizedString(key, value);
            }
        }
        /// <summary>
        /// Get json string file for specific language
        /// </summary>
        /// <param name="Language"></param>
        /// <returns></returns>
        public BaseResponse GetAllJsonStrings(string Language)
        {
            var filePath = $"{LocalizationPath}{Language}.json";

            if (!File.Exists(filePath))
                return new BaseResponse { IsDone = false, Message = $"{this["GeneralNotFoundPath", filePath]}" };

            string jsonString = File.ReadAllText(filePath);

            return new DataResponse<string> { IsDone = true, Data = jsonString };
        }
        /// <summary>
        /// update json string file for specific language
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public BaseResponse UpdateAllJsonStrings(UpdateJsonLocalizer input)
        {
            var filePath = $"{LocalizationPath}{input.Language}.json";
            if (!File.Exists(filePath))
                return new BaseResponse { IsDone = false, Message = $"{this["GeneralNotFoundPath", filePath]}" };

            dynamic jsonObj = JsonConvert.DeserializeObject(input.JSON);
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);

            File.WriteAllText(filePath, output);
            //TODO remove all caches

            return new BaseResponse { IsDone = true };
        }

        #region Private Methods
        /// <summary>
        /// getting string value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetString(string key)
        {
            // var filePath = $"{LocalizationPath}{Thread.CurrentThread.CurrentCulture.Name}.json";

            var filePath = $"{LocalizationPath}ar-EG.json";
            var fullFilePath = Path.GetFullPath(filePath);

            if (File.Exists(fullFilePath))
            {
                var cacheKey = $"locale_{Thread.CurrentThread.CurrentCulture.Name}_{key}";
                var cacheValue = _cache.GetString(cacheKey);

                if (!string.IsNullOrEmpty(cacheValue))
                    return cacheValue;

                var result = GetValueFromJSON(key, fullFilePath);

                if (!string.IsNullOrEmpty(result))
                    _cache.SetString(cacheKey, result);

                return string.IsNullOrEmpty(result) ? key : result;
            }

            return key;
        }
        /// <summary>
        /// Read properitis from json 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetValueFromJSON(string propertyName, string filePath)
        {
            if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(filePath))
                return string.Empty;

            using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using StreamReader streamReader = new(stream);
            using JsonTextReader reader = new(streamReader);

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName && reader.Value as string == propertyName)
                {
                    reader.Read();
                    return _serializer.Deserialize<string>(reader);
                }
            }

            return string.Empty;
        }
        #endregion
    }
}