using Core.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REAC.Application.Contracts.Dtos.Zoom;

namespace REAC.Application.Contracts.IAppServices.UserManage
{
    public interface IZoomService
    {
        Task<ZoomMeting?> GenerateMeeting(string meetingTopic);
        Task<ZoomMeting?> GenerateMeeting(string meetingTopic,DateTime meetingStartTime);
      
        Task<BaseResponse> GetMeetingById(long MeetingId);

        Task<List<ZoomMeetingParticipantsDetails?>> GetMeetingParticipantsById(long MeetingId);
    }
}
