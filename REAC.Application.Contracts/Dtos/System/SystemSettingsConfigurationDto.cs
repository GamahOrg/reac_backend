using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.System
{
    public class SystemSettingsConfigurationDto
    {
        public ReconciliationSettings ReconciliationSettings { get; set; }
    }

    public class ReconciliationSettings
    {
        public string ReconciliationFees { get; set; }
        public string ReformerJoiningRequestFees { get; set; }
    }
}
