using System.ComponentModel.DataAnnotations;

namespace SelfDrvn.Feeds.Share.Models
{
    public class ShareFeedViewModel
    {
        [Required]
        public string FeedId { get; set; }
        [Required]
        public string Type { get; set; }
        public string OriginalFeedUrl { get; set; }
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public string FeedImageUrl { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        [Required]
        public string CreatedDate { get; set; }
        [Required]
        public string CreatorImageUrl { get; set; }
        [Required]
        public string Organization { get; set; }
        [Required]
        public string OrganizationLogoUrl { get; set; }
        public int TimeOffset { get; set; }
        public bool isContentGuidFormat { get; set; } = false;
    }
}
