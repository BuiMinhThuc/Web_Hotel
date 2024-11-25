﻿using QLKS_v1.Payload.Requests.Request_BookingRoom;

namespace QLKS_v1.Payload.Requests.Request_Booking
{
    public class Request_CreateBooking
    {
        public string FullName {  get; set; }
        public string NumberPhone { get; set; }
        public string Email {  get; set; }
        public List<Requesr_ListRoom> RoomTypeList { get; set; }

        /*public string CardNumber {  get; set; }*/
        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }
        public int ChildQuantity { get; set; }

        
    }
}