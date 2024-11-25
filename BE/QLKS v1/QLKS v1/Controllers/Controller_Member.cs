using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLKS_v1.Interfaces;
using QLKS_v1.Payload.Requests.Request_Booking;
using QLKS_v1.Payload.Requests.Request_BookingService;
using QLKS_v1.Payload.Requests.Request_Room;
using QLKS_v1.Services.Interfaces;
using System.Net.Mail;
using System.Net;
using QLKS_v1.Handle;
using QLKS_v1.Services.Implements;
using QLKS_v1.Payload.Requests.Request_Service;
using QLKS_v1.Payload.Requests.Request_BookingRoom;

namespace QLKS_v1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Controller_Member : ControllerBase
    {
        private readonly IService_Booking serviceBooking;
        private readonly IService_Room serviceRoom;
        private readonly IService_Service service;
        private readonly IService_RoomType service_RoomType;
        private readonly IService_BookingService serviceBookingService;
        private readonly IService_RoomImage service_RoomImage;
        private readonly IService_News service_News;
        private readonly IVNPayService _vnpayService;
        private readonly IService_Bill service_Bill;
        private readonly IService_Staff service_Staff;
        private readonly IService_Promotion service_Promotion;
        private readonly IService_Customer service_Customer;
        private readonly IService_StaffShift service_StaffShift;
   
        private readonly IConfiguration configuration;

        public Controller_Member(IService_Booking serviceBooking, IService_Room serviceRoom, IService_Service service, IService_RoomType service_RoomType, IService_BookingService serviceBookingService, IService_RoomImage service_RoomImage, IService_News service_News, IVNPayService vnpayService, IService_Bill service_Bill, IService_Staff service_Staff, IService_Promotion service_Promotion, IService_Customer service_Customer, IService_StaffShift service_StaffShift, IConfiguration configuration)
        {
            this.serviceBooking = serviceBooking;
            this.serviceRoom = serviceRoom;
            this.service = service;
            this.service_RoomType = service_RoomType;
            this.serviceBookingService = serviceBookingService;
            this.service_RoomImage = service_RoomImage;
            this.service_News = service_News;
            _vnpayService = vnpayService;
            this.service_Bill = service_Bill;
            this.service_Staff = service_Staff;
            this.service_Promotion = service_Promotion;
            this.service_Customer = service_Customer;
            this.service_StaffShift = service_StaffShift;
            this.configuration = configuration;
        }

        [HttpPost("CreateBooking")]
        public async Task<IActionResult> PostBooking(Request_CreateBooking request)
        {
            return Ok(await serviceBooking.CreateBooking(request));
        }
        [HttpPost("CreateBookingAdmin")]
        public async Task<IActionResult> PostBookingAdmin(Request_AdminBooking request)
        {
            return Ok(await serviceBooking.AdminCreateBooking(request));
        }
        [HttpPut("UpdateBooking")]
        public async Task<IActionResult> PutBooking(Request_UpdateBooking request)
        {
            return Ok(await serviceBooking.UpdateBooking(request));
        } 
        [HttpDelete("DeleteBooking")]
        public async Task<IActionResult> DeleteBooking(int request)
        {
            return Ok(await serviceBooking.DeleteBooking(request));
        }
        [HttpGet("GetListBooking")]
        public async Task<IActionResult> GetBooking(int? pageSize,int? pageNumber)
        {
            return Ok(await serviceBooking.GetListBooking(pageSize, pageNumber));
        }
        [HttpGet("GetListChooseRoom")]
        public IActionResult GetListChooseRooom()
        {
            return Ok( serviceRoom.getListChooseRoom());
        }

        [HttpGet("GetAllRoom")]
        public IActionResult GetAllRoom(int pageSize=7,int pageNumber=1)
        {
            return Ok(serviceRoom.getAllRoom(pageSize, pageNumber));
        }

        [HttpPost("CreateService")]
        public async Task<IActionResult> CreateService(Request_CreateService request)
        {


            return  Ok(await service.CreateService(request));
        }
        [HttpPut("UpdateService")]
        public async Task<IActionResult> UpdateService(Request_UpdateService request)
        {


            return Ok(await service.UpdateService(request));
        }
        [HttpDelete("DeleteService")]
        public async Task<IActionResult> DeleteService(int Id)
        {


            return Ok(await service.DeleteService(Id));
        }
        [HttpGet("getAllService")]
        public IActionResult GetAllService(int pageSize=7, int pageNumber=1)
        {


            return Ok(service.GetAllService(pageSize, pageNumber));
        }
        [HttpGet("GetListChooseService")]
        public IActionResult GetListChooseService()
        {
            return Ok(service.GetChoonseService());
        }
        [HttpPost("CreateBookingService")]
        public async Task<IActionResult> PostBookingService(Request_CreateBookingService request)
        {
            return Ok(await serviceBookingService.CreateBookingService(request));
        }
        [HttpPut("updateBookingService/{billServiceId}")]
        public async Task<IActionResult> PutBookingService(int billServiceId)
        {
            return Ok(await serviceBookingService.UpdateBookingService(billServiceId));
        }
        [HttpGet("GetListBillService")]
        public IActionResult GetListBillService(int pageSize=7, int pageNumber =1)
        {
            return Ok(serviceBookingService.GetListBookingService(pageSize, pageNumber));
        }
        [HttpGet("GetListBillServiceForCase")]
        public IActionResult GetListBillService(string Type,int pageSize = 7, int pageNumber = 1)
        {
            return Ok(serviceBookingService.GetListBookingServiceForCase(Type,pageSize, pageNumber));
        }
        [HttpGet("GetListRoom")]
        public IActionResult GetListRoom()
        {
            return Ok(serviceRoom.getListRoom());
        }
      
    
        
    [HttpGet("GetListRoomImage")]
        public IActionResult GetListRoomImage()
        {
            return Ok(service_RoomImage.getListAvtRoom());
        }
        [HttpGet("GetListNews")]
        public IActionResult GetListNew(int? pageSize=10,int? pageNumber=1)
        {
            return Ok(service_News.GetNewsPT(pageSize,pageNumber));
        }
        [HttpPost("TaoDuongDanThanhToan/{hoaDonId}/{id}")]
       
        public async Task<IActionResult> TaoDuongDanThanhToan([FromRoute] int hoaDonId,int id)
        {
            
            return Ok(await _vnpayService.TaoDuongDanThanhToan(hoaDonId, HttpContext, id));
        }
        [HttpGet("Return")]
        public async Task<IActionResult> Return()
        {
            var vnpayData = HttpContext.Request.Query;

            return Redirect( await _vnpayService.VNPayReturn(vnpayData));
        }
        [HttpGet("getListPromotion")]
        public IActionResult GetListPromotion()
        {
          

            return Ok(service_Promotion.getListPromotion());
        }
        [HttpGet("getListPromotionFull")]
        public  async Task<IActionResult> GetListPromotionFull(int pageSize=7, int pageNumber=1)
        {


            return Ok( await service_Promotion.getListPromotionAdmin(pageSize, pageNumber));
        }
        [HttpGet("getListRoomTypeName")]
        public IActionResult GetListRoomTypeName()
        {


            return Ok(service_RoomType.getListRoomTypenam());
        }

        [HttpGet("getRoomTypeHot")]
        public async Task<IActionResult> getRoomTypeHot()
        {


            return Ok(await service_RoomType.getListRoomTypeNameHot());
        }

        [HttpGet("getListBill")]
        public IActionResult GetListBill(int pageSize= 7, int pageNumber = 1)
        {


            return Ok(service_Bill.GetAll(pageSize, pageNumber));
        }

        [HttpPut("PayCheckOutBill")]
        public async Task<IActionResult> PayCheckOutBill(int billId, int UserId)
        {


            return Ok(await service_Bill.UpdateBill(billId,UserId));
        }



        [HttpGet("getviewRoom")]
        public IActionResult GetViewRoom(string RoomName)
        {


            return Ok(serviceRoom.ViewChiTietRoom(RoomName));
        }
        [HttpGet("getRoomforCase")]
        public IActionResult GetRoomforCase(string RoomName, int pageSize =7, int pageNumber = 1)
        {


            return Ok(serviceRoom.GetListRoomforCase(RoomName, pageSize, pageNumber));
        }
        [HttpGet("getRoomNull")]
        public async Task<IActionResult> GetListRoomNull(string RoomName, int pageSize = 7, int pageNumber = 1)
        {


            return Ok(await serviceRoom.getListRoomNull(RoomName, pageSize, pageNumber));
        }
        [HttpGet("getRoomNullfull")]
        public async Task<IActionResult> GetListRoomNullFull(string RoomName,DateTime CheckIn, DateTime CheckOut, int pageSize = 7, int pageNumber = 1)
        {


            return Ok(await serviceRoom.getListRoomNullFull(RoomName, pageSize, pageNumber, CheckIn, CheckOut));
        }
        [HttpGet("getAllCustomer")]
        public IActionResult GetAllCustomer(int pageSize=7, int pageNumber=1)
        {


            return Ok(service_Customer.GetAll(pageSize, pageNumber));
        }

        [HttpGet("CreatePDF")]
        public async Task<IActionResult> CreatePDF(int billID)
        {


            return Ok(await service_Bill.createBillPdf(billID));
        }

        [HttpGet("PrintBillPdfPDF")]
        public async Task<IActionResult> PrintBillPdfPDF(int billID)
        {


            return Ok(await service_Bill.PrintBill(billID));
        }


        [HttpGet("getListStaff")]
        public async Task<IActionResult> getListStaff(int pageSize =7, int pageNumber=1)
        {


            return Ok(await service_Staff.getListStaff(pageSize, pageNumber));
        }
        [HttpGet("getListStaffShift")]
        public IActionResult getListStaffShift(int pageSize = 7, int pageNumber = 1)
        {


            return Ok(service_StaffShift.GetListStaffShift(pageSize, pageNumber));
        }

        [HttpDelete("DeleteBillService")]
        public async Task<IActionResult> DeleteBillService(int service)
        {
            return Ok(await serviceBookingService.DeleteBookingService(service));

        }



    }
}
