﻿// Json Mapping Automatically Generated By JsonToolkit Library for C#
// Diego Trinciarelli 2011
// To use this code you will need to reference Newtonsoft's Json Parser, downloadable from codeplex.
// http://json.codeplex.com/
// 

using System;
using Newtonsoft.Json;

namespace S1Parser
{

    public class DZMyFavorite
    {

        public string Version;
        public string Charset;
        public MyFavoriteVariables Variables;

        //Empty Constructor
        public DZMyFavorite()
        {
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static DZMyFavorite FromJson(string json)
        {
            return JsonConvert.DeserializeObject<DZMyFavorite>(json);
        }
    }

    public class FavoriteList : IThreadListItem
    {

        public string Favid;
        public string Uid;
        public string Idtype;
        public string Spaceuid;
        public string Icon;
        public string Url;

        public string Id { get; set; }
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Subtle { get; set; }

        [JsonProperty("dateline"), JsonConverter(typeof (DateTimeConverter))]
        public DateTime AuthorDate { get; set; }

        public string Author { get; set; }

        public string LastPoster { get; set; }

        public DateTime LastPostDate { get; set; }

        //Empty Constructor
        public FavoriteList()
        {
        }

    }

    public class MyFavoriteVariables : IThreadList
    {

        public string Cookiepre;
        public string Auth;
        public string Saltkey;
        public string Member_uid;
        public string Member_username;
        public string Groupid;
        public string Formhash;
        public object Ismoderator;
        public string Readaccess;
        public int Count;

        public FavoriteList[] List;

        [JsonProperty("perpage")]
        public int ItemsPerPage { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPage
        {
            get { return Count/ItemsPerPage; }
        }

        public IThreadListItem[] ThreadList
        {
            get { return List; }
        }

        //Empty Constructor
        public MyFavoriteVariables()
        {
        }

    }

}

//Json Mapping End
