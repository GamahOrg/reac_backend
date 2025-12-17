using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.Dtos.Integration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Shared.Responses;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using static iText.IO.Util.IntHashtable;
using Core.Log.Application.Contracts.IAppServices;
using Core.Log.Application.Contracts.Dto;
using Core.GenericRepository.BaseRepository;
using REAC.EntityFrameworkCore.Entities.Integration;
using REAC.Application.Contracts.IAppServices;
using REAC.EntityFrameworkCore.Entities.Actor;
using AutoMapper;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.Application.AppServices.Integration.GEE
{
    public class ComplaintsService : IComplaintsService
    {
        private readonly HttpClient _httpClient;
        private readonly ISysSettingService _settingService;
        private readonly IGEELoginService _gEELoginService;
        private readonly ISysFileManagerService _sysFileManagerService;
        private readonly ILoggerManager _loggerManager;
        private IEntityRepository _repository;
        private readonly IJwtReader _jwtReader;
        private IMapper _mapper;


        public ComplaintsService(ISysSettingService settingService, IGEELoginService gEELoginService, ISysFileManagerService sysFileManagerService, ILoggerManager loggerManager, IEntityRepository repository, IJwtReader jwtReader, IMapper mapper)
        {
            _httpClient = new HttpClient();
            _settingService = settingService;
            _gEELoginService = gEELoginService;
            _sysFileManagerService = sysFileManagerService;
            _loggerManager = loggerManager;
            _repository = repository;
            _jwtReader = jwtReader;
            _mapper = mapper;
        }

        public async Task<DataResponse<RegionListResponseDto>> GetRegionListAsync()
        {
            var url = _settingService.GetBySettingKey(SysSettingKeyConstants.GetRegionList).Result.Data?.Value;
            var token = (await _gEELoginService.Login()).Data?.Access_token;

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(url))
                return new DataResponse<RegionListResponseDto>(null, false, "حدث خطأ اثناء التكامل");

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new DataResponse<RegionListResponseDto>(null, false, "حدث خطأ اثناء التكامل مع المناطق");

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<RegionListResponseDto>(json);

            if (data == null || data.module_fields.rejon_c.options.Count == 0)
                return new DataResponse<RegionListResponseDto>(null, false, "خطأ داخلي");

            // Test Data
            //var data = new RegionListResponseDto()
            //{
            //    link_fields = new List<object>(),
            //    table_name = "cases",
            //    module_name = "Cases",
            //    module_fields = new ModuleFields()
            //    {
            //        rejon_c = new RejonC()
            //        {
            //            calculated = false,
            //            group = "Rejon",
            //            id_name = "rejon_c",
            //            label = "المنطقة",
            //            len = 100,
            //            name = "rejon_c",
            //            options = new Dictionary<string, RegionOption>()
            //            {
            //                { "1", new RegionOption() { name = "الرياض", value = "الرياض" } },
            //                { "2", new RegionOption() { name = "مكة المكرمة", value = "مكة المكرمة" } },
            //                { "3", new RegionOption() { name = "المدينة المنورة", value = "المدينة المنورة" } },
            //                { "4", new RegionOption() { name = "الشرقية", value = "الشرقية" } },
            //                { "5", new RegionOption() { name = "القصيم", value = "القصيم" } },
            //                { "6", new RegionOption() { name = "تبوك", value = "تبوك" } },
            //                { "7", new RegionOption() { name = "حائل", value = "حائل" } },
            //                { "8", new RegionOption() { name = "عسير", value = "عسير" } },
            //                { "9", new RegionOption() { name = "جازان", value = "جازان" } },
            //                { "10", new RegionOption() { name = "نجران", value = "نجران" } },
            //                { "11", new RegionOption() { name = "الباحة", value = "الباحة" } },
            //                { "12", new RegionOption() { name = "الجوف", value = "الجوف" } },
            //                { "13", new RegionOption() { name = "الحدود الشمالية", value = "الحدود الشمالية" } }
            //            },
            //            related_module = "",
            //            required = 0,
            //            type = "enum"
            //        }
            //    }
            //};


            return new DataResponse<RegionListResponseDto>(data, true, "Success");

        }

        public async Task<DataResponse<ContactResponseDto>> GetContactAsync(string nationalId)
        {
            try
            {
                var url = _settingService.GetBySettingKey(SysSettingKeyConstants.GetContact).Result.Data?.Value;
                url = url.Replace("{nationalId}", nationalId);
                var token = (await _gEELoginService.Login()).Data?.Access_token;

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(url))
                    return new DataResponse<ContactResponseDto>(null, false, "حدث خطأ اثناء التكامل");

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return new DataResponse<ContactResponseDto>(null, false, "حدث خطأ اثناء التكامل مع بيانات المستخدم");

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ContactResponseDto>(json);



                if (data == null || data.total_count == 0)
                    return new DataResponse<ContactResponseDto>(null, true, "");


                // Test Data
                //var entryList = new List<ContactEntry>();
                //var entry = new ContactEntry()
                //{
                //    id = "1",
                //    module_name = "Contacts",
                //    name_value_list = new NameValueList()
                //    {
                //        id = new NameValue() { name = "id", value = "1" },
                //        first_name = new NameValue() { name = "first_name", value = "John" },
                //        last_name = new NameValue() { name = "last_name", value = "Doe" },
                //        email1 = new NameValue() { name = "email1", value = "a@a.com" },
                //        national_id_c = new NameValue() { name = "national_id_c", value = "1234567890" },
                //        phone_mobile = new NameValue() { name = "phone_mobile", value = "0501234567" }
                //    }
                //};
                //entryList.Add(entry);

                //var data = new ContactResponseDto()
                //{
                //    entry_list = entryList,
                //    result_count = 0,
                //    next_offset = 0,
                //    total_count = 0,
                //    relationship_list = new List<object>()
                //};

                return new DataResponse<ContactResponseDto>(data, true, "تم اضافة الشكوي بنجاح");
            }
            catch (Exception e)
            {
                return new DataResponse<ContactResponseDto>(null, false, "خطأ داخلي");

            }


        }

        public async Task<BaseResponse> UploadComplaintFileAsync(UploadComplaintFileNoteDto input)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);


            var token = (await _gEELoginService.Login()).Data?.Access_token;

            //Step 1: Create Note
            var createNoteUrl = _settingService.GetBySettingKey(SysSettingKeyConstants.CreateNewNote).Result.Data?.Value;

            var noteRequestBody = new
            {
                module_name = "Notes",
                name_value_list = new
                {
                    name = input.filename
                }
            };

            var noteRequest = new HttpRequestMessage(HttpMethod.Post, createNoteUrl)
            {
                Content = JsonContent.Create(noteRequestBody)
            };
            noteRequest.Headers.Add("Authorization", $"Bearer {token}");

            var noteResponse = await _httpClient.SendAsync(noteRequest);
            if (!noteResponse.IsSuccessStatusCode)
                return new BaseResponse(false, "خطأ أثناء إنشاء الملاحظة");

            var noteJson = await noteResponse.Content.ReadAsStringAsync();
            var noteData = JsonSerializer.Deserialize<CreateNoteResponseDto>(noteJson);
            if (noteData == null || string.IsNullOrEmpty(noteData.id))
                return new BaseResponse(false, "لم يتم الحصول على رقم الملاحظة");

            // Step 2: Upload File
            var uploadFileUrl = _settingService.GetBySettingKey(SysSettingKeyConstants.UploadFileToNote).Result.Data?.Value;
            input.id = noteData.id;
            UploadComplaintFileRequestDto complaintFileNoteDto = new UploadComplaintFileRequestDto();
            complaintFileNoteDto.note = input;
            var uploadRequest = new HttpRequestMessage(HttpMethod.Post, uploadFileUrl)
            {
                Content = JsonContent.Create(complaintFileNoteDto)
            };
            uploadRequest.Headers.Add("Authorization", $"Bearer {token}");

            var uploadResponse = await _httpClient.SendAsync(uploadRequest);
            if (!uploadResponse.IsSuccessStatusCode)
                return new BaseResponse(false, "خطأ أثناء رفع الملف");


            // Step 3: Save File In Databsae
            var fileSaveResponse = await _sysFileManagerService.UploadComplaintFileAsync(input.file, input.filename);

            if (!fileSaveResponse.IsDone)
                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                      , new LogDto()
                      {
                          Type = Core.Log.Domain.Shared.Enums.LogType.Error,
                          Message = $"Error Saving Complaints File {input.filename} in SysFileManager Table , Errors ===> {fileSaveResponse.Message}",
                      });

            if (fileSaveResponse.IsDone && fileSaveResponse is not null && fileSaveResponse.Data is not null)
            {

                var saveComplaintsFile = await _repository.AddAsync<ComplaintFile, Guid>(new ComplaintFile()
                {
                    FileManagerId = fileSaveResponse.Data.Id,
                    IsDeleted = false,
                    CreatedBy = referenceId,
                    CreatedOn = DateTime.Now,
                    //NoteId = Guid.Parse(noteData.id),
                    NoteId = Guid.NewGuid(),
                });

                var saveFile = await _repository.SaveChangesAsync();

                if (!saveFile.IsDone)
                    _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                          , new LogDto()
                          {
                              Type = Core.Log.Domain.Shared.Enums.LogType.Error,
                              Message = $"Error Saving Complaints File {input.filename} in ComplaintFile Table",
                          });
            }

            var result = new UploadComplaintFileNoteResponseDto() { filename = input.filename, noteId = noteData.id  };

            // Test data
            //var resultTest = new UploadComplaintFileNoteResponseDto() { filename = "fileName.pdf", noteId = "be5d0956-e367-422d-ae98-8e53ad400ad6" };

            return new DataResponse<UploadComplaintFileNoteResponseDto>(result, true, "تم رفع الملف بنجاح");
        }

        public async Task<BaseResponse> CreateComplaintAsync(CreateComplaintRequestDto request)
        {
            var token = (await _gEELoginService.Login()).Data?.Access_token;

            if (token is null)
                return new BaseResponse(false, "حدث خطأ اثناء التكامل");

            // Step 1: Create Contact if contact_id is null
            string contactId = string.Empty;
            string caseId = string.Empty;

            if (string.IsNullOrEmpty(request.contact_id))
            {
                var createContactUrl = _settingService.GetBySettingKey(SysSettingKeyConstants.CreateContact).Result.Data?.Value;
                var contactRequestBody = new
                {
                    module_name = "Contacts",
                    name_value_list = new
                    {
                        national_id_c = request.national_id,
                        first_name = request.first_name,
                        last_name = request.last_name,
                        email1 = request.email1,
                        phone_mobile = request.phone_mobile
                    }
                };

                var contactRequest = new HttpRequestMessage(HttpMethod.Post, createContactUrl)
                {
                    Content = JsonContent.Create(contactRequestBody)
                };
                contactRequest.Headers.Add("Authorization", $"Bearer {token}");

                var contactResponse = await _httpClient.SendAsync(contactRequest);
                if (!contactResponse.IsSuccessStatusCode)
                    return new BaseResponse(false, "خطأ أثناء إنشاء جهة الاتصال");

                var contactJson = await contactResponse.Content.ReadAsStringAsync();
                var contactData = JsonSerializer.Deserialize<CreateContactResponseDto>(contactJson);
                if (contactData == null || string.IsNullOrEmpty(contactData.id))
                    return new BaseResponse(false, "لم يتم الحصول على رقم تعريفي لجهة الاتصال");

                contactId = contactData.id;
            }
            else
            {
                contactId = request.contact_id;
            }

            // Step 2: Create Case
            var createCaseUrl = _settingService.GetBySettingKey(SysSettingKeyConstants.CreateNewCase).Result.Data?.Value;
            var caseRequestBody = new
            {
                module_name = "Cases",
                name_value_list = new
                {
                    name = request.name,
                    description = request.description,
                    requestclassification_c = request.requestclassification_c,
                    rejon_c = request.rejon_c,
                    priority = "P3",
                    status = "Open_New",
                    account_id = "ACCOUNT_ID", // Replace with actual account id if available
                    reqclass_c = "internal",
                    contact_soruce_c = "Web",
                    service_type_c = "REAC",
                    jihah_following_c = "Rega"
                }
            };

            var caseRequest = new HttpRequestMessage(HttpMethod.Post, createCaseUrl)
            {
                Content = JsonContent.Create(caseRequestBody)
            };
            caseRequest.Headers.Add("Authorization", $"Bearer {token}");

            var caseResponse = await _httpClient.SendAsync(caseRequest);
            if (!caseResponse.IsSuccessStatusCode)
                return new BaseResponse(false, "خطأ أثناء إنشاء الشكوى");

            var caseJson = await caseResponse.Content.ReadAsStringAsync();
            var caseData = JsonSerializer.Deserialize<CreateCaseResponseDto>(caseJson);
            if (caseData == null || string.IsNullOrEmpty(caseData.id))
                return new BaseResponse(false, "لم يتم الحصول على رقم الشكوى");
            caseId = caseData.id;

            // Step 3: Link Contact to Case
            var linkContactUrl = _settingService.GetBySettingKey(SysSettingKeyConstants.LinkContactToCase).Result.Data?.Value;
            var linkContactRequestBody = new LinkContactToCaseRequestDto
            {
                module_name = "Cases",
                module_id = caseId,
                link_field_name = "contacts",
                related_ids = new List<string> { contactId }
            };

            var linkContactRequest = new HttpRequestMessage(HttpMethod.Post, linkContactUrl)
            {
                Content = JsonContent.Create(linkContactRequestBody)
            };
            linkContactRequest.Headers.Add("Authorization", $"Bearer {token}");

            var linkContactResponse = await _httpClient.SendAsync(linkContactRequest);
            if (!linkContactResponse.IsSuccessStatusCode)
                return new BaseResponse(false, "خطأ أثناء ربط جهة الاتصال بالشكوى");

            var linkContactJson = await linkContactResponse.Content.ReadAsStringAsync();
            var linkContactData = JsonSerializer.Deserialize<LinkContactToCaseResponseDto>(linkContactJson);
            if (linkContactData == null || linkContactData.created != 1)
                return new BaseResponse(false, "فشل ربط جهة الاتصال بالشكوى");

            // Step 4: Link Notes to Case (iterate over notesIds)
            var linkNoteUrl = _settingService.GetBySettingKey(SysSettingKeyConstants.LinkNoteToCase).Result.Data?.Value;

            var linkNoteRequestBody = new LinkNoteToCaseRequestDto
            {
                module_name = "Cases",
                module_id = caseId,
                link_field_name = "notes",
                related_ids = request.notesIds
            };

            var linkNoteRequest = new HttpRequestMessage(HttpMethod.Post, linkNoteUrl)
            {
                Content = JsonContent.Create(linkNoteRequestBody)
            };
            linkNoteRequest.Headers.Add("Authorization", $"Bearer {token}");

            var linkNoteResponse = await _httpClient.SendAsync(linkNoteRequest);
            if (!linkNoteResponse.IsSuccessStatusCode)
                return new BaseResponse(false, "خطأ أثناء ربط الملاحظة بالشكوى");

            var linkNoteJson = await linkNoteResponse.Content.ReadAsStringAsync();
            var linkNoteData = JsonSerializer.Deserialize<LinkNoteToCaseResponseDto>(linkNoteJson);
            if (linkNoteData == null || linkNoteData.created != 1)
                return new BaseResponse(false, "فشل ربط الملاحظة بالشكوى");

            // Link NoteIds to Case Ids in ComplaintFiles Table
            foreach (var noteId in request.notesIds)
            {

                var updateComplaintFiles = await _repository.ExecuteUpdateAsync<ComplaintFile, Guid>(
                    filter: x => x.NoteId == Guid.Parse(noteId),
                    updateExpression: s => s
                        .SetProperty(c => c.CaseId, Guid.Parse(caseId))
                );
            }


            return new BaseResponse(true, "تمت إضافة الشكوى بنجاح");
        }

        public async Task<DataResponse<CasesByContactResponseDto>> GetCasesByContactAsync(string nationalId)
        {
            var getContactDataUrl = _settingService.GetBySettingKey(SysSettingKeyConstants.GetContact).Result.Data?.Value;
            getContactDataUrl = getContactDataUrl.Replace("{nationalId}", nationalId);
            var token = (await _gEELoginService.Login()).Data?.Access_token;

            var getContactDataRequest = new HttpRequestMessage(HttpMethod.Get, getContactDataUrl);
            getContactDataRequest.Headers.Add("Authorization", $"Bearer {token}");

            var getContactDataResponse = await _httpClient.SendAsync(getContactDataRequest);
            if (!getContactDataResponse.IsSuccessStatusCode)
                return new DataResponse<CasesByContactResponseDto>(null, false, "حدث خطأ اثناء جلب بيانات المستخدم");

            var contactJson = await getContactDataResponse.Content.ReadAsStringAsync();
            var contactData = JsonSerializer.Deserialize<CasesByContactResponseDto>(contactJson);

            if (contactData == null || contactData.entry_list == null || !contactData.entry_list.Any())
                return new DataResponse<CasesByContactResponseDto>(null, false, "خطأ داخلي");

            var moduleId = contactData.entry_list.First().id;

            var getCasesByContactUrl = _settingService.GetBySettingKey(SysSettingKeyConstants.GetCasesByContact).Result.Data?.Value;
            getCasesByContactUrl = getCasesByContactUrl.Replace("{ModuleId}", moduleId);

            var getCasesByContactRequest = new HttpRequestMessage(HttpMethod.Get, getCasesByContactUrl);
            getCasesByContactRequest.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(getCasesByContactRequest);
            if (!response.IsSuccessStatusCode)
                return new DataResponse<CasesByContactResponseDto>(null, false, "حدث خطأ اثناء جلب الشكاوى");

            var casesResponse = await response.Content.ReadAsStringAsync();
            var casesData = JsonSerializer.Deserialize<CasesByContactResponseDto>(casesResponse);

            if (casesData == null)
                return new DataResponse<CasesByContactResponseDto>(null, false, "خطأ داخلي");

            if (casesData != null && casesData.entry_list != null && casesData.entry_list.Count > 0)
            {
                casesData.entry_list = casesData.entry_list
                    .OrderByDescending(e => e.name_value_list?.date_entered?.value)
                    .ToList();
            }


            return new DataResponse<CasesByContactResponseDto>(casesData, true, "");



            // Test Data
            //var entry1 = new CaseEntry()
            //{
            //    id = "1",
            //    module_name = "",
            //    name_value_list = new CaseNameValueList()
            //    {
            //        id = new NameValue() { name = "id", value = "1" },
            //        name = new NameValue() { name = "name", value = "شكوى على خدمة المياه" },
            //        case_number = new NameValue() { name = "case_number", value = "12345" },
            //        status = new NameValue() { name = "status", value = "Open_New" },

            //        date_entered = new NameValue() { name = "date_entered", value = "2023-10-01 10:00:00" }

            //    }
            //};

            //var entry2 = new CaseEntry()
            //{
            //    id = "2",
            //    module_name = "",
            //    name_value_list = new CaseNameValueList()
            //    {
            //        id = new NameValue() { name = "id", value = "2" },
            //        name = new NameValue() { name = "name", value = "شكوى على خدمة المياه" },
            //        case_number = new NameValue() { name = "case_number", value = "12345" },
            //        status = new NameValue() { name = "status", value = "Open_New" },
            //        description = new NameValue() { name = "description", value = "تفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هنا" },
            //        rejon_c = new NameValue() { name = "rejon_c", value = "1" },
            //        requestclassification_c = new NameValue() { name = "requestclassification_c", value = "نوع الشكوى" },
            //        date_entered = new NameValue() { name = "date_entered", value = "2023-10-01 10:00:00" }

            //    }
            //};
            //var entryList = new List<CaseEntry>() { };
            //entryList.Add(entry1);
            //entryList.Add(entry2);

            //var testData = new CasesByContactResponseDto()
            //{
            //    entry_list = entryList,
            //    result_count = entryList.Count,
            //    next_offset = 0,
            //    total_count = entryList.Count.ToString(),
            //    relationship_list = new List<object>()
            //};

            //return new DataResponse<CasesByContactResponseDto>(testData, true, "Success");
        }

        public async Task<DataResponse<CaseByIdResponseDto>> GetCaseByIdAsync(string caseId)
        {
            var url = _settingService.GetBySettingKey(SysSettingKeyConstants.GetCaseById).Result.Data?.Value;
            url = url.Replace("{caseId}", caseId);
            var token = (await _gEELoginService.Login()).Data?.Access_token;

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new DataResponse<CaseByIdResponseDto>(null, false, "حدث خطأ اثناء جلب بيانات الشكوى");

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<CaseByIdResponseDto>(json);
            if (data == null)
                return new DataResponse<CaseByIdResponseDto>(null, false, "خطأ داخلي ");

            // Get Complaint Files from ComplaintFiles Table
            var complaintFiles = (await _repository.GetQueryable<ComplaintFile, Guid>())
                .Include(x => x.FileManager)
                .Where(x => x.CaseId == Guid.Parse(caseId) && x.IsDeleted != true)
                .AsNoTracking()
                .ToList();

            if (complaintFiles is not null && complaintFiles.Count > 0)
                data.ComplaintFiles = _mapper.Map<List<SysFileManagerCreateUpdateDto>>(complaintFiles.Select(x => x.FileManager));

            return new DataResponse<CaseByIdResponseDto>(data, true, "Success");

            // Test Data

            //var entry1 = new CaseEntry()
            //{
            //    id = caseId,
            //    module_name = "",
            //    name_value_list = new CaseNameValueList()
            //    {
            //        id = new NameValue() { name = "id", value = caseId },
            //        name = new NameValue() { name = "name", value = "شكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياهشكوى على خدمة المياه" },
            //        case_number = new NameValue() { name = "case_number", value = "12345" },
            //        status = new NameValue() { name = "status", value = "Open_New" },
            //        description = new NameValue() { name = "description", value = "تفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هناتفاصيل الشكوى هنا" },
            //        rejon_c = new NameValue() { name = "rejon_c", value = "1" },
            //        requestclassification_c = new NameValue() { name = "requestclassification_c", value = "نوع الشكوى" },
            //        date_entered = new NameValue() { name = "date_entered", value = "2023-10-01 10:00:00" }

            //    }
            //};

            //var entryList = new List<CaseEntry>() { };
            //entryList.Add(entry1);

            //var testData = new CaseByIdResponseDto()
            //{
            //    entry_list = entryList,
            //    result_count = entryList.Count,
            //    next_offset = 0,
            //    total_count = entryList.Count.ToString(),
            //    relationship_list = new List<object>(),
            //    ComplaintFiles = new List<SysFileManagerCreateUpdateDto>()
            //    {
            //        new SysFileManagerCreateUpdateDto()
            //        {
            //            Id = Guid.NewGuid(),
            //            FileDescription = "file1.pdf",
            //            FilePath = "/files/file1.pdf",
            //            FileType = "application/pdf",

            //        },
            //        new SysFileManagerCreateUpdateDto()
            //        {
            //            Id = Guid.NewGuid(),
            //            FileDescription = "file1.pdf",
            //            FilePath = "/files/file1.pdf",
            //            FileType = "application/pdf",

            //        },
            //    }
            //};

            //return new DataResponse<CaseByIdResponseDto>(testData, true, "Success");

        }



    }
}
