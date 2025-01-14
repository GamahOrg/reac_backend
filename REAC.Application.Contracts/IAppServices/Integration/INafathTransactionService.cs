using REAC.EntityFrameworkCore.Entities.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.IAppServices.Integration
{
    public interface INafathTransactionService
    {
        void Add(NafathTransaction nafathTransaction);
        void Remove(NafathTransaction nafathTransaction);
        List<NafathTransaction> GettAll();
    }
}
