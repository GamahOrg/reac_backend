using Core.Shared.Responses;


namespace Bpf.Application.ModuleServices
{
    public partial class BpfService
    {
        public async Task<BaseResponse> GetRequestLogForEmployee(Guid requestId, CancellationToken cancellationToken)
        {
            try
            {
                return await GetRequestLog(requestId, false, cancellationToken);
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString(),
                };
            }
        }

    }
}
