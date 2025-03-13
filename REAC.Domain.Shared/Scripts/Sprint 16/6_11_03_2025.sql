update [dbo].syssetting set Value = N'تنويه بانه تم إعادة طلب {CitizenName} للانضمام كمصلح للتعديل لوجود ملاحظات' where [Key]='Reformer.UpdateJoiningRequest.NoFees'

---------------------------------------------------
ALTER TABLE LitigationRequestReformerAssigningRequest
ADD Reason NVARCHAR(MAX) NULL;
---------------------------------------------------