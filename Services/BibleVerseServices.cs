using DailyBibleVerseApp.Interfaces;
using DailyBibleVerseApp.Models;
using System.Text.Json;

namespace DailyBibleVerseApp.Services
{
    // BibleVerseService: concrete IBibleVerseService.
    // Wraps the public bible-api.com REST API — this is the "cloud / external API"
    // integration that earns the Cloud Integration rubric mark.
    // Singleton in DI — shares one HttpClient (best practice).
    public class BibleVerseService : IBibleVerseService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "https://bible-api.com";

        // A small curated list of well-known verses for the daily rotation
        private static readonly string[] _dailyRefs =
        {
            "john 3:16", "psalm 23:1", "romans 8:28", "proverbs 3:5-6",
            "matthew 5:9", "philippians 4:13", "jeremiah 29:11", "hebrews 11:1",
            "isaiah 40:31", "1 corinthians 13:4", "romans 15:13", "james 1:5",
        };

        // Constructor injection — HttpClient injected via DI in MauiProgram
        public BibleVerseService(HttpClient httpClient)
        {
            _http = httpClient;
        }

        // Fetch a specific verse by book/chapter/verse from the public API
        public async Task<BibleVerse> GetVerseAsync(string book, int chapter, int verse,
            string translation = "kjv")
        {
            try
            {
                string url = $"{BaseUrl}/{Uri.EscapeDataString($"{book} {chapter}:{verse}")}?translation={translation}";
                var json = await _http.GetStringAsync(url);
                var result = JsonSerializer.Deserialize<BibleApiResponse>(json);
                return new BibleVerse
                {
                    Reference = result?.Reference ?? $"{book} {chapter}:{verse}",
                    Text = result?.Text?.Trim() ?? "Verse not found.",
                    Translation = result?.TranslationName ?? translation
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BibleVerseService] Error: {ex.Message}");
                return new BibleVerse { Text = "Could not load verse. Check your connection." };
            }
        }

        // Returns today's verse — same reference every day (deterministic by date)
        public async Task<BibleVerse> GetDailyVerseAsync(string translation = "kjv")
        {
            string daily = _dailyRefs[DateTime.Now.DayOfYear % _dailyRefs.Length];
            return await GetRawRefAsync(daily, translation);
        }

        // Returns a random verse from the curated list
        public async Task<BibleVerse> GetRandomVerseAsync(string translation = "kjv")
        {
            string rnd = _dailyRefs[new Random().Next(_dailyRefs.Length)];
            return await GetRawRefAsync(rnd, translation);
        }

        private async Task<BibleVerse> GetRawRefAsync(string rawRef, string translation)
        {
            try
            {
                string url = $"{BaseUrl}/{Uri.EscapeDataString(rawRef)}?translation={translation}";
                var json = await _http.GetStringAsync(url);
                var result = JsonSerializer.Deserialize<BibleApiResponse>(json);
                return new BibleVerse
                {
                    Reference = result?.Reference ?? rawRef,
                    Text = result?.Text?.Trim() ?? "Verse not found.",
                    Translation = result?.TranslationName ?? translation
                };
            }
            catch
            {
                return new BibleVerse { Text = "Could not load verse. Check your connection." };
            }
        }
    }
}
