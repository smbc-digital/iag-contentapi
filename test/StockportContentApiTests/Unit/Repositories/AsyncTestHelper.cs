using Contentful.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class AsyncTestHelper
{
    private const int Timeout = 50;

    public static T Resolve<T>(Task<T> task)
    {
        task.Wait(Timeout);
        return task.Result;
    }
}