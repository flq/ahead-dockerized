using System.Web;

namespace Ahead.Web.Infrastructure;

public static class ResultExtensions
{
    public static IResult BackToHomeWithMessage(this IResultExtensions _, string message) =>
        Results.LocalRedirect($"~/Index?message={HttpUtility.UrlEncode(message)}");
}