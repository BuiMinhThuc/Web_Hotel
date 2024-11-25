using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.Converters;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Services.Interfaces;

namespace QLKS_v1.Services.Implements
{
    public class Service_Customer : IService_Customer
    {
        private readonly Converter_Customer converter_Customer;
        private readonly AppDbContext dbContext;

        public Service_Customer(Converter_Customer converter_Customer, AppDbContext dbContext)
        {
            this.converter_Customer = converter_Customer;
            this.dbContext = dbContext;
        }

        public IQueryable<DTO_Customer> GetAll(int pageSize, int pageNumber)
        {
            return dbContext.customers.OrderByDescending(x => x.Id).AsNoTracking().Skip((pageNumber-1)*pageSize).Take(pageSize).Select(x=>converter_Customer.EntityToDTO(x));
        }

        public async Task<IQueryable<DTO_ShowCustomer>> GetAllListView(int pageSize, int pageNumber)
        {
            // Lấy danh sách khách hàng từ cơ sở dữ liệu và thực hiện nhóm theo FullName, Email, và CardNumber
            var groupedCustomers = await dbContext.customers
                .Where(c => !string.IsNullOrEmpty(c.FullName) || !string.IsNullOrEmpty(c.Email) || !string.IsNullOrEmpty(c.CardNumber)) // Lọc các bản ghi có thông tin
                .GroupBy(c => new { c.FullName, c.Email, c.CardNumber }) // Nhóm theo FullName, Email, và CardNumber
                .Select(g => new DTO_ShowCustomer
                {
                    Id = g.Min(c => c.Id), // Chọn Id nhỏ nhất trong nhóm làm đại diện
                    FullName = g.Key.FullName,
                    Phone = g.Select(c => c.Phone).FirstOrDefault(p => !string.IsNullOrEmpty(p)), // Chọn số điện thoại đầu tiên không null
                    Email = g.Key.Email,
                    CardNumber = g.Key.CardNumber,
                    Nationality = g.Select(c => c.Nationality).FirstOrDefault(n => !string.IsNullOrEmpty(n)), // Chọn quốc tịch đầu tiên không null
                    Gender = g.Select(c => c.Gender).FirstOrDefault(gender => !string.IsNullOrEmpty(gender)), // Chọn giới tính đầu tiên không null
                    DateOfBirth = g.Select(c => c.DateOfBirth).FirstOrDefault(dob => dob.HasValue) // Chọn ngày sinh đầu tiên có giá trị
                })
                .Skip((pageNumber - 1) * pageSize) // Phân trang: bỏ qua số lượng bản ghi tương ứng
                .Take(pageSize) // Lấy số lượng bản ghi theo kích thước trang
                .ToListAsync(); // Thực hiện truy vấn và trả về danh sách đồng bộ

            // Trả về kết quả dưới dạng IQueryable
            return groupedCustomers.AsQueryable();
        }

    }
}
