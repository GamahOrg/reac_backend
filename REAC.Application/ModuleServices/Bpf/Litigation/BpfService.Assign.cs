using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Wpf.Business;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace Bpf.Application.ModuleServices
{
    public partial class BpfService
    {
        public async Task<BaseResponse> Assign(AssignRequestPayload assignRequestPayload, CancellationToken cancellationToken)
        {
            try
            {
                Guid employeeId = Guid.Empty;
                _jwtReader.GetReferenceId(ref employeeId);

                var employee = (await _repository.GetQueryable<SysEmployee, Guid>())?
                        .Where(x => x.Id == employeeId).Single();

                if (employee == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "موظف غير معروف"
                    };

                if (!employee.AllowAssign)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "موظف غير مسموح له"
                    };

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                       .Where(x => x.Id == assignRequestPayload.RequestId).Single();

                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };

                if (litigationRequest.ReformerCitizenId.HasValue)
                {
                    if (!employee.AllowReAssign)
                        return new BaseResponse()
                        {
                            IsDone = false,
                            Message = "موظف غير مسموح له"
                        };
                }

                LitigationRequestReformerAssigningRequest litigationRequestReformerAssigningRequest = new LitigationRequestReformerAssigningRequest();
                litigationRequestReformerAssigningRequest.CitizenId = assignRequestPayload.ReformerId;
                litigationRequestReformerAssigningRequest.RequestOn = DateTime.Now;
                litigationRequestReformerAssigningRequest.LitigationRequestId = litigationRequest.Id;
                litigationRequestReformerAssigningRequest.Response = LitigationRequestReformerAssigningRequestStatus.None;
                await _repository.AddAsync<LitigationRequestReformerAssigningRequest, Guid>(litigationRequestReformerAssigningRequest);

                var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                return repositorySaveResponse;
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    Exception = ex.Message + "\n" + ex.ToString(),
                };
            }
        }
    }
}
