﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjTravelPlatformV3.Models;

public partial class TDcExchangeItem
{
    public int FProductId { get; set; }

    public string FName { get; set; }

    public int? FPointsRequired { get; set; }

    public decimal? FMoneyRequired { get; set; }

    public string FProductType { get; set; }

    public int? FQuantity { get; set; }

    public string FImagePath { get; set; }

    public string FNote { get; set; }

    public virtual ICollection<TDcExchangeRecord> TDcExchangeRecords { get; set; } = new List<TDcExchangeRecord>();
}