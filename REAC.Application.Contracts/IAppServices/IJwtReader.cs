namespace REAC.Application.Contracts.IAppServices
{
    public interface IJwtReader
    {
        public bool GetCitizenId(ref Guid customerId);
        public bool GetEmployeeId(ref Guid employeeId);
        public bool GetReferenceId(ref Guid referenceId);
        public bool GetUserId(ref Guid? userId);
        public Guid? GetUserId();
    }
}
