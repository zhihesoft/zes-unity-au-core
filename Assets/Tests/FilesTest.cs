using Au;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

internal class FilesTest
{
    [Test]
    public async Task TestFiles()
    {
        DirectoryInfo dir1 = new DirectoryInfo("Temp/FileTests");
        DirectoryInfo dir2 = new DirectoryInfo("Temp/FileTests2");
        Files.EnsureDir(dir1);
        if (dir2.Exists)
        {
            dir2.Delete(); // make sure dir2 not exists
        }

        FileInfo file1 = new FileInfo(Path.Combine(dir1.FullName, "test.txt"));
        FileInfo file2 = new FileInfo(Path.Combine(dir2.FullName, "test.txt"));

        string text = "Hello world";
        Files.Save(file1.FullName, text);

        Files.CopyDir(dir1, dir2);
        Assert.IsTrue(dir2.Exists, "dir2 should exists after copy");
        Assert.IsTrue(file2.Exists, "file2 should exists after copy");
        var txt = await Files.Read(file2.FullName);
        Assert.AreEqual(txt, text, "use local file path to read file2 content");
        var txt2 = await Files.Read("file://" + file2.FullName);
        Assert.AreEqual(txt2, text, "use file:// protocol to read file2 content");

        Files.ClearDir(dir1);
        Files.ClearDir(dir2);

        file2 = new FileInfo(Path.Combine(dir2.FullName, "test.txt"));
        Assert.IsFalse(file1.Exists, "file still return exists after clear dir1");
        Assert.IsFalse(file2.Exists, "file2 still return exists after clear dir2");

        dir1.Delete();
        dir2.Delete();
    }
}
