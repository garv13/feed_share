
using System;

namespace SelfDrvn.Feeds.Share.Models
{
    public class FeedViewModel
    {
         
        public int FeedId { get; set; }
         
        public string Type { get; set; }
        public string OriginalFeedUrl { get; set; }
        public string Title { get; set; }
         
        public string Content { get; set; }
        public string FeedImageUrl { get; set; }
         
        public string CreatedBy { get; set; }
         
        public DateTime CreatedDate { get; set; }
         
        public string CreatorImageUrl { get; set; }
         
        public string Organization { get; set; }
         
        public string OrganizationLogoUrl { get; set; }
        public int TimeOffset { get; set; }        
    }
}