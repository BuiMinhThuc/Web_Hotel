using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_Customer
    {
        public DTO_Customer EntityToDTO(Customer customer)
        {
            return new DTO_Customer
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Phone= customer.Phone?? null,
                Email = customer.Email??null,
                Address = customer.Address ?? null,
                Nationality = customer.Nationality??null,
                Gender = customer.Gender ?? null,
                DateOfBirth = customer.DateOfBirth ?? null,
                CardNumber = customer.CardNumber ?? null,
                ContentPay= customer.ContentPay ?? null,
                CreateBooking = customer.CreateBooking ?? null,
            };
        }
    }
}
