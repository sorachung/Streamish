using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Models;
using Streamish.Utils;

namespace Streamish.Repositories
{

    public class VideoRepository : BaseRepository, IVideoRepository
    {
        public VideoRepository(IConfiguration configuration) : base(configuration) { }

        public List<Video> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
               SELECT v.Id AS VideoId, v.Title, v.Description, v.Url, v.DateCreated AS VideoDateCreated, v.UserProfileId AS VideoUserProfileId,

                      up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                      up.ImageUrl AS UserProfileImageUrl
                        
                 FROM Video v 
                      JOIN UserProfile up ON v.UserProfileId = up.Id
             ORDER BY v.DateCreated
            ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        var videos = new List<Video>();
                        while (reader.Read())
                        {
                            videos.Add(MakeNewVideoFromReader(reader));
                        }

                        return videos;
                    }
                }
            }
        }

        public List<Video> GetAllWithComments()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT v.Id AS VideoId, v.Title, v.Description, v.Url, 
                       v.DateCreated AS VideoDateCreated, v.UserProfileId As VideoUserProfileId,

                       up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                       up.ImageUrl AS UserProfileImageUrl,
                        
                       c.Id AS CommentId, c.Message, c.UserProfileId AS CommentUserProfileId
                  FROM Video v 
                       JOIN UserProfile up ON v.UserProfileId = up.Id
                       LEFT JOIN Comment c on c.VideoId = v.id
             ORDER BY  v.DateCreated
            ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        var videos = new List<Video>();
                        while (reader.Read())
                        {
                            var videoId = DbUtils.GetInt(reader, "VideoId");

                            var existingVideo = videos.FirstOrDefault(p => p.Id == videoId);
                            if (existingVideo == null)
                            {
                                existingVideo = MakeNewVideoFromReader(reader);

                                videos.Add(existingVideo);
                            }

                            if (DbUtils.IsNotDbNull(reader, "CommentId"))
                            {
                                existingVideo.Comments.Add(new Comment()
                                {
                                    Id = DbUtils.GetInt(reader, "CommentId"),
                                    Message = DbUtils.GetString(reader, "Message"),
                                    VideoId = videoId,
                                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
                                });
                            }
                        }

                        return videos;
                    }
                }
            }
        }

        public Video GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                          SELECT v.Id AS VideoId, Title, Description, Url, v.DateCreated AS VideoDateCreated, UserProfileId AS VideoUserProfileId,

                                    up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                                    up.ImageUrl AS UserProfileImageUrl
                            FROM Video v
                                 JOIN UserProfile up ON v.UserProfileId = up.Id
                           WHERE v.Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        Video video = null;
                        if (reader.Read())
                        {
                            video = MakeNewVideoFromReader(reader);
                        }

                        return video;
                    }
                }
            }
        }

        public Video GetVideoByIdWithComments(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT v.Id AS VideoId, v.Title, v.Description, v.Url, 
                       v.DateCreated AS VideoDateCreated, v.UserProfileId As VideoUserProfileId,

                       up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                       up.ImageUrl AS UserProfileImageUrl,
                        
                       c.Id AS CommentId, c.Message, c.UserProfileId AS CommentUserProfileId
                  FROM Video v 
                       JOIN UserProfile up ON v.UserProfileId = up.Id
                       LEFT JOIN Comment c on c.VideoId = v.id
                  WHERE v.Id = @Id
            ";
                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        
                        Video video = null;
                        while (reader.Read())
                        {
                            if (video == null)
                            {
                                video = MakeNewVideoFromReader(reader);
                            }

                            if (DbUtils.IsNotDbNull(reader, "CommentId"))
                            {
                                video.Comments.Add(new Comment()
                                {
                                    Id = DbUtils.GetInt(reader, "CommentId"),
                                    Message = DbUtils.GetString(reader, "Message"),
                                    VideoId = DbUtils.GetInt(reader, "VideoId"),
                                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
                                });
                            }
                        }

                        return video;
                    }
                }
            }
        }

        public void Add(Video video)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Video (Title, Description, DateCreated, Url, UserProfileId)
                        OUTPUT INSERTED.ID
                        VALUES (@Title, @Description, @DateCreated, @Url, @UserProfileId)";

                    DbUtils.AddParameter(cmd, "@Title", video.Title);
                    DbUtils.AddParameter(cmd, "@Description", video.Description);
                    DbUtils.AddParameter(cmd, "@DateCreated", video.DateCreated);
                    DbUtils.AddParameter(cmd, "@Url", video.Url);
                    DbUtils.AddParameter(cmd, "@UserProfileId", video.UserProfileId);

                    video.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(Video video)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE Video
                           SET Title = @Title,
                               Description = @Description,
                               DateCreated = @DateCreated,
                               Url = @Url,
                               UserProfileId = @UserProfileId
                         WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Title", video.Title);
                    DbUtils.AddParameter(cmd, "@Description", video.Description);
                    DbUtils.AddParameter(cmd, "@DateCreated", video.DateCreated);
                    DbUtils.AddParameter(cmd, "@Url", video.Url);
                    DbUtils.AddParameter(cmd, "@UserProfileId", video.UserProfileId);
                    DbUtils.AddParameter(cmd, "@Id", video.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Video WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Video> Search(string criterion, bool sortDescending)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    var sql = @"
              SELECT v.Id  AS VideoId, v.Title, v.Description, v.Url, v.DateCreated AS VideoDateCreated, v.UserProfileId AS VideoUserProfileId,

                     up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                     up.ImageUrl AS UserProfileImageUrl
                        
                FROM Video v 
                     JOIN UserProfile up ON v.UserProfileId = up.Id
               WHERE v.Title LIKE @Criterion OR v.Description LIKE @Criterion";

                    if (sortDescending)
                    {
                        sql += " ORDER BY v.DateCreated DESC";
                    }
                    else
                    {
                        sql += " ORDER BY v.DateCreated";
                    }

                    cmd.CommandText = sql;
                    DbUtils.AddParameter(cmd, "@Criterion", $"%{criterion}%");
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        var videos = new List<Video>();
                        while (reader.Read())
                        {
                            videos.Add(MakeNewVideoFromReader(reader));
                        }

                        return videos;
                    }
                }
            }
        }

        public List<Video> SearchAll(string titleq, string descriptionq, DateTime? dateq, int? userq, bool sortDescending)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    var sql = @"
              SELECT v.Id  AS VideoId, v.Title, v.Description, v.Url, v.DateCreated AS VideoDateCreated, v.UserProfileId AS VideoUserProfileId,

                     up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                     up.ImageUrl AS UserProfileImageUrl
                        
                FROM Video v 
                     JOIN UserProfile up ON v.UserProfileId = up.Id
               WHERE";

                    sql += titleq == null ? "" : " v.Title LIKE @titleq AND ";
                    sql += descriptionq == null ? "" : " v.Description LIKE @descriptionq AND ";
                    sql += dateq == null ? "" : " v.DateCreated > @dateq AND ";
                    sql += userq == null ? "" : " v.UserProfileId = @userq ";

                    if (userq == null && sql.EndsWith("AND "))
                    {
                        sql = sql.Substring(0, sql.LastIndexOf(" AND")) + " ";
                    }

                    if (sortDescending)
                    {
                        sql += " ORDER BY v.DateCreated DESC";
                    }
                    else
                    {
                        sql += " ORDER BY v.DateCreated";
                    }

                    cmd.CommandText = sql;

                    if (titleq != null) DbUtils.AddParameter(cmd, "@titleq", $"%{titleq}%");

                    if(descriptionq != null) DbUtils.AddParameter(cmd, "@descriptionq", $"%{descriptionq}%");

                    if (dateq != null) DbUtils.AddParameter(cmd, "@dateq", dateq);

                    if (userq != null) DbUtils.AddParameter(cmd, "@userq", userq);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        var videos = new List<Video>();
                        while (reader.Read())
                        {
                            videos.Add(MakeNewVideoFromReader(reader));
                        }

                        return videos;
                    }
                }
            }
        }

        public List<Video> Hottest(DateTime since)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    var sql = @"
              SELECT v.Id AS VideoId, v.Title, v.Description, v.Url, v.DateCreated AS VideoDateCreated, v.UserProfileId AS VideoUserProfileId,

                     up.Name, up.Email, up.DateCreated AS UserProfileDateCreated,
                     up.ImageUrl AS UserProfileImageUrl
                        
                FROM Video v 
                     JOIN UserProfile up ON v.UserProfileId = up.Id
               WHERE v.DateCreated > @since";

                    cmd.CommandText = sql;
                    DbUtils.AddParameter(cmd, "@since", since);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        var videos = new List<Video>();
                        while (reader.Read())
                        {
                            videos.Add(MakeNewVideoFromReader(reader));
                        }

                        return videos;
                    }
                }
            }
        }

        private Video MakeNewVideoFromReader(SqlDataReader reader)
        {
            var video = new Video()
            {
                Id = DbUtils.GetInt(reader, "VideoId"),
                Title = DbUtils.GetString(reader, "Title"),
                Description = DbUtils.GetString(reader, "Description"),
                DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                Url = DbUtils.GetString(reader, "Url"),
                UserProfileId = DbUtils.GetInt(reader, "VideoUserProfileId"),
                UserProfile = new UserProfile()
                {
                    Id = DbUtils.GetInt(reader, "VideoUserProfileId"),
                    Name = DbUtils.GetString(reader, "Name"),
                    Email = DbUtils.GetString(reader, "Email"),
                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                },
                Comments = new List<Comment>()
            };

            return video;
        }
    }
}