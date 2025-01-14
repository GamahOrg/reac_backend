using REAC.Application.Contracts.IAppServices.Integration;
using REAC.EntityFrameworkCore.Entities.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.AppServices.Integration.GEE
{
    public class NafathTransactionService : INafathTransactionService
    {
        private List<NafathTransaction> cachedTransactionsList = new List<NafathTransaction>();

        public void Add(NafathTransaction nafathTransaction)
        {
            cachedTransactionsList.Add(nafathTransaction);
        }

        public void Remove(NafathTransaction nafathTransaction)
        {
            cachedTransactionsList.Remove(nafathTransaction);
        }

        public List<NafathTransaction> GettAll()
        {
            return cachedTransactionsList;
        }

    } 
}
