using System;
using System.IO;
using System.Net;
using System.Text;

namespace Go.Tools
{
	public class UrlTools
	{
		public static string GetUrl(string url)
		{
            //if (url.EndsWith("/"))
            //    url = url.Substring(0, url.Length - 1);

			return GetUrlContents(url);
		}

		private static string GetUrlContents(string url, int readTimeout = 500)
		{
            try
            {
                var html = string.Empty;
                using (var outputStream = new MemoryStream())
                {
#if true
                    var webRequest = (HttpWebRequest)WebRequest.Create(url);
                    webRequest.KeepAlive = false;
                    webRequest.ProtocolVersion = HttpVersion.Version10;
#else
                    var webRequest = WebRequest.Create(url);
#endif
                    using (var webResponse = webRequest.GetResponse())
                    {
                        var receiveStream = webResponse.GetResponseStream();
                        if (receiveStream != null)
                        {
                            receiveStream.ReadTimeout = readTimeout;
                            receiveStream.CopyTo(outputStream);
                        }
                    }

                    // Check for GZipped content.
                    var leadingBytes = new byte[3];
                    outputStream.Position = 0;
                    var bytesRead = outputStream.Read(leadingBytes, 0, leadingBytes.Length);
                    outputStream.Position = 0;
                    if (bytesRead == leadingBytes.Length && GZipTools.IsGZip(leadingBytes))
                    {
                        string unzippedString;
                        if (GZipTools.UnGZip(outputStream, Encoding.UTF8, out unzippedString))
                            html = unzippedString;
                    }
                    else
                    {
                        html = StreamTools.GetStringFromStream(outputStream, Encoding.UTF8);
                    }
                }

                return html;
            }
            catch (WebException ex)
            {
                if (ex.Response == null && ex.HResult == -2146233079)
                    return "500: " + ex.Message;

                var response = (HttpWebResponse)ex.Response;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Accepted:
                            return "202: Accepted: The request has been accepted for further processing.";
                        //case HttpStatusCode.Ambiguous:
                        //	return "300: Ambiguous: The requested information has multiple representations. The default action is to treat this status as a redirect and follow the contents of the Location header associated with this response.";
                        case HttpStatusCode.BadGateway:
                            return "502: BadGateway: An intermediate proxy server received a bad response from another proxy or the origin server.";
                        case HttpStatusCode.BadRequest:
                            return "400: BadRequest: The request could not be understood by the server. System.Net.HttpStatusCode.BadRequest is sent when no other error is applicable, or if the exact error is unknown or does not have its own error code.";
                        case HttpStatusCode.Conflict:
                            return "409: Conflict: The request could not be carried out because of a conflict on the server.";
                        case HttpStatusCode.Continue:
                            return "100: Continue: The client can continue with its request.";
                        case HttpStatusCode.Created:
                            return "201: Created: The request resulted in a new resource created before the response was sent.";
                        case HttpStatusCode.ExpectationFailed:
                            return "417: ExpectationFailed: An expectation given in an Expect header could not be met by the server.";
                        case HttpStatusCode.Forbidden:
                            return "403: Forbidden: The server refuses to fulfill the request.";
                        case HttpStatusCode.Found:
                            return "302: Found: The requested information is located at the URI specified in the Location header. The default action when this status is received is to follow the Location header associated with the response. When the original request method was POST, the redirected request will use the GET method.";
                        case HttpStatusCode.GatewayTimeout:
                            return "504: GatewayTimeout: An intermediate proxy server timed out while waiting for a response from another proxy or the origin server.";
                        case HttpStatusCode.Gone:
                            return "410: Gone: The requested resource is no longer available.";
                        case HttpStatusCode.HttpVersionNotSupported:
                            return "505: HttpVersionNotSupported: The requested HTTP version is not supported by the server.";
                        case HttpStatusCode.InternalServerError:
                            return "500: InternalServerError: A generic error has occurred on the server.";
                        case HttpStatusCode.LengthRequired:
                            return "411: LengthRequired: The required Content-length header is missing.";
                        case HttpStatusCode.MethodNotAllowed:
                            return "405: MethodNotAllowed: The request method (POST or GET) is not allowed on the requested resource.";
                        //case HttpStatusCode.Moved:
                        //	return "301: Moved: The requested information has been moved to the URI specified in the Location header. The default action when this status is received is to follow the Location header associated with the response. When the original request method was POST, the redirected request will use the GET method.";
                        case HttpStatusCode.MovedPermanently:
                            return "301: MovedPermanently: The requested information has been moved to the URI specified in the Location header. The default action when this status is received is to follow the Location header associated with the response.";
                        case HttpStatusCode.MultipleChoices:
                            return "300: MultipleChoices: The requested information has multiple representations. The default action is to treat this status as a redirect and follow the contents of the Location header associated with this response.";
                        case HttpStatusCode.NoContent:
                            return "204: NoContent: The request has been successfully processed and that the response is intentionally blank.";
                        case HttpStatusCode.NonAuthoritativeInformation:
                            return "203: NonAuthoritativeInformation: The returned metainformation is from a cached copy instead of the origin server and therefore may be incorrect.";
                        case HttpStatusCode.NotAcceptable:
                            return "406: NotAcceptable: The client has indicated with Accept headers that it will not accept any of the available representations of the resource.";
                        case HttpStatusCode.NotFound:
                            return "404: The requested resource does not exist on the server.";
                        case HttpStatusCode.NotImplemented:
                            return "501: NotImplemented: The server does not support the requested function.";
                        case HttpStatusCode.NotModified:
                            return "304: NotModified: The client's cached copy is up to date. The contents of the resource are not transferred.";
                        case HttpStatusCode.OK:
                            return "200: OK: The request succeeded and that the requested information is in the response. This is the most common status code to receive.";
                        case HttpStatusCode.PartialContent:
                            return "206: PartialContent: The response is a partial response as requested by a GET request that includes a byte range.";
                        case HttpStatusCode.PaymentRequired:
                            return "402: PaymentRequired: Reserved for future use.";
                        case HttpStatusCode.PreconditionFailed:
                            return "412: PreconditionFailed: A condition set for this request failed, and the request cannot be carried out. Conditions are set with conditional request headers like If-Match, If-None-Match, or If-Unmodified-Since.";
                        default:
                            return string.Format("{0}: {1}", response.StatusCode, response.StatusDescription);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                return "500: " + ex.Message;
            }
		}
	}
}
