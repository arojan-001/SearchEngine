using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchEngine.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private static List<string> Articles = new List<string>
        {
            "Call me when you get home",
            "He is forever complaining about this country",
            "If you cannot make it, call ME as soon as possible",
            "These are a few frequently asked questions about online courses."
        };

        private static List<KeyValuePair<string, string>> docTable = new List<KeyValuePair<string, string>>();

        [HttpPost("addarticle")]
        public ActionResult<IEnumerable<string>> AddArticle(string article)
        {
            Articles.Add(article);

            return Ok(Articles);
        }


        [HttpGet("generate")]
        public ActionResult<IEnumerable<KeyValuePair<string, string>>> GenerateKeyValues()
        {
            ClearAndPopulateDocTable();

            return Ok(docTable);
        }

    [HttpGet("search")]
    public ActionResult<IEnumerable<string>> SearchText(string srchtxt)
    {
        if (string.IsNullOrEmpty(srchtxt))
        {
            return BadRequest("Search text cannot be empty.");
        }

        List<string> searchWords = srchtxt.ToLower().Split(' ').ToList();
        List<string> foundKeys = new List<string>();

        foreach (var docKey in docTable.Select(kvp => kvp.Key).Distinct())
        {
            if (ContainsAllWords(docKey, searchWords))
            {
                foundKeys.Add(docKey);
            }
        }

        if (foundKeys.Count == 0)
        {
            foundKeys.Add($"No occurrences of '{srchtxt}' found.");
            return Ok(foundKeys);
        }

        return Ok(foundKeys.Distinct());
    }

    private void ClearAndPopulateDocTable()
    {
        docTable.Clear();

        for (int i = 0; i < Articles.Count; i++)
        {
            string[] words = Articles[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < words.Length; j++)
            {
                string key = $"Doc{i + 1}"; // Key format: "Doc" + index
                string value = words[j].TrimEnd('.'); // Word from the article
                docTable.Add(new KeyValuePair<string, string>(key, value.ToLower()));
            }
        }
    }

    private bool ContainsAllWords(string docKey, List<string> searchWords)
    {
        int foundCount = 0;

        foreach (string searchWord in searchWords)
        {
            if (docTable.Any(word => word.Key == docKey && word.Value == searchWord))
            {
                foundCount++;
            }
        }

        return foundCount == searchWords.Count;
    }
}
}
