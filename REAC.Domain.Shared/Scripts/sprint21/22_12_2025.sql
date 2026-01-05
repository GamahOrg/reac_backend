  
 
  -- update nextflowstepId, status for Accept reconciliation  
   update WorkFlowStepEvent set NextWorkFlowStepDestinationId='F0E28556-E0AB-4B7E-97F1-8A9912D720A3' , InternalStatusId=28 ,ExternalStatusId=28 where id='3F01924E-59F4-44B7-9506-75C85B89B773' and EventName=N'قبول'
   -- update status for reconciliation fees 
   update WorkFlowStepEvent set ActionModeId=Null,NextWorkFlowStepDestinationId=Null , InternalStatusId=2 ,ExternalStatusId=2 where Id='8D908DF8-C897-4CD3-94CF-B8F5CD218F9F' and EventName=N'تم دفع رسوم الصلح'
