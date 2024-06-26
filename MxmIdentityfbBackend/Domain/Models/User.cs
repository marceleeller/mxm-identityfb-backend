﻿using Microsoft.AspNetCore.Identity;

namespace MxmIdentityfbBackend.Domain.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
