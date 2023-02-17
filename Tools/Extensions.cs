using System;
using SelfDrvn.Feeds.Share.Data;
using SelfDrvn.Feeds.Share.Models;

namespace SelfDrvn.Feeds.Share.Tools
{
    public static class Extensions
    {
        public static Post ToPost(this FeedViewModel feed)
        {
            var post = new Post
            {
                FeedId = feed.FeedId,
                UniqueId = StringTools.RandomString(8),
                Type = feed.Type,
                OriginalFeedUrl = feed.OriginalFeedUrl,
                Title = feed.Title,
                Content = feed.Content,
                FeedImageUrl = feed.FeedImageUrl,
                CreatedBy = feed.CreatedBy,
                CreatedDate = feed.CreatedDate,
                CreatorImageUrl = feed.CreatorImageUrl,
                Organization = feed.Organization,
                OrganizationLogoUrl = feed.OrganizationLogoUrl
            };
            return post;
        }

        public static DisplayViewModel ToDisplayViewModel(this Post post, MetaViewModel metaView = null, long feedView = 0)
        {
            var displayViewModel = new DisplayViewModel
            {
                FeedId = post.FeedId,
                Type = post.Type,
                OriginalFeedUrl = post.OriginalFeedUrl,
                Title = post.Title,
                Content = post.Content,
                FeedImageUrl = post.FeedImageUrl,
                CreatedBy = post.CreatedBy,
                CreatedDate = post.CreatedDate,
                CreatorImageUrl = post.CreatorImageUrl,
                Organization = post.Organization,
                OrganizationLogoUrl = post.OrganizationLogoUrl,
                Meta = metaView,
                TimeOffset = 0,
                FeedView = feedView
            };
            return displayViewModel;
        }

        public static string TimeAgo(this DateTime dateTime, int offset = 0)
        {
            string result = string.Empty;
            // convert back to current time for calculation
            var dateResult = dateTime.AddMinutes(offset);
            var timeSpan = DateTime.UtcNow.Subtract(dateResult);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("a moment ago", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format("about {0} minutes ago", timeSpan.Minutes) :
                    "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format("about {0} hours ago", timeSpan.Hours) :
                    "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format("about {0} days ago", timeSpan.Days) :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format("about {0} months ago", timeSpan.Days / 30) :
                    "about a month ago";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("about {0} years ago", timeSpan.Days / 365) :
                    "about a year ago";
            }

            return result;
        }
    }
}