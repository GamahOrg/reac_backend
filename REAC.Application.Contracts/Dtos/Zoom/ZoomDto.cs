using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using System.Text.Json.Serialization;

namespace REAC.Application.Contracts.Dtos.Zoom;

public class ZoomAccessToken : BaseEntity<Guid>
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
    public string scope { get; set; }
}

public class ZoomMeting : BaseEntity<Guid>
{
    //URL to start the meeting. This URL should only be used by the host of the meeting and should not be shared with anyone other than the host of the meeting, since anyone with this URL will be able to log in to the Zoom Client as the host of the meeting.
    public string start_url { get; set; }
    public int type { get; set; }
    public string uuid { get; set; }

    public long id { get; set; }
    public string host_id { get; set; }
    public string host_email { get; set; }
    public string topic { get; set; }
    public string status { get; set; }
    public string start_time { get; set; }
    public int duration { get; set; }
    public string created_at { get; set; }
    //URL for participants to join the meeting. This URL should only be shared with users that you would like to invite for the meeting.
    public string join_url { get; set; }

}

public class ZoomMeetingDetails : BaseEntity<Guid>
{
    //public long id { get; set; }
    public string? source { get; set; }
    public string start_time { get; set; }
    public int total_minutes { get; set; }
    public string end_time { get; set; }
    public int duration { get; set; }
    public int participants_count { get; set; }
    public List<ZoomMeetingParticipantsDetails> Participants { get; set; }


}

public class ZoomMeetingParticipantsDetails : BaseEntity<Guid>
{
    public string name { get; set; }
    public int total_minutes { get; set; }
    public string join_time { get; set; }
    public string leave_time { get; set; }
  //  public string user_email { get; set; }

}