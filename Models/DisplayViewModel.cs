
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SelfDrvn.Feeds.Share.Models
{
    public class DisplayViewModel : FeedViewModel
    {
        public MetaViewModel Meta { get; set; }
        public SocialViewModel Social { get; set; }
        public long FeedView { get; set; }
    }
}