namespace REAC.Domain.Shared.Enum
{
    public enum LitigationSessionType
    {
        /// <summary>
        /// جلسة تحكيم
        /// </summary>
        ArbitrationSession = 1,

        /// <summary>
        /// جلسة النطق بالحكم
        /// </summary>
        VerdictPronouncement = 2,

        /// <summary>
        /// جلسة مداولة بين المحكمين
        /// </summary>
        DeliberationBetweenArbitrators = 3
    }
}
