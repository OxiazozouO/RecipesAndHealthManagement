using Android.ViewModel;
using HtmlAgilityPack;

namespace Android.Helper;

public static class HtmlHelper
{
    public static string GetHtml(string html, out List<string> images, out Dictionary<int, HashSet<int>> mp)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);
        images = [];
        List<int> ids = [];
        List<int> tags = [];


        var imgs = doc.DocumentNode.SelectNodes("//img[@class='insert-img']");
        if (imgs != null)
        {
            foreach (var img in imgs)
            {
                string dataFlag = img.GetAttributeValue("data-flag", "");
                images.Add(dataFlag);
                img.Attributes.Remove("data-flag");
                img.SetAttributeValue("data-flag", $"{images.Count - 1}");
                img.Attributes.Remove("src");
                img.Attributes.Remove("alt");
                img.Attributes.Remove("width");
                img.Attributes.Remove("contenteditable");
            }
        }

        var tgs = doc.DocumentNode.SelectNodes("//div[@class='tag']");
        if (tgs != null)
        {
            foreach (var tg in tgs)
            {
                tg.Attributes.Remove("contenteditable");
                string dataFlag = tg.GetAttributeValue("data-flag", "");

                if (!dataFlag.Contains('_')) continue;
                tg.InnerHtml = "";
                var arr = dataFlag.Split('_');
                tags.Add(int.Parse(arr[0]));
                ids.Add(int.Parse(arr[1]));
            }
        }

        var ass = doc.DocumentNode.SelectNodes("//a");
        if (ass != null)
            foreach (var a in ass)
                a.Attributes.Remove("contenteditable");

        mp = new Dictionary<int, HashSet<int>>();

        for (var i = 0; i < ids.Count; i++)
        {
            var id = ids[i];
            var idc = tags[i];
            if (mp.TryGetValue(idc, out var list))
                list.Add(id);
            else mp[idc] = [id];
        }

        return doc.DocumentNode.OuterHtml;
    }

    public static string SetHtml(string html, List<CollectionImageModel>? images, List<CollectionTabModel>? tabs)
    {
        if (string.IsNullOrEmpty(html) || tabs is null || images is null) return null;
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var imgs = doc.DocumentNode.SelectNodes("//img[@class='insert-img']");
        if (imgs != null)
        {
            foreach (var tg in imgs)
            {
                int dataFlag = int.Parse(tg.GetAttributeValue("data-flag", ""));
                tg.SetAttributeValue("data-flag", images[dataFlag].Id);
                tg.SetAttributeValue("src", images[dataFlag].Url);
                tg.SetAttributeValue("alt", "");
                tg.SetAttributeValue("width", "400");
                tg.SetAttributeValue("contenteditable", "false");
            }
        }

        var tgs = doc.DocumentNode.SelectNodes("//div[@class='tag']");
        if (tgs != null)
        {
            var tbs = tabs.ToDictionary(t => $"{t.IdCategory}_{t.Id}", t => t);
            foreach (var tg in tgs)
            {
                string dataFlag = tg.GetAttributeValue("data-flag", "");
                if (tbs.TryGetValue(dataFlag, out var t))
                {
                    tg.InnerHtml = $"""
                                    <img src = "{t.FileUrl}">
                                    <div class ="tag-content">
                                       <div class ="tag-title">{t.Title}</div>
                                       <div class ="tag-refer">{t.Refer}</div>
                                    </div>
                                    """;
                }
            }
        }

        return doc.DocumentNode.OuterHtml;
    }
}