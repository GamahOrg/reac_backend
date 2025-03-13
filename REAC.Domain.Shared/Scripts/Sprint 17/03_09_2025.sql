UPDATE SysSetting
SET Value = N'تم اعتماد طلب {CitizenName} للانضمام كمصلح .'
WHERE [Key] = 'Reformer.AcceptJoiningRequest.WaitingForFees';

UPDATE SysSetting
SET Value = N'تم اعتماد طلب {CitizenName} للانضمام كمحكم.'
WHERE [Key] = 'Arbitrator.AcceptJoiningRequest.WaitingForFees';

UPDATE SysSetting
SET Value = N'تم أعادة تعيين جلسة الصلح  رقم الدعوى: {LitigationCode}  برجاء التواجد فى {SessionDatetime}, رابط الجلسه {SessionLink}'
WHERE [Key] = 'ReconciliationSession.Reset';

UPDATE SysSetting
SET Value = N'تم تحديد موعد لجلسة الصلح  رقم الدعوى: {LitigationCode}  برجاء التواجد فى {SessionDatetime}, رابط الجلسه {SessionLink}'
WHERE [Key] = 'ReconciliationSession.Set';