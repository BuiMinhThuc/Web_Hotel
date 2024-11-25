using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_Customer
    {
        IQueryable<DTO_Customer> GetAll(int pageSize, int pageNumber);
        Task<IQueryable<DTO_ShowCustomer>> GetAllListView(int pageSize, int pageNumber);
    }
}
