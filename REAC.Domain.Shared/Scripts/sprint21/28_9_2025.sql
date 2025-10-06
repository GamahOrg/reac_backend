-----Remove session link 
 update sysSetting
  set [Value] = N'تم اضافة موعد لجلسة الحكم في الدعوي المسجلة برقم {LitigationCode}'
  where [Key] like '%ArbitrationSession.Added%'

update sysSetting
set [Value] = N'تم تحديد موعد لجلسة الصلح  رقم الدعوى: {LitigationCode} برجاء التواجد فى {SessionDatetime},'
where [Key] like '%ReconciliationSession.Set%'


update sysSetting
set [Value] = N'تم أعادة تعيين جلسة الصلح  ترقم الدعوى: {LitigationCode} برجاء التواجد فى {SessionDatetime},'
where [Key] like '%ReconciliationSession.Reset%'
----------------------------------------------------------------------

------adding new step to arbitration
 insert SysLitigationRequestStatus (Id,RequestStatus, CreatedOn,IsDeleted,Translations) values (38,N'بانتظار موافقة المحكمين على حكم التحكيم','2025-09-30 19:14:23.9567362',0,N'[{"LanguageCode":"en-US","RequestStatus":"WaitingForArbitratorsApproval"},{"LanguageCode":"ar","RequestStatus": "بانتظار موافقة المحكمين"}]')

 insert SYSsETTING values( 'B05F8141-EF64-4C4C-A941-012539612345','ArbitrationService.JudgmentAddedForAgreement',N'يرجى الموافقه على حكم التحكيم ليتم اصداره  للدعوة المسجله برقم  {LitigationCode}' )

     alter table dbo.LitigationRequestArbitratorAssigningRequest 
  Add ResponseArbitrationMinutes bit null

  
     alter table dbo.LitigationRequestArbitratorAssigningRequest 
  Add ArbitrationMinutesOn datetime null

-----------------------------------------------------------------------

-------------------adding new step-----------------------------------
  insert SysLitigationRequestStatus (Id,RequestStatus, CreatedOn,IsDeleted,Translations) values (39,N'تم ارسال قائمة اختيار محكم','2025-10-05 19:14:23.9567362',0,N'[{"LanguageCode":"en-US","RequestStatus":"ArbitratorsNominationListIsSent"},{"LanguageCode":"ar","RequestStatus": "تم ارسال قائمة اختيار محكم"}]')
  insert SysLitigationRequestStatus (Id,RequestStatus, CreatedOn,IsDeleted,Translations) values (40,N'تم اختيار المحكمين','2025-10-05 19:14:23.9567362',0,N'[{"LanguageCode":"en-US","RequestStatus":"ArbitratorsSelectionIsDone"},{"LanguageCode":"ar","RequestStatus": "تم اختيار المحكمين"}]')
