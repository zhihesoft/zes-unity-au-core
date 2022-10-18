using Au;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

internal class AssetSetTest
{
    [Test]
    public async Task TestForAssetSet()
    {
        AssetSet ass = new AssetSet("");
        await ass.LoadBundle("languages", null);
        var obj = await ass.LoadObject(
            Path.Combine("Assets", "Bundles", "languages", "i18n-zh-cn.json"),
            typeof(TextAsset));
        Assert.IsNotNull(obj);
    }
}
