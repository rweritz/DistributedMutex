using Medallion.Threading;
using Medallion.Threading.FileSystem;
using Medallion.Threading.Postgres;

var id = Guid.NewGuid();
var @lock = GetPostgresLock();

while (true)
{
    await using (var handle = await @lock.TryAcquireAsync())
    {
        if (handle != null)
        {
            while (true)
            {
                Console.WriteLine($"[{DateTime.Now}][{id}] I have the lock");
                Thread.Sleep(1000);
            }
        }
    }
    Console.WriteLine($"[{DateTime.Now}][{id}] I don't have the lock");
    Thread.Sleep(1000);
}

static IDistributedLock GetFileLock()
{
    var lockFileDirectory = new DirectoryInfo(@"D:\_lock");
    return new FileDistributedLock(lockFileDirectory, "MyLockName");
}

static IDistributedLock GetPostgresLock()
{
    const string connectionString = "User ID=postgres;Password=my-password;Host=localhost;Port=5432;Database=postgres;";
    return new PostgresDistributedLock(
        new PostgresAdvisoryLockKey("MyLockName", allowHashing: true), connectionString);
}