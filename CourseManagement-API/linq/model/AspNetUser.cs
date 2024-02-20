using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace linq.model;

public partial class AspNetUser : IdentityUser<string>
{

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public bool? IsTeacher { get; set; }

    public bool? IsStudent { get; set; }

    public DateTime? LockDate { get; set; }

    public string? Image {  get; set; }
    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; } = new List<AspNetUserClaim>();

    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; } = new List<AspNetUserLogin>();

    public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; } = new List<AspNetUserToken>();

    public virtual ICollection<CourseStudent> CourseStudents { get; set; } = new List<CourseStudent>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
}
