using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.Exceptions
{
    public class CouldNotCreateException : Exception
    {
        public CouldNotCreateException(string message = "Could not create.") : base(message) { }
    }
}
