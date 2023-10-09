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
        private static string[] Articles = new[]
        {
            "Call me when you get home",
            "He is forever complaining about this country",
            "If you cannot make it, call ME as soon as possible",
            "These are a few frequently asked questions about online courses."
        };

        private static List<KeyValuePair<string, string>> docTable = new List<KeyValuePair<string, string>>();

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

            foreach (var kvp in docTable)
            {
                if (ContainsAllWords(kvp.Key, searchWords))
                {
                    foundKeys.Add(kvp.Key);
                }
            }

            if (foundKeys.Count == 0)
            {
                return NotFound($"No occurrences of '{srchtxt}' found.");
            }

            return Ok(foundKeys.Distinct());
        }

        private void ClearAndPopulateDocTable()
        {
            docTable.Clear();

            for (int i = 0; i < Articles.Length; i++)
            {
                string[] words = Articles[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < words.Length; j++)
                {
                    string key = $"Doc{i}"; // Key format: "Doc" + index
                    string value = words[j].TrimEnd('.'); // Word from the article
                    docTable.Add(new KeyValuePair<string, string>(key, value.ToLower()));
                }
            }
        }

        private bool ContainsAllWords(string docKey, List<string> needles)
        {
            int foundCount = 0;

            foreach (string needle in needles)
            {
                if (docTable.Any(word => word.Key == docKey && word.Value == needle))
                {
                    foundCount++;
                }
            }

            return foundCount == needles.Count;
        }
    }
}
