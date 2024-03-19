using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public record LoginResponse(bool flag, string message = null!,string token = null!,ApplicationUser User = null!);
}
