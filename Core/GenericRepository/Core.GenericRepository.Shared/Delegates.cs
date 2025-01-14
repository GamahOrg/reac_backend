using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Core.GenericRepository.Shared
{
    public class RepositoryDelegates
    {
        public delegate BaseResponse BeforeUpdateDelegate(DbContext dbContext, object baseEntity);
        public delegate BaseResponse AfterUpdateDelegate(DbContext dbContext, object baseEntity);
        public delegate BaseResponse BeforeAddDelegate(DbContext dbContext, object baseEntity);
        public delegate BaseResponse AfterAddDelegate(DbContext dbContext, object baseEntity);
        public delegate BaseResponse BeforeDeleteDelegate(DbContext dbContext, object baseEntity);
        public delegate BaseResponse AfterDeleteDelegate(DbContext dbContext, object baseEntity);
        public delegate BaseResponse BeforeSaveDelegate(DbContext dbContext);
        public delegate BaseResponse AfterSaveDelegate(DbContext dbContext);
    }
}
