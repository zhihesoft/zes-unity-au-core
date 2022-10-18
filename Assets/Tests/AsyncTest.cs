using Au;
using NUnit.Framework;
using System.Threading.Tasks;
using UnityEngine.Networking;

internal class AsyncTest
{
    [Test]
    public async Task TestForWaitUntil()
    {
        await Async.WaitUntil(() => true);
    }

    [Test]
    public async Task TestForWaitAsyncOp()
    {
        var req = UnityWebRequest.Get("http://www.baidu.com");
        await Async.WaitAsyncOperation(req.SendWebRequest());
    }
}

