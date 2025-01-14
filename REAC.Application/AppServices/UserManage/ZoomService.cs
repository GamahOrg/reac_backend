using Core.GenericRepository.BaseRepository;
using Core.Shared.Responses;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.IAppServices.Common;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.Contracts.IAppServices.UserManage;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using REAC.Application.Contracts.Dtos.Zoom;

namespace REAC.Application.AppServices.UserManage
{
    public class ZoomService : IZoomService
    {
        private readonly ISysSettingService _settingService;

        public ZoomService(ISysSettingService settingService)
        {
            _settingService = settingService;
        }
        private string clientID;
        private string clientSecret;
        private string accountID;
        private int meetingDuration;
        private string meetingDefaultStartTime = DateTime.UtcNow.AddMinutes(10).ToString("yyyy-MM-ddTHH:mm:ssZ");//to be el litigiation date time

        //public string ClientID
        //{
        //    get => clientID; 
        //    set
        //    {
        //        clientID = _settingService.GetBySettingKey(SysSettingKeyConstants.Zoom_ClientID).Result.Data?.Value;
        //    }
        //}
        //public string ClientSecret { get => clientSecret; set => clientSecret = _settingService.GetBySettingKey(SysSettingKeyConstants.Zoom_ClientSecret).Result.Data?.Value; }
        //public string AccountId { get => accountID; set => accountID = _settingService.GetBySettingKey(SysSettingKeyConstants.Zoom_AccountID).Result.Data?.Value; }
        //public int MeetingDuration { get => meetingDuration; set => meetingDuration = int.Parse(_settingService.GetBySettingKey(SysSettingKeyConstants.Zoom_MeetingDuration).Result.Data?.Value); }

        public async Task<ZoomMeting?> GenerateMeeting(string meetingTopic)
        {
            meetingDuration = int.Parse(_settingService.GetBySettingKey(SysSettingKeyConstants.Zoom_MeetingDuration).Result.Data?.Value);
            string accessToken = await GetAccessToken();

            HttpClient httpclient = new HttpClient();
            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //create Zoom Meeting
            string apiURL = "https://api.zoom.us/v2/users/me/meetings";
            httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var requestBody = new
            {
                topic = meetingTopic,
                type = 2,//scheduled meeting
                //1 - An instant meeting.
                //2 - A scheduled meeting.
                //3 - A recurring meeting with no fixed time.
                //8 - A recurring meeting with fixed time.
                //10 - A screen share only meeting.

                start_time = meetingDefaultStartTime,
                duration = meetingDuration,
                timezone = "UTC",
                settings = new
                {
                    host_video = true,
                    participant_video = true,
                    auto_recording = "cloud",
                    jbh_time = 10,//If the value of the join_before_host field is true, this field indicates the time limits when a participant can join a meeting before the meeting's host.
                    join_before_host = true//Whether participants can join the meeting before its host
                }
            };

            var requestContect = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await httpclient.PostAsync(apiURL, requestContect);


            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var _zoomMeeting = JsonSerializer.Deserialize<ZoomMeting>(responseContent);
                //Console.WriteLine(_zoomMeeting?.start_url);
                return _zoomMeeting;
                //return _zoomMeeting != null ? $"{_zoomMeeting.join_url}, {_zoomMeeting.id}" : ""; ;
            }
            else
            {
                return null;
            }


        }
        
