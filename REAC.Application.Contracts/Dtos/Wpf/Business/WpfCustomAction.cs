using Core.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Wpf.Business
{
    public interface IWpfCustomAction
    {
        BaseResponse Execute();
    }
}
