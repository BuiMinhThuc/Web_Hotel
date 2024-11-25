using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_Bill
    {
        IQueryable<DTO_Bill> GetAll(int pageSize, int pageNumber);
        Task<string> createBillPdf(int billId);
       
        Task<DTO_BillToPDF> PrintBill(int billId);
        Task<ResponseBase> UpdateBill(int billId,int UserId);

        Task<int> TotalPersonAdult();
        Task<int> TotalPersonChild();
    }
}
