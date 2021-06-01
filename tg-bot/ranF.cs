using System;
using System.Collections.Generic;

namespace tg_bot
{
    public class ranF
    {
        public data data { get; set; }

    }
    public class data
    {
        public movie movie { get; set; }
    }
    public class movie
    {
        public int id { get; set; }
        public string Url { get; set; }
        public string title_long { get; set; }
        public int year { get; set; }
        public double rating { get; set; }
        public int runtime { get; set; }
        public List<string> genres { get; set; }
        public int like_count { get; set; }
        public string description_full { get; set; }
        public string language { get; set; }
        public string large_cover_image { get; set; }

    }
}
