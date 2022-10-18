using Au;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

internal class I18nTest
{
    [Test]
    public async Task TestForI18nDef()
    {
        var json = await Files.Read(Path.Combine("Assets", "Bundles", "languages", "i18n-zh-cn.json"));
        var def = new I18nData("zh-cn", json);
        Assert.AreEqual(def.name, "zh-cn");
        Assert.AreEqual(def.Translate("200020"), "英雄不存在");
    }

    [Test]
    public async Task TestForI18n()
    {
        var json = await Files.Read(Path.Combine("Assets", "Bundles", "languages", "i18n-zh-cn.json"));
        I18n.AddData(I18n.LanguageCN, json);
        var go = new GameObject();
        var txt = go.AddComponent<TextMeshProUGUI>();
        await Task.Yield();
        var i18n = go.AddComponent<I18n>();
        i18n.languageId = "18000";
        await Task.Yield();
        Assert.AreEqual(txt.text, "测试怪01");

        i18n.languageId = "700001";
        i18n.Refresh();
        Assert.AreEqual(txt.text, "700001", "700001 is not in current set, so it is id only");

        json = await Files.Read(Path.Combine("Assets", "Bundles", "languages", "i18n-en-us.json"));
        I18n.AddAddition(I18n.LanguageCN, "us", json);
        i18n.Refresh();
        Assert.AreEqual(txt.text, "xxxxxxxxx", "700001 should be set");

        I18n.RemoveData(I18n.LanguageCN);
        Object.Destroy(go);
    }
}
