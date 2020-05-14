// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseController.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>15-4-2020 19:00</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    using Microsoft.AspNetCore.Http;
    #region Using Directives

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;

    #endregion Using Directives

    /// <summary>
    /// Base class for a MVC controller providing logger and settings data members.
    /// </summary>
    /// <typeparam name="TSettings">The settings class.</typeparam>
    public class BaseController<TSettings> : ControllerBase where TSettings : class, new()
    {
        #region Protected Data Members

        protected readonly ILogger<BaseController<TSettings>> _logger;
        protected readonly IHostApplicationLifetime _lifetime;
        protected readonly IHostEnvironment _environment;
        protected readonly IConfiguration _config;
        protected readonly TSettings _settings;

        #endregion Protected Data Members

        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="config"></param>
        /// <param name="environment"></param>
        /// <param name="lifetime"></param>
        /// <param name="logger"></param>
        public BaseController(TSettings settings,
                              IConfiguration config,
                              IHostEnvironment environment,
                              IHostApplicationLifetime lifetime,
                              ILogger<BaseController<TSettings>> logger)
        {
            _settings = settings;
            _config = config;
            _environment = environment;
            _lifetime = lifetime;
            _logger = logger;

            _logger?.LogDebug($"BaseController<{typeof(TSettings)}>()");
        }

        // HTTP status codes (* indicates supported by ControllerBase):
        // see also https://en.wikipedia.org/wiki/List_of_HTTP_status_codes
        //
        // 1xx Informational
        //      100 Continue                                                        Continue
        //      101 Switching Protocols                                             SwitchingProtocols
        //      102 Processing                                                      Processing
        //      103 Early Hints                                                     EarlyHints
        //
        // 2xx Success
        //    * 200 - OK                                    (OK, Content)           OK
        //    * 201 - Created                               (Created)               Created
        //    * 202 - Accepted                              (Accepted)              Accepted
        //      203 - Non-authoritative Information                                 NonAuthoritativeInformation
        //    * 204 - No Content                            (NoContent)             NoContent
        //      205 - Reset Content                                                 ResetContent
        //      206 - Partial Content                                               PartialContent
        //      207 - Multi-Status                                                  MultiStatus
        //      208 - Already Reported                                              AlreadyReported 
        //      226 - IM Used                                                       IMUsed
        //
        // 3xx Redirection
        //      300 - Multiple Choices                                              MultipleChoices, Ambiguous
        //      301 - Moved Permanently                                             MovedPermanently, Moved
        //    * 302 - Found (Redirect)                      (Redirect)              Found, Redirect
        //      303 - See Other                                                     SeeOther, RedirectMethod
        //      304 - Not Modified                                                  NotModified
        //      305 - Use Proxy                                                     UseProxy
        //      306 -                                                               Unused                                                    
        //      307 - Temporary Redirect                                            RedirectKeepVerb, TemporaryRedirect
        //      308 - Permanent Redirect                    (RedirectPermanent)     PermanentRedirect
        //
        // 4xx Client Error
        //    * 400 - Bad Request                           (BadRequest)            BadRequest
        //    * 401 - Unauthorized                          (Unauthorized)          Unauthorized
        //      402 - Payment Required                                              PaymentRequired
        //    * 403 - Forbidden                             (Forbid)                Forbidden
        //    * 404 - Not Found                             (NotFound)              NotFound
        //      405 - Method Not Allowed                                            MethodNotAllowed
        //      406 - Not Acceptable                                                NotAcceptable
        //      407 - Proxy Authentication Required                                 ProxyAuthenticationRequired
        //      408 - Request Timeout                                               RequestTimeout
        //    * 409 - Conflict                              (Conflict)              Conflict
        //      410 - Gone                                                          Gone
        //      411 - Length Required                                               LengthRequired
        //      412 - Precondition Failed                                           PreconditionFailed
        //      413 - Payload Too Large                                             RequestEntityTooLarge
        //      414 - Request-URI Too Long                                          RequestUriTooLong
        //      415 - Unsupported Media Type                                        UnsupportedMediaType
        //      416 - Requested Range Not Satisfiable                               RequestedRangeNotSatisfiable
        //      417 - Expectation Failed                                            ExpectationFailed
        //      418 - I'm a teapot
        //      419 - Authentication Timeout
        //      421 - Misdirected Request                                           MisdirectedRequest
        //    * 422 - Unprocessable Entity                  (UnprocessableEntity)   UnprocessableEntity
        //      423 - Locked                                                        Locked
        //      424 - Failed Dependency                                             FailedDependency
        //      425 - Too Early
        //      426 - Upgrade Required                                              UpgradeRequired
        //      428 - Precondition Required                                         PreconditionRequired
        //      429 - Too Many Requests                                             TooManyRequests
        //      431 - Request Header Fields Too Large                               RequestHeaderFieldsTooLarge
        //      444 - Connection Closed Without Response
        //      451 - Unavailable For Legal Reasons                                 UnavailableForLegalReasons
        //      499 - Client Closed Request
        //
        // 5xx Server Error
        //      500 - Internal Server Error                                         InternalServerError
        //      501 - Not Implemented                                               NotImplemented
        //      502 - Bad Gateway                                                   BadGateway
        //      503 - Service Unavailable                                           ServiceUnavailable
        //      504 - Gateway Timeout                                               GatewayTimeout
        //      505 - HTTP Version Not Supported                                    HttpVersionNotSupported
        //      506 - Variant Also Negotiates                                       VariantAlsoNegotiates
        //      507 - Insufficient Storage                                          InsufficientStorage
        //      508 - Loop Detected                                                 LoopDetected
        //      510 - Not Extended                                                  NotExtended
        //      511 - Network Authentication Required                               NetworkAuthenticationRequired
        //      599 - Network Connect Timeout Error
        //
        // Additional HTTPS status code methods are added here.

        [NonAction] public virtual StatusCodeResult PaymentRequired() => StatusCode(StatusCodes.Status402PaymentRequired);
        [NonAction] public virtual ObjectResult PaymentRequired(object value) => StatusCode(StatusCodes.Status402PaymentRequired, value);

        [NonAction] public virtual StatusCodeResult MethodNotAllowed() => StatusCode(StatusCodes.Status405MethodNotAllowed);
        [NonAction] public virtual ObjectResult MethodNotAllowed(object value) => StatusCode(StatusCodes.Status405MethodNotAllowed, value);

        [NonAction] public virtual StatusCodeResult NotAcceptable() => StatusCode(StatusCodes.Status406NotAcceptable);
        [NonAction] public virtual ObjectResult NotAcceptable(object value) => StatusCode(StatusCodes.Status406NotAcceptable, value);

        [NonAction] public virtual StatusCodeResult ProxyAuthenticationRequired() => StatusCode(StatusCodes.Status407ProxyAuthenticationRequired);
        [NonAction] public virtual ObjectResult ProxyAuthenticationRequired(object value) => StatusCode(StatusCodes.Status407ProxyAuthenticationRequired, value);

        [NonAction] public virtual StatusCodeResult RequestTimeout() => StatusCode(StatusCodes.Status408RequestTimeout);
        [NonAction] public virtual ObjectResult RequestTimeout(object value) => StatusCode(StatusCodes.Status408RequestTimeout, value);

        [NonAction] public virtual StatusCodeResult Gone() => StatusCode(StatusCodes.Status410Gone);
        [NonAction] public virtual ObjectResult Gone(object value) => StatusCode(StatusCodes.Status410Gone, value);

        [NonAction] public virtual StatusCodeResult LengthRequired() => StatusCode(StatusCodes.Status411LengthRequired);
        [NonAction] public virtual ObjectResult LengthRequired(object value) => StatusCode(StatusCodes.Status411LengthRequired, value);

        [NonAction] public virtual StatusCodeResult PreconditionFailed() => StatusCode(StatusCodes.Status412PreconditionFailed);
        [NonAction] public virtual ObjectResult PreconditionFailed(object value) => StatusCode(StatusCodes.Status412PreconditionFailed, value);

        [NonAction] public virtual StatusCodeResult PayloadTooLarge() => StatusCode(StatusCodes.Status413PayloadTooLarge);
        [NonAction] public virtual ObjectResult PayloadTooLarge(object value) => StatusCode(StatusCodes.Status413PayloadTooLarge, value);

        [NonAction] public virtual StatusCodeResult RequestEntityTooLarge() => StatusCode(StatusCodes.Status413RequestEntityTooLarge);
        [NonAction] public virtual ObjectResult RequestEntityTooLarge(object value) => StatusCode(StatusCodes.Status413RequestEntityTooLarge, value);

        [NonAction] public virtual StatusCodeResult RequestUriTooLong() => StatusCode(StatusCodes.Status414RequestUriTooLong);
        [NonAction] public virtual ObjectResult RequestUriTooLong(object value) => StatusCode(StatusCodes.Status414RequestUriTooLong, value);

        [NonAction] public virtual StatusCodeResult UriTooLong() => StatusCode(StatusCodes.Status414UriTooLong);
        [NonAction] public virtual ObjectResult UriTooLong(object value) => StatusCode(StatusCodes.Status414UriTooLong, value);

        [NonAction] public virtual StatusCodeResult UnsupportedMediaType() => StatusCode(StatusCodes.Status415UnsupportedMediaType);
        [NonAction] public virtual ObjectResult UnsupportedMediaType(object value) => StatusCode(StatusCodes.Status415UnsupportedMediaType, value);

        [NonAction] public virtual StatusCodeResult RangeNotSatisfiable() => StatusCode(StatusCodes.Status416RangeNotSatisfiable);
        [NonAction] public virtual ObjectResult RangeNotSatisfiable(object value) => StatusCode(StatusCodes.Status416RangeNotSatisfiable, value);

        [NonAction] public virtual StatusCodeResult RequestedRangeNotSatisfiable() => StatusCode(StatusCodes.Status416RequestedRangeNotSatisfiable);
        [NonAction] public virtual ObjectResult RequestedRangeNotSatisfiable(object value) => StatusCode(StatusCodes.Status416RequestedRangeNotSatisfiable, value);

        [NonAction] public virtual StatusCodeResult ExpectationFailed() => StatusCode(StatusCodes.Status417ExpectationFailed);
        [NonAction] public virtual ObjectResult ExpectationFailed(object value) => StatusCode(StatusCodes.Status417ExpectationFailed, value);

        [NonAction] public virtual StatusCodeResult ImATeapot() => StatusCode(StatusCodes.Status418ImATeapot);
        [NonAction] public virtual ObjectResult ImATeapot(object value) => StatusCode(StatusCodes.Status418ImATeapot, value);

        [NonAction] public virtual StatusCodeResult AuthenticationTimeout() => StatusCode(StatusCodes.Status419AuthenticationTimeout);
        [NonAction] public virtual ObjectResult AuthenticationTimeout(object value) => StatusCode(StatusCodes.Status419AuthenticationTimeout, value);

        [NonAction] public virtual StatusCodeResult MisdirectedRequest() => StatusCode(StatusCodes.Status421MisdirectedRequest);
        [NonAction] public virtual ObjectResult MisdirectedRequest(object value) => StatusCode(StatusCodes.Status421MisdirectedRequest, value);

        [NonAction] public virtual StatusCodeResult Locked() => StatusCode(StatusCodes.Status423Locked);
        [NonAction] public virtual ObjectResult Locked(object value) => StatusCode(StatusCodes.Status423Locked, value);

        [NonAction] public virtual StatusCodeResult FailedDependency() => StatusCode(StatusCodes.Status424FailedDependency);
        [NonAction] public virtual ObjectResult FailedDependency(object value) => StatusCode(StatusCodes.Status424FailedDependency, value);

        [NonAction] public virtual StatusCodeResult TooEarly() => StatusCode(425);
        [NonAction] public virtual ObjectResult TooEarly(object value) => StatusCode(425, value);

        [NonAction] public virtual StatusCodeResult UpgradeRequired() => StatusCode(StatusCodes.Status426UpgradeRequired);
        [NonAction] public virtual ObjectResult UpgradeRequired(object value) => StatusCode(StatusCodes.Status426UpgradeRequired, value);

        [NonAction] public virtual StatusCodeResult PreconditionRequired() => StatusCode(StatusCodes.Status428PreconditionRequired);
        [NonAction] public virtual ObjectResult PreconditionRequired(object value) => StatusCode(StatusCodes.Status428PreconditionRequired, value);

        [NonAction] public virtual StatusCodeResult TooManyRequests() => StatusCode(StatusCodes.Status429TooManyRequests);
        [NonAction] public virtual ObjectResult TooManyRequests(object value) => StatusCode(StatusCodes.Status429TooManyRequests, value);

        [NonAction] public virtual StatusCodeResult RequestHeaderFieldsTooLarge() => StatusCode(StatusCodes.Status431RequestHeaderFieldsTooLarge);
        [NonAction] public virtual ObjectResult RequestHeaderFieldsTooLarge(object value) => StatusCode(StatusCodes.Status431RequestHeaderFieldsTooLarge, value);

        [NonAction] public virtual StatusCodeResult ConnectionClosedWithoutResponse() => StatusCode(444);
        [NonAction] public virtual ObjectResult ConnectionClosedWithoutResponse(object value) => StatusCode(444, value);

        [NonAction] public virtual StatusCodeResult UnavailableForLegalReasons() => StatusCode(StatusCodes.Status451UnavailableForLegalReasons);
        [NonAction] public virtual ObjectResult UnavailableForLegalReasons(object value) => StatusCode(StatusCodes.Status451UnavailableForLegalReasons, value);

        [NonAction] public virtual StatusCodeResult ClientClosedRequest() => StatusCode(499);
        [NonAction] public virtual ObjectResult ClientClosedRequest(object value) => StatusCode(499, value);

        [NonAction] public virtual StatusCodeResult InternalServerError() => StatusCode(StatusCodes.Status500InternalServerError);
        [NonAction] public virtual ObjectResult InternalServerError(object value) => StatusCode(StatusCodes.Status500InternalServerError, value);

        [NonAction] public virtual StatusCodeResult NotImplemented() => StatusCode(StatusCodes.Status501NotImplemented);
        [NonAction] public virtual ObjectResult NotImplemented(object value) => StatusCode(StatusCodes.Status501NotImplemented, value);

        [NonAction] public virtual StatusCodeResult BadGateway() => StatusCode(StatusCodes.Status502BadGateway);
        [NonAction] public virtual ObjectResult BadGateway(object value) => StatusCode(StatusCodes.Status502BadGateway, value);

        [NonAction] public virtual StatusCodeResult ServiceUnavailable() => StatusCode(StatusCodes.Status503ServiceUnavailable);
        [NonAction] public virtual ObjectResult ServiceUnavailable(object value) => StatusCode(StatusCodes.Status503ServiceUnavailable, value);

        [NonAction] public virtual StatusCodeResult GatewayTimeout() => StatusCode(StatusCodes.Status504GatewayTimeout);
        [NonAction] public virtual ObjectResult GatewayTimeout(object value) => StatusCode(StatusCodes.Status504GatewayTimeout, value);

        [NonAction] public virtual StatusCodeResult HttpVersionNotsupported() => StatusCode(StatusCodes.Status505HttpVersionNotsupported);
        [NonAction] public virtual ObjectResult HttpVersionNotsupported(object value) => StatusCode(StatusCodes.Status505HttpVersionNotsupported, value);

        [NonAction] public virtual StatusCodeResult VariantAlsoNegotiates() => StatusCode(StatusCodes.Status506VariantAlsoNegotiates);
        [NonAction] public virtual ObjectResult VariantAlsoNegotiates(object value) => StatusCode(StatusCodes.Status506VariantAlsoNegotiates, value);

        [NonAction] public virtual StatusCodeResult InsufficientStorage() => StatusCode(StatusCodes.Status507InsufficientStorage);
        [NonAction] public virtual ObjectResult InsufficientStorage(object value) => StatusCode(StatusCodes.Status507InsufficientStorage, value);

        [NonAction] public virtual StatusCodeResult LoopDetected() => StatusCode(StatusCodes.Status508LoopDetected);
        [NonAction] public virtual ObjectResult LoopDetected(object value) => StatusCode(StatusCodes.Status508LoopDetected, value);

        [NonAction] public virtual StatusCodeResult NotExtended() => StatusCode(StatusCodes.Status510NotExtended);
        [NonAction] public virtual ObjectResult NotExtended(object value) => StatusCode(StatusCodes.Status510NotExtended, value);

        [NonAction] public virtual StatusCodeResult NetworkAuthenticationRequired() => StatusCode(StatusCodes.Status511NetworkAuthenticationRequired);
        [NonAction] public virtual ObjectResult NetworkAuthenticationRequired(object value) => StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, value);

        [NonAction] public virtual StatusCodeResult NetworkConnectTimeoutError() => StatusCode(599);
        [NonAction] public virtual ObjectResult NetworkConnectTimeoutError(object value) => StatusCode(599, value);

        #endregion Constructors

        /// <summary>
        /// Returns the DataStatus explanation and code as a string return value and an associated HTTP status code.
        /// Good is always mapped to OK (200), Uncertain is mapped to ServiceUnavailable (503). Bad values are mapped
        /// to various result codes (400-499, 500-599).
        /// </summary>
        /// <param name="status">The data status</param>
        /// <returns>The mapped data status</returns>
        [NonAction]
        public ObjectResult StatusCode(DataStatus status)
            => status.Code switch
            {
                (uint)DataStatus.Codes.Good                                    => Ok(status),
                (uint)DataStatus.Codes.Uncertain                               => ServiceUnavailable(status),
                (uint)DataStatus.Codes.Bad                                     => InternalServerError(status),

                (uint)DataStatus.Codes.GoodCallAgain                           => Ok(status),
                (uint)DataStatus.Codes.GoodClamped                             => Ok(status),
                (uint)DataStatus.Codes.GoodCommunicationEvent                  => Ok(status),
                (uint)DataStatus.Codes.GoodCompletesAsynchronously             => Ok(status),
                (uint)DataStatus.Codes.GoodDataIgnored                         => Ok(status),
                (uint)DataStatus.Codes.GoodDependentValueChanged               => Ok(status),
                (uint)DataStatus.Codes.GoodEdited                              => Ok(status),
                (uint)DataStatus.Codes.GoodEntryInserted                       => Ok(status),
                (uint)DataStatus.Codes.GoodEntryReplaced                       => Ok(status),
                (uint)DataStatus.Codes.GoodLocalOverride                       => Ok(status),
                (uint)DataStatus.Codes.GoodMoreData                            => Ok(status),
                (uint)DataStatus.Codes.GoodNoData                              => Ok(status),
                (uint)DataStatus.Codes.GoodNonCriticalTimeout                  => Ok(status),
                (uint)DataStatus.Codes.GoodOverload                            => Ok(status),
                (uint)DataStatus.Codes.GoodPostActionFailed                    => Ok(status),
                (uint)DataStatus.Codes.GoodResultsMayBeIncomplete              => Ok(status),
                (uint)DataStatus.Codes.GoodShutdownEvent                       => Ok(status),
                (uint)DataStatus.Codes.GoodSubscriptionTransferred             => Ok(status),

                (uint)DataStatus.Codes.UncertainDataSubNormal                  => ServiceUnavailable(status),
                (uint)DataStatus.Codes.UncertainDependentValueChanged          => ServiceUnavailable(status),
                (uint)DataStatus.Codes.UncertainDominantValueChanged           => ServiceUnavailable(status),
                (uint)DataStatus.Codes.UncertainEngineeringUnitsExceeded       => ServiceUnavailable(status),
                (uint)DataStatus.Codes.UncertainInitialValue                   => ServiceUnavailable(status),
                (uint)DataStatus.Codes.UncertainLastUsableValue                => ServiceUnavailable(status),
                (uint)DataStatus.Codes.UncertainNoCommunicationLastUsableValue => ServiceUnavailable(status),
                (uint)DataStatus.Codes.UncertainNotAllNodesAvailable           => ServiceUnavailable(status),
                (uint)DataStatus.Codes.UncertainReferenceNotDeleted            => ServiceUnavailable(status),
                (uint)DataStatus.Codes.UncertainReferenceOutOfServer           => ServiceUnavailable(status),
                (uint)DataStatus.Codes.UncertainSensorNotAccurate              => ServiceUnavailable(status),
                (uint)DataStatus.Codes.UncertainSubNormal                      => ServiceUnavailable(status),
                (uint)DataStatus.Codes.UncertainSubstituteValue                => ServiceUnavailable(status),

                (uint)DataStatus.Codes.BadAggregateConfigurationRejected       => InternalServerError(status),
                (uint)DataStatus.Codes.BadAggregateInvalidInputs               => InternalServerError(status),
                (uint)DataStatus.Codes.BadAggregateListMismatch                => InternalServerError(status),
                (uint)DataStatus.Codes.BadAggregateNotSupported                => InternalServerError(status),
                (uint)DataStatus.Codes.BadAlreadyExists                        => InternalServerError(status),
                (uint)DataStatus.Codes.BadApplicationSignatureInvalid          => NotAcceptable(status),
                (uint)DataStatus.Codes.BadArgumentsMissing                     => BadRequest(status),
                (uint)DataStatus.Codes.BadAttributeIdInvalid                   => BadRequest(status),
                (uint)DataStatus.Codes.BadBoundNotFound                        => RangeNotSatisfiable(status),
                (uint)DataStatus.Codes.BadBoundNotSupported                    => RangeNotSatisfiable(status),
                (uint)DataStatus.Codes.BadBrowseDirectionInvalid               => BadRequest(status),
                (uint)DataStatus.Codes.BadBrowseNameDuplicated                 => BadRequest(status),
                (uint)DataStatus.Codes.BadBrowseNameInvalid                    => BadRequest(status),
                (uint)DataStatus.Codes.BadCertificateChainIncomplete           => NotAcceptable(status),
                (uint)DataStatus.Codes.BadCertificateHostNameInvalid           => NotAcceptable(status),
                (uint)DataStatus.Codes.BadCertificateInvalid                   => NotAcceptable(status),
                (uint)DataStatus.Codes.BadCertificateIssuerRevocationUnknown   => NotAcceptable(status),
                (uint)DataStatus.Codes.BadCertificateIssuerRevoked             => NotAcceptable(status),
                (uint)DataStatus.Codes.BadCertificateIssuerTimeInvalid         => NotAcceptable(status),
                (uint)DataStatus.Codes.BadCertificateIssuerUseNotAllowed       => NotAcceptable(status),
                (uint)DataStatus.Codes.BadCertificatePolicyCheckFailed         => NotAcceptable(status),
                (uint)DataStatus.Codes.BadCertificateRevocationUnknown         => NotAcceptable(status),
                (uint)DataStatus.Codes.BadCertificateRevoked                   => NotAcceptable(status),
                (uint)DataStatus.Codes.BadCertificateTimeInvalid               => NotAcceptable(status),
                (uint)DataStatus.Codes.BadCertificateUntrusted                 => NotAcceptable(status),
                (uint)DataStatus.Codes.BadCertificateUriInvalid                => NotAcceptable(status),
                (uint)DataStatus.Codes.BadCertificateUseNotAllowed             => NotAcceptable(status),
                (uint)DataStatus.Codes.BadCommunicationError                   => InternalServerError(status),
                (uint)DataStatus.Codes.BadConditionAlreadyDisabled             => InternalServerError(status),
                (uint)DataStatus.Codes.BadConditionAlreadyEnabled              => InternalServerError(status),
                (uint)DataStatus.Codes.BadConditionAlreadyShelved              => InternalServerError(status),
                (uint)DataStatus.Codes.BadConditionBranchAlreadyAcked          => InternalServerError(status),
                (uint)DataStatus.Codes.BadConditionBranchAlreadyConfirmed      => InternalServerError(status),
                (uint)DataStatus.Codes.BadConditionDisabled                    => InternalServerError(status),
                (uint)DataStatus.Codes.BadConditionNotShelved                  => InternalServerError(status),
                (uint)DataStatus.Codes.BadConfigurationError                   => InternalServerError(status),
                (uint)DataStatus.Codes.BadConnectionClosed                     => InternalServerError(status),
                (uint)DataStatus.Codes.BadConnectionRejected                   => InternalServerError(status),
                (uint)DataStatus.Codes.BadContentFilterInvalid                 => InternalServerError(status),
                (uint)DataStatus.Codes.BadContinuationPointInvalid             => InternalServerError(status),
                (uint)DataStatus.Codes.BadDataEncodingInvalid                  => InternalServerError(status),
                (uint)DataStatus.Codes.BadDataEncodingUnsupported              => InternalServerError(status),
                (uint)DataStatus.Codes.BadDataLost                             => InternalServerError(status),
                (uint)DataStatus.Codes.BadDataTypeIdUnknown                    => InternalServerError(status),
                (uint)DataStatus.Codes.BadDataUnavailable                      => InternalServerError(status),
                (uint)DataStatus.Codes.BadDeadbandFilterInvalid                => InternalServerError(status),
                (uint)DataStatus.Codes.BadDecodingError                        => InternalServerError(status),
                (uint)DataStatus.Codes.BadDependentValueChanged                => InternalServerError(status),
                (uint)DataStatus.Codes.BadDeviceFailure                        => InternalServerError(status),
                (uint)DataStatus.Codes.BadDialogNotActive                      => InternalServerError(status),
                (uint)DataStatus.Codes.BadDialogResponseInvalid                => InternalServerError(status),
                (uint)DataStatus.Codes.BadDisconnect                           => InternalServerError(status),
                (uint)DataStatus.Codes.BadDiscoveryUrlMissing                  => InternalServerError(status),
                (uint)DataStatus.Codes.BadDominantValueChanged                 => InternalServerError(status),
                (uint)DataStatus.Codes.BadDuplicateReferenceNotAllowed         => InternalServerError(status),
                (uint)DataStatus.Codes.BadEncodingError                        => InternalServerError(status),
                (uint)DataStatus.Codes.BadEncodingLimitsExceeded               => InternalServerError(status),
                (uint)DataStatus.Codes.BadEndOfStream                          => InternalServerError(status),
                (uint)DataStatus.Codes.BadEntryExists                          => InternalServerError(status),
                (uint)DataStatus.Codes.BadEventFilterInvalid                   => InternalServerError(status),
                (uint)DataStatus.Codes.BadEventIdUnknown                       => InternalServerError(status),
                (uint)DataStatus.Codes.BadEventNotAcknowledgeable              => InternalServerError(status),
                (uint)DataStatus.Codes.BadExpectedStreamToBlock                => InternalServerError(status),
                (uint)DataStatus.Codes.BadFilterElementInvalid                 => InternalServerError(status),
                (uint)DataStatus.Codes.BadFilterLiteralInvalid                 => InternalServerError(status),
                (uint)DataStatus.Codes.BadFilterNotAllowed                     => InternalServerError(status),
                (uint)DataStatus.Codes.BadFilterOperandCountMismatch           => InternalServerError(status),
                (uint)DataStatus.Codes.BadFilterOperandInvalid                 => InternalServerError(status),
                (uint)DataStatus.Codes.BadFilterOperatorInvalid                => InternalServerError(status),
                (uint)DataStatus.Codes.BadFilterOperatorUnsupported            => InternalServerError(status),
                (uint)DataStatus.Codes.BadHistoryOperationInvalid              => InternalServerError(status),
                (uint)DataStatus.Codes.BadHistoryOperationUnsupported          => InternalServerError(status),
                (uint)DataStatus.Codes.BadIdentityChangeNotSupported           => NotAcceptable(status),
                (uint)DataStatus.Codes.BadIdentityTokenInvalid                 => NotAcceptable(status),
                (uint)DataStatus.Codes.BadIdentityTokenRejected                => NotAcceptable(status),
                (uint)DataStatus.Codes.BadIndexRangeInvalid                    => RangeNotSatisfiable(status),
                (uint)DataStatus.Codes.BadIndexRangeNoData                     => RangeNotSatisfiable(status),
                (uint)DataStatus.Codes.BadInsufficientClientProfile            => BadRequest(status),
                (uint)DataStatus.Codes.BadInternalError                        => InternalServerError(status),
                (uint)DataStatus.Codes.BadInvalidArgument                      => BadRequest(status),
                (uint)DataStatus.Codes.BadInvalidSelfReference                 => BadRequest(status),
                (uint)DataStatus.Codes.BadInvalidState                         => BadRequest(status),
                (uint)DataStatus.Codes.BadInvalidTimestamp                     => BadRequest(status),
                (uint)DataStatus.Codes.BadInvalidTimestampArgument             => BadRequest(status),
                (uint)DataStatus.Codes.BadLicenseExpired                       => InternalServerError(status),
                (uint)DataStatus.Codes.BadLicenseLimitsExceeded                => InternalServerError(status),
                (uint)DataStatus.Codes.BadLicenseNotAvailable                  => InternalServerError(status),
                (uint)DataStatus.Codes.BadMaxAgeInvalid                        => BadRequest(status),
                (uint)DataStatus.Codes.BadMaxConnectionsReached                => InternalServerError(status),
                (uint)DataStatus.Codes.BadMessageNotAvailable                  => BadRequest(status),
                (uint)DataStatus.Codes.BadMethodInvalid                        => BadRequest(status),
                (uint)DataStatus.Codes.BadMonitoredItemFilterInvalid           => BadRequest(status),
                (uint)DataStatus.Codes.BadMonitoredItemFilterUnsupported       => BadRequest(status),
                (uint)DataStatus.Codes.BadMonitoredItemIdInvalid               => BadRequest(status),
                (uint)DataStatus.Codes.BadMonitoringModeInvalid                => BadRequest(status),
                (uint)DataStatus.Codes.BadNoCommunication                      => InternalServerError(status),
                (uint)DataStatus.Codes.BadNoContinuationPoints                 => InternalServerError(status),
                (uint)DataStatus.Codes.BadNoData                               => InternalServerError(status),
                (uint)DataStatus.Codes.BadNoDataAvailable                      => InternalServerError(status),
                (uint)DataStatus.Codes.BadNodeAttributesInvalid                => InternalServerError(status),
                (uint)DataStatus.Codes.BadNodeClassInvalid                     => InternalServerError(status),
                (uint)DataStatus.Codes.BadNodeIdExists                         => InternalServerError(status),
                (uint)DataStatus.Codes.BadNodeIdInvalid                        => InternalServerError(status),
                (uint)DataStatus.Codes.BadNodeIdRejected                       => InternalServerError(status),
                (uint)DataStatus.Codes.BadNodeIdUnknown                        => InternalServerError(status),
                (uint)DataStatus.Codes.BadNoDeleteRights                       => InternalServerError(status),
                (uint)DataStatus.Codes.BadNodeNotInView                        => InternalServerError(status),
                (uint)DataStatus.Codes.BadNoEntryExists                        => NotFound(status),
                (uint)DataStatus.Codes.BadNoMatch                              => NotFound(status),
                (uint)DataStatus.Codes.BadNonceInvalid                         => InternalServerError(status),
                (uint)DataStatus.Codes.BadNoSubscription                       => InternalServerError(status),
                (uint)DataStatus.Codes.BadNotConnected                         => BadGateway(status),
                (uint)DataStatus.Codes.BadNotExecutable                        => InternalServerError(status),
                (uint)DataStatus.Codes.BadNotFound                             => NotFound(status),
                (uint)DataStatus.Codes.BadNothingToDo                          => UnprocessableEntity(status),
                (uint)DataStatus.Codes.BadNotImplemented                       => NotImplemented(status),
                (uint)DataStatus.Codes.BadNotReadable                          => NotAcceptable(status),
                (uint)DataStatus.Codes.BadNotSupported                         => NotAcceptable(status),
                (uint)DataStatus.Codes.BadNotTypeDefinition                    => NotAcceptable(status),
                (uint)DataStatus.Codes.BadNotWritable                          => NotAcceptable(status),
                (uint)DataStatus.Codes.BadNoValidCertificates                  => NotAcceptable(status),
                (uint)DataStatus.Codes.BadNumericOverflow                      => NotAcceptable(status),
                (uint)DataStatus.Codes.BadObjectDeleted                        => NotAcceptable(status),
                (uint)DataStatus.Codes.BadOperationAbandoned                   => NotAcceptable(status),
                (uint)DataStatus.Codes.BadOutOfMemory                          => InternalServerError(status),
                (uint)DataStatus.Codes.BadOutOfRange                           => InternalServerError(status),
                (uint)DataStatus.Codes.BadOutOfService                         => InternalServerError(status),
                (uint)DataStatus.Codes.BadParentNodeIdInvalid                  => BadRequest(status),
                (uint)DataStatus.Codes.BadProtocolVersionUnsupported           => BadRequest(status),
                (uint)DataStatus.Codes.BadQueryTooComplex                      => BadRequest(status),
                (uint)DataStatus.Codes.BadReferenceLocalOnly                   => BadRequest(status),
                (uint)DataStatus.Codes.BadReferenceNotAllowed                  => BadRequest(status),
                (uint)DataStatus.Codes.BadReferenceTypeIdInvalid               => BadRequest(status),
                (uint)DataStatus.Codes.BadRefreshInProgress                    => ServiceUnavailable(status),
                (uint)DataStatus.Codes.BadRequestCancelledByClient             => BadRequest(status),
                (uint)DataStatus.Codes.BadRequestCancelledByRequest            => BadRequest(status),
                (uint)DataStatus.Codes.BadRequestHeaderInvalid                 => BadRequest(status),
                (uint)DataStatus.Codes.BadRequestInterrupted                   => InternalServerError(status),
                (uint)DataStatus.Codes.BadRequestNotAllowed                    => BadRequest(status),
                (uint)DataStatus.Codes.BadRequestNotComplete                   => BadRequest(status),
                (uint)DataStatus.Codes.BadRequestTimeout                       => BadRequest(status),
                (uint)DataStatus.Codes.BadRequestTooLarge                      => BadRequest(status),
                (uint)DataStatus.Codes.BadRequestTypeInvalid                   => BadRequest(status),
                (uint)DataStatus.Codes.BadResourceUnavailable                  => InternalServerError(status),
                (uint)DataStatus.Codes.BadResponseTooLarge                     => InternalServerError(status),
                (uint)DataStatus.Codes.BadSecureChannelClosed                  => NotAcceptable(status),
                (uint)DataStatus.Codes.BadSecureChannelIdInvalid               => NotAcceptable(status),
                (uint)DataStatus.Codes.BadSecureChannelTokenUnknown            => NotAcceptable(status),
                (uint)DataStatus.Codes.BadSecurityChecksFailed                 => NotAcceptable(status),
                (uint)DataStatus.Codes.BadSecurityModeInsufficient             => NotAcceptable(status),
                (uint)DataStatus.Codes.BadSecurityModeRejected                 => NotAcceptable(status),
                (uint)DataStatus.Codes.BadSecurityPolicyRejected               => NotAcceptable(status),
                (uint)DataStatus.Codes.BadSempahoreFileMissing                 => InternalServerError(status),
                (uint)DataStatus.Codes.BadSensorFailure                        => InternalServerError(status),
                (uint)DataStatus.Codes.BadSequenceNumberInvalid                => NotAcceptable(status),
                (uint)DataStatus.Codes.BadSequenceNumberUnknown                => NotAcceptable(status),
                (uint)DataStatus.Codes.BadServerHalted                         => InternalServerError(status),
                (uint)DataStatus.Codes.BadServerIndexInvalid                   => InternalServerError(status),
                (uint)DataStatus.Codes.BadServerNameMissing                    => InternalServerError(status),
                (uint)DataStatus.Codes.BadServerNotConnected                   => InternalServerError(status),
                (uint)DataStatus.Codes.BadServerUriInvalid                     => InternalServerError(status),
                (uint)DataStatus.Codes.BadServiceUnsupported                   => InternalServerError(status),
                (uint)DataStatus.Codes.BadSessionClosed                        => InternalServerError(status),
                (uint)DataStatus.Codes.BadSessionIdInvalid                     => InternalServerError(status),
                (uint)DataStatus.Codes.BadSessionNotActivated                  => InternalServerError(status),
                (uint)DataStatus.Codes.BadShelvingTimeOutOfRange               => InternalServerError(status),
                (uint)DataStatus.Codes.BadShutdown                             => InternalServerError(status),
                (uint)DataStatus.Codes.BadSourceNodeIdInvalid                  => InternalServerError(status),
                (uint)DataStatus.Codes.BadStateNotActive                       => InternalServerError(status),
                (uint)DataStatus.Codes.BadStructureMissing                     => InternalServerError(status),
                (uint)DataStatus.Codes.BadSubscriptionIdInvalid                => BadRequest(status),
                (uint)DataStatus.Codes.BadSyntaxError                          => BadRequest(status),
                (uint)DataStatus.Codes.BadTargetNodeIdInvalid                  => BadRequest(status),
                (uint)DataStatus.Codes.BadTcpEndpointUrlInvalid                => BadRequest(status),
                (uint)DataStatus.Codes.BadTcpInternalError                     => InternalServerError(status),
                (uint)DataStatus.Codes.BadTcpMessageTooLarge                   => BadRequest(status),
                (uint)DataStatus.Codes.BadTcpMessageTypeInvalid                => BadRequest(status),
                (uint)DataStatus.Codes.BadTcpNotEnoughResources                => InternalServerError(status),
                (uint)DataStatus.Codes.BadTcpSecureChannelUnknown              => InternalServerError(status),
                (uint)DataStatus.Codes.BadTcpServerTooBusy                     => InternalServerError(status),
                (uint)DataStatus.Codes.BadTimeout                              => GatewayTimeout(status),
                (uint)DataStatus.Codes.BadTimestampNotSupported                => BadRequest(status),
                (uint)DataStatus.Codes.BadTimestampsToReturnInvalid            => BadRequest(status),
                (uint)DataStatus.Codes.BadTooManyArguments                     => BadRequest(status),
                (uint)DataStatus.Codes.BadTooManyMatches                       => BadRequest(status),
                (uint)DataStatus.Codes.BadTooManyMonitoredItems                => BadRequest(status),
                (uint)DataStatus.Codes.BadTooManyOperations                    => BadRequest(status),
                (uint)DataStatus.Codes.BadTooManyPublishRequests               => BadRequest(status),
                (uint)DataStatus.Codes.BadTooManySessions                      => BadRequest(status),
                (uint)DataStatus.Codes.BadTooManySubscriptions                 => BadRequest(status),
                (uint)DataStatus.Codes.BadTypeDefinitionInvalid                => BadRequest(status),
                (uint)DataStatus.Codes.BadTypeMismatch                         => BadRequest(status),
                (uint)DataStatus.Codes.BadUnexpectedError                      => InternalServerError(status),
                (uint)DataStatus.Codes.BadUnknownResponse                      => InternalServerError(status),
                (uint)DataStatus.Codes.BadUserAccessDenied                     => NotAcceptable(status),
                (uint)DataStatus.Codes.BadUserSignatureInvalid                 => NotAcceptable(status),
                (uint)DataStatus.Codes.BadViewIdUnknown                        => BadRequest(status),
                (uint)DataStatus.Codes.BadViewParameterMismatch                => BadRequest(status),
                (uint)DataStatus.Codes.BadViewTimestampInvalid                 => BadRequest(status),
                (uint)DataStatus.Codes.BadViewVersionInvalid                   => BadRequest(status),
                (uint)DataStatus.Codes.BadWaitingForInitialData                => InternalServerError(status),
                (uint)DataStatus.Codes.BadWaitingForResponse                   => InternalServerError(status),
                (uint)DataStatus.Codes.BadWouldBlock                           => NotAcceptable(status),
                (uint)DataStatus.Codes.BadWriteNotSupported                    => NotAcceptable(status),

                _ => throw new NotSupportedException()
            };
    }
}