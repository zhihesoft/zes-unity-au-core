using Au;
using NUnit.Framework;
using UnityEngine;

internal class TagsTest
{
    [Test]
    public void TestForTags()
    {
        var go = new GameObject();
        Tags tags = go.AddComponent<Tags>();
        tags.items = new Tag[] {
            new Tag{ name = "test1", target = new GameObject() },
            new Tag{ name = "test2", target = new GameObject() },
            new Tag{ name = "test3", target = new GameObject() },
        };
        Assert.NotNull(tags.Get("test1"));
        Assert.NotNull(tags.Get("test2"));
        Assert.NotNull(tags.Get("test3"));
        Assert.IsNull(tags.Get("none1"));

        Object.Destroy(go);
    }
}
