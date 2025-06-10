﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Domain.Entities
{
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public Role Role { get; set; }

        public bool IsActive { get; set; } = false;

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiration { get; set; }

        public int? ImageId { get; set; } = null;

        public Image? Image { get; set; } = null;

        public List<Offer> Offers { get; set; } = new();
    }
}