        public async Task<ZoomMeting?> GenerateMeeting(string meetingTopic,DateTime meetingStartTime)
        {
            meetingDuration = int.Parse(_settingService.GetBySettingKey(SysSettingKeyConstants.Zoom_MeetingDuration).Result.Data?.Value);
            string accessToken = await GetAccessToken();

            HttpClient httpclient = new HttpClient();
            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //create Zoom Meeting
            string apiURL = "https://api.zoom.us/v2/users/me/meetings";
            httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var requestBody = new
            {
                topic = meetingTopic,
                type = 2,//scheduled meeting
                //1 - An instant meeting.
                //2 - A scheduled meeting.
                //3 - A recurring meeting with no fixed time.
                //8 - A recurring meeting with fixed time.
                //10 - A screen share only meeting.

                start_time = meetingStartTime,
                duration = meetingDuration,
                timezone = "UTC",
                settings = new
                {
                    host_video = true,
                    participant_video = true,
                    auto_recording = "cloud",
                    jbh_time = 10,//If the value of the join_before_host field is true, this field indicates the time limits when a participant can join a meeting before the meeting's host.
                    join_before_host = true//Whether participants can join the meeting before its host
                }
            };

            var requestContect = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await httpclient.PostAsync(apiURL, requestContect);


            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var _zoomMeeting = JsonSerializer.Deserialize<ZoomMeting>(responseContent);
                //Console.WriteLine(_zoomMeeting?.start_url);
                return _zoomMeeting;
                //return _zoomMeeting != null ? $"{_zoomMeeting.join_url}, {_zoomMeeting.id}" : ""; ;
            }
            else
            {
                return null;
            }


        }

        private async Task<string> GetAccessToken()
        {
            clientID = _settingService.GetBySettingKey(SysSettingKeyConstants.Zoom_ClientID).Result.Data?.Value;
            accountID = _settingService.GetBySettingKey(SysSettingKeyConstants.Zoom_AccountID).Result.Data?.Value;
            clientSecret = _settingService.GetBySettingKey(SysSettingKeyConstants.Zoom_ClientSecret).Result.Data?.Value;

            var client = new HttpClient();
            client.BaseAddress = new Uri("https://zoom.us/oauth/token");
            var request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientID}:{clientSecret}")));
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>{

                { "grant_type", "account_credentials" },

                { "account_id", accountID }
            });
            request.Headers.Host = "zoom.us";

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var token = JsonSerializer.Deserialize<ZoomAccessToken>(content);
                return token != null ? token.access_token : "";
            }
            else
            {
                // Handle the error
                Console.WriteLine($"Failed to retrieve access token: {response.ReasonPhrase}");
                return "";
            }
        }


        public async Task<BaseResponse> GetMeetingById(long id)
        {
            string accessToken = await GetAccessToken();

            HttpClient httpclient = new HttpClient();
            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //get Zoom Meeting
            string apiURL = "https://api.zoom.us/v2/past_meetings/" + id.ToString();
            httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


            var response = await httpclient.GetAsync(apiURL);


            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var _zoomMeeting = JsonSerializer.Deserialize<ZoomMeetingDetails>(responseContent);

               
                var participants = await GetMeetingParticipantsById(id);
                _zoomMeeting.Participants = participants; 

                
                return new DataResponse<ZoomMeetingDetails>() { Data = _zoomMeeting , IsDone = true };
            }
            else
            {
                return new BaseResponse() {  IsDone = true , Message = "خطأ داخلي"};
            }


        }

        public async Task<List<ZoomMeetingParticipantsDetails?>> GetMeetingParticipantsById(long id)
        {
            string accessToken = await GetAccessToken();

            HttpClient httpclient = new HttpClient();
            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //get Zoom Meeting Participants
            string apiURL = "https://api.zoom.us/v2/past_meetings/" + id.ToString() + "/participants";
            httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


            var response = await httpclient.GetAsync(apiURL);
            if (!response.IsSuccessStatusCode)
            {
                return new List<ZoomMeetingParticipantsDetails>(); 
            }
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(jsonResponse))
            {
                return new List<ZoomMeetingParticipantsDetails>(); 
            }

            
            var jsonDocument = JsonDocument.Parse(jsonResponse);

            
            if (jsonDocument.RootElement.ValueKind == JsonValueKind.Array)
            {
                
                return JsonSerializer.Deserialize<List<ZoomMeetingParticipantsDetails>>(jsonResponse);
            }
            else if (jsonDocument.RootElement.ValueKind == JsonValueKind.Object &&
                     jsonDocument.RootElement.TryGetProperty("participants", out var participantsElement))
            {
                // Extract the 'participants' array if it's wrapped in an object
                return JsonSerializer.Deserialize<List<ZoomMeetingParticipantsDetails>>(participantsElement.GetRawText());
            }

            
            return new List<ZoomMeetingParticipantsDetails>();
           

        }
    }

   
}