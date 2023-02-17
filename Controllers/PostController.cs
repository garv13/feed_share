using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SelfDrvn.Feeds.Share.Data;
using SelfDrvn.Feeds.Share.Models;
using SelfDrvn.Feeds.Share.Services;
using SelfDrvn.Feeds.Share.Tools;

namespace SelfDrvn.Feeds.Share.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostShareContext _context;
        private readonly IAppSettingsService _appSettings;
        private static readonly HttpClient client = new HttpClient();

        public PostController(IPostShareContext context, IAppSettingsService appSettings)
        {
            _context = context;
            _appSettings = appSettings;
        }

        [HttpGet("p/{postId}")]
        public async Task<IActionResult> Index([FromRoute] string postId)
        {
            Post post = null;
            long views = 0;
            DisplayViewModel feed = null;

            if (!string.IsNullOrEmpty(postId))
            {
                post = _context.Posts.AsQueryable().FirstOrDefault(p => p.UniqueId == postId);
                views = _context.ViewsMetaData.CountDocuments(p => p.BlobUrl == post.FeedImageUrl && p.IsActive == true);
            }
            if (post == null) return NotFound();

            if (post.Type == "video")
            {
                MetaViewModel meta = await GetVideoMetadata(post.FeedImageUrl);
                feed = post.ToDisplayViewModel(meta,views);
            }
            else
            {
                feed = post.ToDisplayViewModel(null,views);
            }

            var cookieOffset = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "timezoneoffset");
            if (!cookieOffset.Equals(default(KeyValuePair<string, string>)))
            {
                int timeOffset = 0;
                int.TryParse(cookieOffset.Value, out timeOffset);
                feed.CreatedDate = feed.CreatedDate.AddMinutes(-1 * timeOffset);
                feed.TimeOffset = timeOffset;
            }

            feed.Social = new SocialViewModel
            {
                FacebookAppID = _appSettings.Settings.SocialMediaSettings.FacebookAppID
            };

            return View(feed); ;
        }

        private static async Task<MetaViewModel> GetVideoMetadata(string videoUrl)
        {
            MetaViewModel meta = new MetaViewModel();

            try
            {
                var emptyValue = new KeyValuePair<string, IEnumerable<string>>();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, new Uri(videoUrl));
                HttpResponseMessage response = await client.SendAsync(request);

                // Get the thumbnail
                var thumbnail = response.Headers.FirstOrDefault(h => h.Key == "x-ms-meta-thumbnail");
                if (!thumbnail.Equals(emptyValue) && thumbnail.Value.Any())
                {
                    meta.Thumbnail = thumbnail.Value.FirstOrDefault();
                }
                // Get the 360p
                var v360 = response.Headers.FirstOrDefault(h => h.Key == "x-ms-meta-v360p");
                if (!v360.Equals(emptyValue) && v360.Value.Any())
                {
                    meta.V360 = v360.Value.FirstOrDefault();
                }
                // Get the 480p
                var v480 = response.Headers.FirstOrDefault(h => h.Key == "x-ms-meta-v480p");
                if (!v480.Equals(emptyValue) && v480.Value.Any())
                {
                    meta.V480 = v480.Value.FirstOrDefault();
                }
                // Get the 720p
                var v720 = response.Headers.FirstOrDefault(h => h.Key == "x-ms-meta-v720p");
                if (!v720.Equals(emptyValue) && v720.Value.Any())
                {
                    meta.V720 = v720.Value.FirstOrDefault();
                }
                // Get the 1080p
                var v1080 = response.Headers.FirstOrDefault(h => h.Key == "x-ms-meta-v1080p");
                if (!1080.Equals(emptyValue) && v1080.Value.Any())
                {
                    meta.V1080 = v1080.Value.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return meta;
        }
    }
}