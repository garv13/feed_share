using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SelfDrvn.Feeds.Share.Models
{
    public class ViewMetadataRequest
    {        
            public string blobUrl { get; set; }
            public int userId { get; set; }
            public DateTime dateViewed { get; set; }
            public bool isUtc { get; set; }
            public bool isActive { get; set; }
    }
}
