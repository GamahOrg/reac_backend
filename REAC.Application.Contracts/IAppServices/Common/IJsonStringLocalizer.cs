using Core.Shared.Responses;
using Microsoft.Extensions.Localization;
using REAC.Application.Contracts.Dtos.Common;

namespace REAC.Application.Contracts.IAppServices.Common
{
    public interface IJsonStringLocalizer : IStringLocalizer
    {
        /// <summary>
        /// Get json string file for specific language
        /// </summary>
        /// <param name="Language"></param>
        /// <returns></returns>
        BaseResponse GetAllJsonStrings(string Language);
        /// <summary>
        /// update json string file for specific language
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        BaseResponse UpdateAllJsonStrings(UpdateJsonLocalizer input);
    }
}
