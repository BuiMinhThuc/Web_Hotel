using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLKS_v1.Payload.Requests.Request_CustomerFeedBack;
using QLKS_v1.Payload.Requests.Request_Equipment;
using QLKS_v1.Payload.Requests.Request_ImageRoom;
using QLKS_v1.Payload.Requests.Request_News;
using QLKS_v1.Payload.Requests.Request_Promotion;
using QLKS_v1.Payload.Requests.Request_Room;
using QLKS_v1.Payload.Requests.Request_RoomType;
using QLKS_v1.Payload.Requests.Request_Staff;
using QLKS_v1.Payload.Requests.Request_StaffShift;
using QLKS_v1.Payload.Requests.Request_TypeService;
using QLKS_v1.Payload.Requests.Request_User;
using QLKS_v1.Services.Implements;
using QLKS_v1.Services.Interfaces;

namespace QLKS_v1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Controller_Admin : ControllerBase
    {
        private readonly IService_TypeService service_TypeService;
        private readonly IService_Room service_Room;
        private readonly IService_Customer service_Customer;
        private readonly IService_Staff service_Staff;
        private readonly IService_User service_User;
        private readonly IService_News service_News;
        private readonly IService_Promotion service_Promotion;
        private readonly IService_FeedBackCustomer service_FeedBack;
        private readonly IService_RoomType service_RoomType;
        private readonly IService_StaffShift service_StaffShift;
        private readonly IService_RoomImage service_RoomImage;
        private readonly IService_BookingService service_BookingService;
        private readonly IService_Equiment service_Equiment;
        private readonly IService_Bill service_Bill;

        public Controller_Admin(IService_TypeService service_TypeService, IService_Room service_Room, IService_Customer service_Customer, IService_Staff service_Staff, IService_User service_User, IService_News service_News, IService_Promotion service_Promotion, IService_FeedBackCustomer service_FeedBack, IService_RoomType service_RoomType, IService_StaffShift service_StaffShift, IService_RoomImage service_RoomImage, IService_BookingService service_BookingService, IService_Equiment service_Equiment, IService_Bill service_Bill)
        {
            this.service_TypeService = service_TypeService;
            this.service_Room = service_Room;
            this.service_Customer = service_Customer;
            this.service_Staff = service_Staff;
            this.service_User = service_User;
            this.service_News = service_News;
            this.service_Promotion = service_Promotion;
            this.service_FeedBack = service_FeedBack;
            this.service_RoomType = service_RoomType;
            this.service_StaffShift = service_StaffShift;
            this.service_RoomImage = service_RoomImage;
            this.service_BookingService = service_BookingService;
            this.service_Equiment = service_Equiment;
            this.service_Bill = service_Bill;
        }

        [HttpGet("GetNumTypeRoonAloneActiveHotel")]
        public async Task<IActionResult> GetNumTypeRoonAloneActiveHotel()
        {
            return Ok(await service_Room.TypeALone());
        }
        [HttpGet("GetNumTypeRoonDoubleActiveHotel")]
        public async Task<IActionResult> GetNumTypeRoonDoubleActiveHotel()
        {
            return Ok(await service_Room.TypeDouble());
        }
        [HttpGet("GetNumTypeRoonVipActiveHotel")]
        public async Task<IActionResult> GetNumTypeRoonVipActiveHotel()
        {
            return Ok(await service_Room.TypeVip());
        }
        [HttpGet("GetAdultinHotel")]
        public async Task<IActionResult> GetAdultinHotel()
        {
            return Ok(await service_Bill.TotalPersonAdult());
        }
        [HttpGet("GetChildinHotel")]
        public async Task<IActionResult> GetChildinHotel()
        {
            return Ok(await service_Bill.TotalPersonChild());
        }


        [HttpPost("CreateEquipment")]
        public async Task<IActionResult> CreateEquipment(Request_CreateEquipment service)
        {
            return Ok(await service_Equiment.CreateEquipment(service));

        }
        [HttpPut("UpdateEquipment")]
        public async Task<IActionResult> UpdateEquipment(Request_UpdateEquipment service)
        {
            return Ok(await service_Equiment.UpdateEquipment(service));

        }
        [HttpDelete("DeleteEquipment")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            return Ok(await service_Equiment.DeleteEquipment(id));

        }
        [HttpGet("GetEquipmentbyRoomId")]
        public async Task<IActionResult> GetEquipmentbyRoomId(int RoomId)
        {
            return Ok(await service_Equiment.GetListEquipmentForId(RoomId));
        }



        [HttpPost("CreateStaffShift")]
        public async Task<IActionResult> CreateStaffShift(Request_CreateStaffShift service)
        {
            return Ok(await service_StaffShift.CreateStaffShift(service));

        }
        [HttpPut("UpdateStaffShift")]
        public async Task<IActionResult> UpdateStaffShift(Request_UpdateStaffShift service)
        {
            return Ok(await service_StaffShift.UpdateStaffShift(service));

        }
        [HttpDelete("DeleteStaffShift")]
        public async Task<IActionResult> DeleteStaffShift(int service)
        {
            return Ok(await service_StaffShift.DeleteStaffShift(service));

        }
        [HttpPost("CreateImgRoom")]
        public async Task<IActionResult> CreateImgRoom(Request_CreateImgRoom service)
        {
            return Ok(await service_RoomImage.CreateImgeRoom(service));

        }
        [HttpPut("UpdateImgRoom")]
        public async Task<IActionResult> UpdateImgRoom(Request_UpdateImgRoom service)
        {
            return Ok(await service_RoomImage.UpdateImgeRoom(service));

        }
        [HttpDelete("DeleteImgRoom")]
        public async Task<IActionResult> DeleteImgRoom(int service)
        {
            return Ok(await service_RoomImage.DeletaImgeRoom(service));

        }



        [HttpGet("GetListImgRoom")]
        public async Task<IActionResult> GetListImgRoom(int Id)
        {
            return Ok(await service_RoomImage.ListImgOfRoom(Id));
        }


        [HttpPost("CreateRoomType")]
        public async Task<IActionResult> CreateRoomType(Request_CreateRoomType service)
        {
            return Ok(await service_RoomType.CreateRooomType(service));

        }
        [HttpPut("UpdateRoomType")]
        public async Task<IActionResult> UpdateRoomType(Request_UpdateRoomType service)
        {
            return Ok(await service_RoomType.UpdateRooomType(service));

        }
        [HttpDelete("DeleteRoomType")]
        public async Task<IActionResult> DeleteRoomType(int service)
        {
            return Ok(await service_RoomType.DeleteRooomType(service));

        }

        [HttpGet("GetlistRoomType")]
        public IActionResult GetlistRoomType(int pageSize=7, int pageNumber=1) 
        {
            return Ok(service_RoomType.AdmingetRoomType(pageSize, pageNumber));
        }


        [HttpPost("CreateTypeService")]
        public async Task<IActionResult> CreateTypeService(Request_CreateServiceType service)
        {
            return Ok(await service_TypeService.CreateTypeService(service));

        }
        [HttpPut("UpdateTypeService")]
        public async Task<IActionResult> UpdateTypeService(Request_UpdateTypeService service)
        {
            return Ok(await service_TypeService.UpdateTypeService(service));

        }
        [HttpDelete("DeleteTypeService")]
        public async Task<IActionResult> DeleteTypeService(int service)
        {
            return Ok(await service_TypeService.DeleteTypeService(service));

        }




        [HttpGet("GetChoonseTYpeService")]
        public IActionResult GetChoonseTYpeService()
        {
            return Ok(service_TypeService.ChooseTypeServices());
        }

        [HttpGet("AdminGetChoonseTYpeService")]
        public IActionResult AdminGetChoonseTYpeService(int pageSize=7, int pageNumber=1)
        {
            return Ok(service_TypeService.AdminChooseTypeServices(pageSize,pageNumber));
        }


        [HttpPost("CreateRoom")]
        public async Task<IActionResult> CreateRoom(Request_CreateRoom request)
        {
            return Ok(await service_Room.CreateRoom(request));
        }

        [HttpPut("UpdateRoom")]
        public async Task<IActionResult> UpdateRoom(Request_UpdateRoom request)
        {
            return Ok(await service_Room.UpdateRoom(request));
        }

        [HttpDelete("DeleteRoom")]
        public async Task<IActionResult> DeleteRoom(int request)
        {
            return Ok(await service_Room.DeleteRoom(request));
        }


        [HttpGet("getlisShowCustomer")]
        public async Task<IActionResult> getlisShowCustomer(int pageSize=7,int pageNumber = 1)
        {
            return Ok( await service_Customer.GetAllListView(pageSize, pageNumber));
        }

        [HttpPut("UpdateStaff")]
        public async Task<IActionResult> UpdateStaff(Request_UpdateStaff request)
        {
            return Ok(await service_Staff.UpdateStaff(request));
        }
        [HttpDelete("DeleteStaff")]
        public async Task<IActionResult> DeleteStaff(int Id)
        {
            return Ok(await service_Staff.DeleteStaff(Id));
        }
        [HttpPut("UpdateUserf")]
        public async Task<IActionResult> UpdateUserf(Request_UpdateUser request)
        {
            return Ok(await service_User.UpdateUser(request));
        }
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            return Ok(await service_User.DeleteUser(Id));
        }
        [HttpPost("CreateNews")]
        public async Task<IActionResult> CreateNews(Request_CreateNews request)
        {
            return Ok(await service_News.CreateNews(request));
        }
        [HttpPut("UpdateNews")]
        public async Task<IActionResult> UpdateNews(Request_UpdateNews request)
        {
            return Ok(await service_News.UpdateNews(request));
        }
        [HttpDelete("DeleteNews")]
        public async Task<IActionResult> DeleteNews(int Id)
        {
            return Ok(await service_News.DeleteNews(Id));
        }
        [HttpPost("CreatePromotion")]
        public async Task<IActionResult> CreatePromotion(Request_CreatePromotion request)
        {
            return Ok(await service_Promotion.CreatePromotion(request));
        }
        [HttpPut("UpdatePromotion")]
        public async Task<IActionResult> UpdatePromotion(Request_UpdatePromotion request)
        {
            return Ok(await service_Promotion.UpdatePromotion(request));
        }
        [HttpDelete("DeletePromotion")]
        public async Task<IActionResult> DeletePromotion(int Id)
        {
            return Ok(await service_Promotion.DeletePromotion(Id));
        }
        [HttpPost("CreateFeedback")]
        public async Task<IActionResult> CreateFeedback(Request_CreateFeedBack request)
        {
            return Ok(await service_FeedBack.CreateFeedBack(request));
        }
        [HttpGet("GetListFeedback")]
        public IActionResult GetListFeedback(int pageSize=7, int pageNumber = 1)
        {
            return Ok(service_FeedBack.GetlistFeedBack(pageSize, pageNumber));
        }
        [HttpGet("getListBookingBillService")]
        public async Task<IActionResult> getListBookingBillService(int User,int pageSize = 7, int pageNumber = 1)
        {
            return Ok( await service_BookingService.GetListBookingServiceForUser(User,pageSize, pageNumber));
        }

    }
}
