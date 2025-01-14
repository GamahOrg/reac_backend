using Core.Shared.Responses;


namespace Bpf.Application.ModuleServices
{
    public partial class BpfService
    {
        public async Task<BaseResponse> GetRequestLogForApplicant(Guid requestId, CancellationToken cancellationToken)
        {
            try
            {
                return await GetRequestLog(requestId, true, cancellationToken);
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
