using Microsoft.AspNetCore.Mvc;

namespace TestHelpers.Extensions;

public static class ActionResultExtensions
{
    public static T GetObjectResultContent<T>(this ActionResult<T> result)
    {
        return (T) ((ObjectResult) result.Result).Value;
    }
}
