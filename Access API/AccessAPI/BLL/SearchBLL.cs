﻿using System;
using Access_API.DAL;

namespace Access_API.BLL
{
    public class SearchBLL
    {
        public string SearchBll(string words, int? sourceId, string? author, int? categoryId,
            DateTime? beforeDate, DateTime? afterDate)
        {
            string url = Urls.SearchUrl + $"/search?words={words}";
            if (sourceId is not null) url += $"&sourceId={sourceId}";
            if (author is not null) url += $"&author={author}";
            if (categoryId is not null) url += $"&categoryId={categoryId}";
            if (beforeDate is not null) url += $"&beforeDate={beforeDate:o}";
            if (afterDate is not null) url += $"&afterDate={afterDate:o}";

            SearchDAL dal = new SearchDAL();

            return dal.GetSearchResults(url);
        }
    }
}
