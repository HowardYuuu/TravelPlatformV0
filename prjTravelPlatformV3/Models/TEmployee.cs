﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjTravelPlatformV3.Models;

public partial class TEmployee
{
    public int FEmployeeId { get; set; }

    public string FName { get; set; }

    public string FAccountNumber { get; set; }

    public string FPassword { get; set; }

    public string FSalt { get; set; }

    public string FEmail { get; set; }

    public string FAddress { get; set; }

    public string FBirth { get; set; }

    public string FPhone { get; set; }

    public int? FStaffId { get; set; }

    public string FIdentityNumber { get; set; }

    public string FImagePath { get; set; }

    public string FGender { get; set; }

    public bool? FStatus { get; set; }

    public bool? FIsLogin { get; set; }

    public DateTime? FLastLoginDateTime { get; set; }

    public virtual TEmployeeStaff FStaff { get; set; }
}