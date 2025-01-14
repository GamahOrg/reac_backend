using Core.GenericRepository.Shared.BaseEntities;
using Core.Log.Domain.Shared.Enums;

namespace Core.Log.Application.Contracts.Dto
{
    public class LogDto : BaseEntity<Guid>
    {
        public string Message { get; set; }
        public LogType Type { get; set; }
        public string Format()
        {
            return "[" + Type.ToString() + "]" + "[" + DateTime.Now.ToString() + "]" + Message;
        }
    }
}
