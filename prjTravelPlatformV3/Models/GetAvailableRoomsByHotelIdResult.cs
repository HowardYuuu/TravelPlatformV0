﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace prjTravelPlatformV3.Models
{
    public partial class GetAvailableRoomsByHotelIdResult
    {
        public int fRoomId { get; set; }
        public string fRoomNumber { get; set; }
        public int? isReserveRoomId { get; set; }
        public int fHotelId { get; set; }
        public int fRoomTypeId { get; set; }
        public int? fMaxCapacity { get; set; }
    }
}
