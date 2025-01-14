namespace REAC.Domain.Shared.Enum
{
    public enum LitigationRequestDecision
    {
        ///<summary>تنازل عن الحق</summary>
        WaiverOfRight = 7,
        ///<summary>تنازل عن الدعوى</summary>
        WaiverOfTheLitigation = 8,
        ///<summary>عدم الصُلح</summary>
        ReconciliationWasNotMade = 9,
        ///<summary>تم الصلح</summary>
        ReconciliationWasMade = 18
    }

    public enum ReconciliationRequestDecision
    {
        ///<summary>تنازل عن الحق</summary>
        WaiverOfRight = 7,
        ///<summary>تنازل عن الدعوى</summary>
        WaiverOfTheLitigation = 8,
        ///<summary>عدم الصُلح</summary>
        ReconciliationWasNotMade = 9,
        ///<summary>تم الصلح</summary>
        ReconciliationWasMade = 18,
        ///<summary> صلح فى منصه تراضى</summary>
        ReconciliationOnTradyPlatform = 25
    }


}
