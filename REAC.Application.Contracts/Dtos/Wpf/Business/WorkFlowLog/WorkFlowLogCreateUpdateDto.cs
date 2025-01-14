using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowLog;

public partial class WorkFlowLogCreateUpdateDto : WorkFlowLogDto
{
    public bool ShowForApplicant { get; set; }

    public List<WorkFlowLogTranslation> Translations { get; set; } = new List<WorkFlowLogTranslation>();

}
