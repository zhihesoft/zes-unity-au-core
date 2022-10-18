using Au;
using NUnit.Framework;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

internal class I18nTest
{
    [Test]
    public async Task TestForI18n()
    {
        I18n.translator = (id) =>
        {
            if (id == "1")
            {
                return "test";
            }
            else
            {
                return "failed";
            }
        };

        var go = new GameObject();
        var txt = go.AddComponent<TextMeshProUGUI>();
        await Task.Yield();
        var i18n = go.AddComponent<I18n>();
        i18n.languageId = "1";
        await Task.Yield();
        Assert.AreEqual(txt.text, "test");

        Object.Destroy(i18n);
        i18n = go.AddComponent<I18n>();
        i18n.languageId = "0";
        await Task.Yield();
        Assert.AreEqual(txt.text, "failed");

        Object.Destroy(go);
    }
}
