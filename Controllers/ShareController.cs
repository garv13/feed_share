using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SelfDrvn.Feeds.Share.Data;
using SelfDrvn.Feeds.Share.Models;
using SelfDrvn.Feeds.Share.Services;
using SelfDrvn.Feeds.Share.Tools;

namespace SelfDrvn.Feeds.Share.Controllers
{

    [Authorize]
    [EnableCors("AllowAll")]
    public class ShareController : Controller
    {
        private readonly IPostShareContext _context;
        private readonly IAppSettingsService _appSettings;
        private const string API_Key = "4150f4a4835141a88eb665ae738e7669idscsdncsdakl";
        public ShareController(IPostShareContext context, IAppSettingsService appSettings)
        {
            _context = context;
            _appSettings = appSettings;
        }

        [HttpPost]
        [Route("api/[controller]")]
        public string ShareAsync([FromBody] ShareFeedViewModel feedData)
        {
            if (feedData == null) return string.Empty;
            DateTime createdDate;
            if (!DateTime.TryParse(feedData.CreatedDate, null, System.Globalization.DateTimeStyles.RoundtripKind, out createdDate))
            {
                throw new Exception("date not correct");
            }
            FeedViewModel feed = new FeedViewModel
            {
                Content = feedData.Content,
                CreatedBy = feedData.CreatedBy,
                CreatedDate = createdDate,
                CreatorImageUrl = feedData.CreatorImageUrl,
                FeedId = int.Parse(feedData.FeedId),
                FeedImageUrl = feedData.FeedImageUrl,
                Organization = feedData.Organization,
                OrganizationLogoUrl = feedData.OrganizationLogoUrl,
                OriginalFeedUrl = feedData.OriginalFeedUrl,
                TimeOffset = feedData.TimeOffset,
                Title = feedData.Title,
                Type = feedData.Type
            };

            Post post = null;
            post = _context.Posts.AsQueryable().FirstOrDefault(p => p.FeedId == feed.FeedId);

            if (feed.Type == "photos" || feed.Type == "image")
            {
                if (string.IsNullOrEmpty(feed.FeedImageUrl) ? false : feed.FeedImageUrl.Contains(','))
                    feed.Title = "Shared an album";
                else
                    feed.Title = "Shared a photo";

                feed.Type = "photos";
            }
            else if (feed.Type == "status" || feed.Type == "text")
            {
                feed.Title = "Shared a status";
                feed.Type = "text";
            }
            else if (feed.Type == "preview" || feed.Type == "url")
            {
                feed.Title = "Shared an url";
                feed.Type = "url";
            }
            else if (feed.Type == "Level" || feed.Type == "Badge")
                feed.Type = "system";

            if (string.IsNullOrEmpty(feed.Title))
                feed.Title = "Shared a " + feed.Type;
            string userIdFormat = "email";
            if (feedData.isContentGuidFormat || string.Equals(_appSettings.Settings.UserIdFormat, "userid"))
                userIdFormat = "userid";

            feed.Content = StringTools.CleanNameTag(feed.Content, userIdFormat, true).Replace("\n", "<br>");
            if (post == null)
            {
                post = feed.ToPost();
                _context.Posts.InsertOne(post);
            }
            else if (feed.ToPost() != post)
            {
                FilterDefinition<Post> filter = null;
                if (!string.IsNullOrEmpty(feed.OriginalFeedUrl))
                {
                    filter = Builders<Post>.Filter.Eq(p => p.OriginalFeedUrl, feed.OriginalFeedUrl);
                }
                else
                {
                    filter = Builders<Post>.Filter.Eq(p => p.FeedId, feed.FeedId);
                }
                UpdateDefinition<Post> update = Builders<Post>.Update
                    .Set(p => p.FeedId, feed.FeedId)
                    .Set(p => p.OriginalFeedUrl, feed.OriginalFeedUrl)
                    .Set(p => p.Title, feed.Title)
                    .Set(p => p.Content, feed.Content)
                    .Set(p => p.CreatedDate, feed.CreatedDate)
                    .Set(p => p.FeedImageUrl, string.IsNullOrEmpty(feed.FeedImageUrl) ? "" : feed.FeedImageUrl);
                _context.Posts.FindOneAndUpdate(filter, update);
            }

            string host = HttpContext.Request.Host.Value;

            return $"https://{host}/p/{post.UniqueId}";
        }

        /// <summary>
        ///   add/update view count of specific blob url
        /// </summary> 
        [HttpPost]
        [AllowAnonymous]
        [Route("api/[controller]/saveview")]
        public IActionResult SaveView([FromBody] ViewMetadataRequest viewMetaData)
        {
            try
            {
                Authenticate();
            }
            catch (Exception)
            {
                return StatusCode(401, "Api Key Incorrect");
            }

            var result = new List<ViewMetaData>();
            if (string.IsNullOrEmpty(viewMetaData.blobUrl) && Convert.ToInt32(viewMetaData.userId) > 0)
                return StatusCode(400, "blob url or userId is not correct");
            try
            {
                var builder = Builders<ViewMetaData>.Filter;
                var filter = _context.ViewsMetaData.AsQueryable().FirstOrDefault(p => p.BlobUrl == viewMetaData.blobUrl && p.UserId == Convert.ToInt32(viewMetaData.userId));

                if (filter == null)
                {
                    var viewToInsert = new ViewMetaData { BlobUrl = viewMetaData.blobUrl, UserId = Convert.ToInt32(viewMetaData.userId), DateViewed = viewMetaData.dateViewed, IsUtc = true, IsActive = true };
                    _context.ViewsMetaData.InsertOne(viewToInsert);
                }
                result = _context.ViewsMetaData.AsQueryable().Where(p => p.BlobUrl == viewMetaData.blobUrl).ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return StatusCode(500, "An Error has occurred ");
            }
            return Ok(result);
        }

        /// <summary>
        ///   get view count details of specific blob url
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("api/[controller]/getviewers")]
        public IActionResult GetView(string blobUrl)
        {
            try
            {
                Authenticate();
            }
            catch (Exception)
            {
                return StatusCode(401, "Api Key Incorrect");
            }
            var result = new List<ViewMetaData>();
            if (string.IsNullOrEmpty(blobUrl))
                return StatusCode(400, "blob url is not correct");
            try
            {
                result = _context.ViewsMetaData.AsQueryable().Where(p => p.BlobUrl == blobUrl && p.IsActive == true).ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return StatusCode(500, "An Error has occurred ");
            }
            return Ok(result);
        }

        /// <summary>
        ///   delete view count details of specific user
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("api/[controller]/changeViewerStatus")]
        public async Task<IActionResult> DeleteView(string userId, bool isActive)
        {
            try
            {
                Authenticate();
            }
            catch (Exception)
            {
                return StatusCode(401, "Api Key Incorrect");
            }

            var result = new List<ViewMetaData>();
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int uid))
                return StatusCode(400, "User Id incorrect");

            try
            {
                await _context.ViewsMetaData.UpdateManyAsync(
                    Builders<ViewMetaData>.Filter.Eq("uid", uid),
                    Builders<ViewMetaData>.Update.Set("isActive", isActive));
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "An Error has occurred ");
            }
        }

        private void Authenticate()
        {
            Microsoft.Extensions.Primitives.StringValues apiKey;
            HttpContext.Request.Headers.TryGetValue("Authorization", out apiKey);
            if (apiKey.Count == 0 || !String.Equals(API_Key, apiKey[0]))
                throw new Exception("Api Key Incorrect");
        }
    }
}

