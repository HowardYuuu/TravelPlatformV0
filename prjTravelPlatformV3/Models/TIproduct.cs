﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjTravelPlatformV3.Models;

public partial class TIproduct
{
    public int FProductId { get; set; }

    public string FProductName { get; set; }

    public bool? FProSource { get; set; }

    public int? FSupplierId { get; set; }

    public int? FTypeId { get; set; }

    public string FRelease { get; set; }

    public bool? FProStatus { get; set; }

    public string FImagePath { get; set; }

    public string FDescription { get; set; }

    public virtual TCcompanyInfo FSupplier { get; set; }

    public virtual TItype FType { get; set; }

    public virtual ICollection<TIproductSpec> TIproductSpecs { get; set; } = new List<TIproductSpec>();
}