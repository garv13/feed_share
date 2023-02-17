using System;

namespace SelfDrvn.Feeds.Share.Models
{
    public class MetaViewModel
    {
        public string Thumbnail { get; set; }
        public string V360 { get; set; }
        public string V480 { get; set; }
        public string V720 { get; set; }
        public string V1080 { get; set; }
        public bool IsThumbnailReady()
        {
            return !string.IsNullOrEmpty(Thumbnail);
        }
        public bool IsVideoReady()
        {
            return !string.IsNullOrEmpty(V360) || !string.IsNullOrEmpty(V480) || !string.IsNullOrEmpty(V720) || !string.IsNullOrEmpty(V1080);
        }
    }
}