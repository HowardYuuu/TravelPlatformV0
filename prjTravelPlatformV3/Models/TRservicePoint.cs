﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjTravelPlatformV3.Models;

public partial class TRservicePoint
{
    public int FServicePointId { get; set; }

    public string FServicePoint { get; set; }

    public string FAddress { get; set; }

    public string FPhone { get; set; }

    public bool? FServicePointInUse { get; set; }

    public virtual ICollection<TRcarInfo> TRcarInfos { get; set; } = new List<TRcarInfo>();

    public virtual ICollection<TRcarRentOrderDetail> TRcarRentOrderDetails { get; set; } = new List<TRcarRentOrderDetail>();
}