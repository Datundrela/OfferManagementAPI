using OfferManagement.Application.AuthServices.JWT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.Authentication.AuthServices.Models
{
    public class LoginResponseModel
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public AuthTokens? Tokens { get; set; }
    }
}
