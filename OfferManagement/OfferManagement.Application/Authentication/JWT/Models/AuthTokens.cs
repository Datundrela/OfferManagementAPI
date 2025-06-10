using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.AuthServices.JWT.Models
{
    public class AuthTokens
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

}
