﻿using Com.Ctrip.Framework.Apollo.Exceptions;

using System.Net.Http;

namespace Com.Ctrip.Framework.Apollo.Util.Http;

public class HttpUtil : IDisposable
{
    private readonly IApolloOptions _options;

    public HttpUtil(IApolloOptions options) => _options = options;

    public Task<HttpResponse<T>> DoGetAsync<T>(Uri url) => DoGetAsync<T>(url, _options.Timeout);

    public async Task<HttpResponse<T>> DoGetAsync<T>(Uri url, int timeout)
    {
        Exception e;
        try
        {
#if NET40
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(timeout);
#else
            using var cts = new CancellationTokenSource(timeout);
#endif
            var httpClient = new HttpClient(_options.HttpMessageHandler, false)
            {
                Timeout = TimeSpan.FromMilliseconds(timeout > 0 ? timeout : _options.Timeout)
            };

            if (!string.IsNullOrWhiteSpace(_options.Secret))
                foreach (var header in Signature.BuildHttpHeaders(url, _options.AppId, _options.Secret!))
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);

            using var response = await Timeout(httpClient.GetAsync(url, cts.Token), timeout, cts).ConfigureAwait(false);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    {
#if NET40
                        var task = response.Content.ReadAsAsync<T>();
#elif NETFRAMEWORK
                        var task = response.Content.ReadAsAsync<T>(cts.Token);
#else
                        var task = response.Content.ReadFromJsonAsync<T>(
                            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DictionaryKeyPolicy = JsonNamingPolicy.CamelCase }, cts.Token);
#endif
                        return new(response.StatusCode, await task.ConfigureAwait(false));
                    }
                case HttpStatusCode.NotModified:
                    return new(response.StatusCode);
            }

            e = new ApolloConfigStatusCodeException(response.StatusCode, $"Get operation failed for {url}");
        }
        catch (Exception ex)
        {
            e = new ApolloConfigException("Could not complete get operation", ex);
        }

        throw e;
    }

    public void Dispose() => _options.Dispose();

    private static async Task<T> Timeout<T>(Task<T> task, int millisecondsDelay, CancellationTokenSource cts)
    {
#if NET40
        if (await TaskEx.WhenAny(task, TaskEx.Delay(millisecondsDelay, cts.Token)).ConfigureAwait(false) == task)
#else
        if (await Task.WhenAny(task, Task.Delay(millisecondsDelay, cts.Token)).ConfigureAwait(false) == task)
#endif
            return task.Result;

        cts.Cancel();

        throw new TimeoutException();
    }
}
