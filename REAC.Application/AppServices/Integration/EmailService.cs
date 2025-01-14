using Core.Contracts;
using Core.Dtos;
using Core.GenericRepository.BaseRepository;
using Core.Shared.Responses;

namespace REAC.Application.AppServices.Integration;

public class EmailService : IMailService
{
    private readonly IEntityRepository _repository;
    public EmailService(IEntityRepository entityRepository)
    {
        _repository = entityRepository;
    }
    public async Task<BaseResponse> SendMail(List<string> toAddressList, string subject, string body)
    {
        BaseResponse response = new BaseResponse();

        return response;
    }

    async Task<SendEmailOptions?> GetEmailSettings()
    {
        SendEmailOptions? sendEmail = null;

        return sendEmail;
    }
}
