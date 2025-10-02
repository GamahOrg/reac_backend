-----Remove session link 
 update sysSetting
  set [Value] = N'تم اضافة موعد لجلسة الحكم في الدعوي المسجلة برقم {LitigationCode}'
  where Id='2EFA2177-2D45-46CA-BC3E-90413343A08F'

update sysSetting
set [Value] = N'تم تحديد موعد لجلسة الصلح  رقم الدعوى: {LitigationCode} برجاء التواجد فى {SessionDatetime},'
where Id='DAE1B827-E123-45DD-82CC-B33EFC8769E3'


update sysSetting
set [Value] = N'تم أعادة تعيين جلسة الصلح  ترقم الدعوى: {LitigationCode} برجاء التواجد فى {SessionDatetime},'
where Id='6D4525EB-D821-4825-BFB8-B71EC6783EDD'
----------------------------------------------------------------------

------adding new step to arbitration
 insert SysLitigationRequestStatus (Id,RequestStatus, CreatedOn,IsDeleted,Translations) values (38,N'بانتظار موافقة المحكمين على حكم التحكيم','2025-09-30 19:14:23.9567362',0,N'[{"LanguageCode":"en-US","RequestStatus":"WaitingForArbitratorsApproval"},{"LanguageCode":"ar","RequestStatus": "بانتظار موافقة المحكمين"}]')

 insert SYSsETTING values( 'B05F8141-EF64-4C4C-A941-012539612345','ArbitrationService.JudgmentAddedForAgreement',N'يرجى الموافقه على حكم التحكيم ليتم اصداره  للدعوة المسجله برقم  {LitigationCode}' )

     alter table dbo.LitigationRequestArbitratorAssigningRequest 
  Add ResponseArbitrationMinutes bit null

  
     alter table dbo.LitigationRequestArbitratorAssigningRequest 
  Add ArbitrationMinutesOn datetime null

-----------------------------------------------------------------------