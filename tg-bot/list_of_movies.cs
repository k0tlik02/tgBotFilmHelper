using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tg_bot
{
    public class list_of_movies
        {
        public Data Data { get; set; }
    }
    public class Data
    {
        public int Limit { get; set; }
        public List<Movies> Movies { get; set; }
    }
    public class Movies
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title_long { get; set; }
        public int Year { get; set; }
        public double Rating { get; set; }
        public int Runtime { get; set; }
        public List<string> Genres { get; set; }
        public string Description_full { get; set; }
        public string Large_cover_image { get; set; }
        public string Language { get; set; }
    }
}