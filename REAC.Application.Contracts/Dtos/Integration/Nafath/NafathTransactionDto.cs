using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Integration.Nafath
{
    public class NafathTransactionDto
    {
        public string id { get; set; }
        public string random { get; set; }
        public string transId { get; set; }
    }
}