using Streamish.Models;
using System;
using System.Collections.Generic;

namespace Streamish.Repositories
{
    public interface IVideoRepository
    {
        void Add(Video video);
        void Delete(int id);
        List<Video> GetAll();
        List<Video> GetAllWithComments();
        Video GetVideoByIdWithComments(int id);
        Video GetById(int id);
        void Update(Video video);
        List<Video> Search(string criterion, bool sortDescending);
        List<Video> SearchAll(string titleq, string descriptionq, DateTime? dateq, int? userq, bool sortDescending);
        List<Video> Hottest(DateTime since);
    }
}