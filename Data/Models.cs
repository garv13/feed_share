using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using SelfDrvn.Feeds.Share.Configs;
using SelfDrvn.Feeds.Share.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Authentication;

namespace SelfDrvn.Feeds.Share.Data
{
    public interface IPostShareContext
    {
        IMongoCollection<Post> Posts { get; }
        IMongoCollection<ViewMetaData> ViewsMetaData { get; }
    }


    public class PostShareContext : IPostShareContext
    {
        private readonly IMongoDatabase _db;
        public PostShareContext(AppSettings settings)
        {
            string connectionString = DockerTools.UseDockerizeConnectionString(settings);
            Console.WriteLine($"Connecting to MongoDB using connectionstring: {connectionString}");
            MongoClientSettings mongoSettings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            if (connectionString.Contains("ssl=true"))
            {
                mongoSettings.SslSettings =
                              new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            }
            var client = new MongoClient(mongoSettings);
            _db = client.GetDatabase(DockerTools.UseDockerizeDatabaseName(settings));

        }
        public IMongoCollection<Post> Posts => _db.GetCollection<Post>(Post.DOCUMENT_NAME); 
        public IMongoCollection<ViewMetaData> ViewsMetaData => _db.GetCollection<ViewMetaData>(ViewMetaData.DOCUMENT_NAME);
    }

    public class Post
    {
        public const string DOCUMENT_NAME = "posts";
        public ObjectId Id { get; set; }
        [Required]
        [BsonElement("feedId")]
        public int FeedId { get; set; }
        [Required]
        [BsonElement("uniqueId")]
        public string UniqueId { get; set; }
        [Required]
        [BsonElement("type")]
        public string Type { get; set; }
        [Required]
        [BsonElement("originalFeedUrl")]
        public string OriginalFeedUrl { get; set; }
        [Required]
        [BsonElement("title")]
        public string Title { get; set; }
        [Required]
        [BsonElement("content")]
        public string Content { get; set; }
        [BsonElement("feedImageUrl")]
        public string FeedImageUrl { get; set; }
        [Required]
        [BsonElement("createdBy")]
        public string CreatedBy { get; set; }
        [Required]
        [BsonElement("createdDate")]
        public DateTime CreatedDate { get; set; }
        [Required]
        [BsonElement("creatorImageUrl")]
        public string CreatorImageUrl { get; set; }
        [Required]
        [BsonElement("organization")]
        public string Organization { get; set; }
        [Required]
        [BsonElement("organizationLogoUrl")]
        public string OrganizationLogoUrl { get; set; }
    }

    public class ViewMetaData
    {
        public const string DOCUMENT_NAME = "view_metadata";
        public ObjectId Id { get; set; }
        [Required]
        [BsonElement("bloburl")]
        public string BlobUrl { get; set; }
        
        [BsonElement("uid")] 
        public int UserId { get; set; } 
        
        [BsonElement("date")]
        public DateTime DateViewed { get; set; } 
      
        [BsonElement("isUtc")]
        public bool IsUtc { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }

    }
}