using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Domain.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

}
