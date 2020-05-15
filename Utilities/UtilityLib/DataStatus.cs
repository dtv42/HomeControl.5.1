// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStatus.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:53</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    #region Using Directives

    using System.Text.Json.Serialization;

    #endregion

    /// <summary>
    ///  The data status provides code, name, and explanantion based on OPC UA.
    ///  The numeric code describes the result of a service or operation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The StatusCode is defined in <b>OPC UA Specifications Part 4: Services, section 7.22</b>
    /// titled <b>StatusCode</b>.<br/>
    /// Also see https://github.com/OPCFoundation/UA-.NETStandard/blob/master/Stack/Opc.Ua.Core/Types/BuiltIn/StatusCode.cs. <br/>
    /// <br/></para>
    /// <para>
    /// A numeric code that is used to describe the result of a service or operation. The
    /// StatusCode uses bit-assignments, which are described below.
    /// <br/></para>
    /// <para>
    /// The StatusCode is a 32-bit number, with the top 16-bits (high word) representing the
    /// numeric value of an error or condition, whereas the bottom 16-bits (low word) represents
    /// additional flags to provide more information on the meaning of the status code.
    /// <br/></para>
    public class DataStatus
    {
        #region Public Enums

        /// <summary>
        ///  The enumeration of all predefined status codes (OPC UA).
        /// </summary>
        public enum Codes : uint
        {
            /// <summary>No Error.</summary>
            Good = 0x0,
            /// <summary>The value is uncertain, but no specific reason is known.</summary>
            Uncertain = 0x40000000,
            /// <summary>The value is bad, but no specific reason is known.</summary>
            Bad = 0x80000000,
            /// <summary>An unexpected error occurred.</summary>
            BadUnexpectedError = 0x80010000,
            /// <summary>An internal error occurred as a result of a programming or configuration error.</summary>
            BadInternalError = 0x80020000,
            /// <summary>Not enough memory to complete the operation.</summary>
            BadOutOfMemory = 0x80030000,
            /// <summary>An operating system resource is not available.</summary>
            BadResourceUnavailable = 0x80040000,
            /// <summary>A low level communication error occurred.</summary>
            BadCommunicationError = 0x80050000,
            /// <summary>Encoding halted because of invalid data in the objects being serialized.</summary>
            BadEncodingError = 0x80060000,
            /// <summary>Decoding halted because of invalid data in the stream.</summary>
            BadDecodingError = 0x80070000,
            /// <summary>The message encoding/decoding limits imposed by the stack have been exceeded.</summary>
            BadEncodingLimitsExceeded = 0x80080000,
            /// <summary>The request message size exceeds limits set by the server.</summary>
            BadRequestTooLarge = 0x80b80000,
            /// <summary>The response message size exceeds limits set by the client.</summary>
            BadResponseTooLarge = 0x80b90000,
            /// <summary>An unrecognized response was received from the server.</summary>
            BadUnknownResponse = 0x80090000,
            /// <summary>The operation timed out.</summary>
            BadTimeout = 0x800a0000,
            /// <summary>The server does not support the requested service.</summary>
            BadServiceUnsupported = 0x800b0000,
            /// <summary>The operation was cancelled because the application is shutting down.</summary>
            BadShutdown = 0x800c0000,
            /// <summary>The operation could not complete because the client is not connected to the server.</summary>
            BadServerNotConnected = 0x800d0000,
            /// <summary>The server has stopped and cannot process any requests.</summary>
            BadServerHalted = 0x800e0000,
            /// <summary>There was nothing to do because the client passed a list of operations with no elements.</summary>
            BadNothingToDo = 0x800f0000,
            /// <summary>The request could not be processed because it specified too many operations.</summary>
            BadTooManyOperations = 0x80100000,
            /// <summary>The request could not be processed because there are too many monitored items in the subscription.</summary>
            BadTooManyMonitoredItems = 0x80db0000,
            /// <summary>The extension object cannot be (de)serialized because the data type id is not recognized.</summary>
            BadDataTypeIdUnknown = 0x80110000,
            /// <summary>The certificate provided as a parameter is not valid.</summary>
            BadCertificateInvalid = 0x80120000,
            /// <summary>An error occurred verifying security.</summary>
            BadSecurityChecksFailed = 0x80130000,
            /// <summary>The certificate does not meet the requirements of the security policy.</summary>
            BadCertificatePolicyCheckFailed = 0x81140000,
            /// <summary>The certificate has expired or is not yet valid.</summary>
            BadCertificateTimeInvalid = 0x80140000,
            /// <summary>An issuer certificate has expired or is not yet valid.</summary>
            BadCertificateIssuerTimeInvalid = 0x80150000,
            /// <summary>The HostName used to connect to a server does not match a HostName in the certificate.</summary>
            BadCertificateHostNameInvalid = 0x80160000,
            /// <summary>The URI specified in the ApplicationDescription does not match the URI in the certificate.</summary>
            BadCertificateUriInvalid = 0x80170000,
            /// <summary>The certificate may not be used for the requested operation.</summary>
            BadCertificateUseNotAllowed = 0x80180000,
            /// <summary>The issuer certificate may not be used for the requested operation.</summary>
            BadCertificateIssuerUseNotAllowed = 0x80190000,
            /// <summary>The certificate is not trusted.</summary>
            BadCertificateUntrusted = 0x801a0000,
            /// <summary>It was not possible to determine if the certificate has been revoked.</summary>
            BadCertificateRevocationUnknown = 0x801b0000,
            /// <summary>It was not possible to determine if the issuer certificate has been revoked.</summary>
            BadCertificateIssuerRevocationUnknown = 0x801c0000,
            /// <summary>The certificate has been revoked.</summary>
            BadCertificateRevoked = 0x801d0000,
            /// <summary>The issuer certificate has been revoked.</summary>
            BadCertificateIssuerRevoked = 0x801e0000,
            /// <summary>The certificate chain is incomplete.</summary>
            BadCertificateChainIncomplete = 0x810d0000,
            /// <summary>User does not have permission to perform the requested operation.</summary>
            BadUserAccessDenied = 0x801f0000,
            /// <summary>The user identity token is not valid.</summary>
            BadIdentityTokenInvalid = 0x80200000,
            /// <summary>The user identity token is valid but the server has rejected it.</summary>
            BadIdentityTokenRejected = 0x80210000,
            /// <summary>The specified secure channel is no longer valid.</summary>
            BadSecureChannelIdInvalid = 0x80220000,
            /// <summary>The timestamp is outside the range allowed by the server.</summary>
            BadInvalidTimestamp = 0x80230000,
            /// <summary>The nonce does appear to be not a random value or it is not the correct length.</summary>
            BadNonceInvalid = 0x80240000,
            /// <summary>The session id is not valid.</summary>
            BadSessionIdInvalid = 0x80250000,
            /// <summary>The session was closed by the client.</summary>
            BadSessionClosed = 0x80260000,
            /// <summary>The session cannot be used because ActivateSession has not been called.</summary>
            BadSessionNotActivated = 0x80270000,
            /// <summary>The subscription id is not valid.</summary>
            BadSubscriptionIdInvalid = 0x80280000,
            /// <summary>The header for the request is missing or invalid.</summary>
            BadRequestHeaderInvalid = 0x802a0000,
            /// <summary>The timestamps to return parameter is invalid.</summary>
            BadTimestampsToReturnInvalid = 0x802b0000,
            /// <summary>The request was cancelled by the client.</summary>
            BadRequestCancelledByClient = 0x802c0000,
            /// <summary>Too many arguments were provided.</summary>
            BadTooManyArguments = 0x80e50000,
            /// <summary>The server requires a license to operate in general or to perform a service or operation, but existing license is expired.</summary>
            BadLicenseExpired = 0x810e0000,
            /// <summary>The server has limits on number of allowed operations / objects, based on installed licenses, and these limits where exceeded.</summary>
            BadLicenseLimitsExceeded = 0x810f0000,
            /// <summary>The server does not have a license which is required to operate in general or to perform a service or operation.</summary>
            BadLicenseNotAvailable = 0x81100000,
            /// <summary>The subscription was transferred to another session.</summary>
            GoodSubscriptionTransferred = 0x2d0000,
            /// <summary>The processing will complete asynchronously.</summary>
            GoodCompletesAsynchronously = 0x2e0000,
            /// <summary>Sampling has slowed down due to resource limitations.</summary>
            GoodOverload = 0x2f0000,
            /// <summary>The value written was accepted but was clamped.</summary>
            GoodClamped = 0x300000,
            /// <summary>Communication with the data source is defined, but not established, and there is no last known value available.</summary>
            BadNoCommunication = 0x80310000,
            /// <summary>Waiting for the server to obtain values from the underlying data source.</summary>
            BadWaitingForInitialData = 0x80320000,
            /// <summary>The syntax of the node id is not valid.</summary>
            BadNodeIdInvalid = 0x80330000,
            /// <summary>The node id refers to a node that does not exist in the server address space.</summary>
            BadNodeIdUnknown = 0x80340000,
            /// <summary>The attribute is not supported for the specified Node.</summary>
            BadAttributeIdInvalid = 0x80350000,
            /// <summary>The syntax of the index range parameter is invalid.</summary>
            BadIndexRangeInvalid = 0x80360000,
            /// <summary>No data exists within the range of indexes specified.</summary>
            BadIndexRangeNoData = 0x80370000,
            /// <summary>The data encoding is invalid.</summary>
            BadDataEncodingInvalid = 0x80380000,
            /// <summary>The server does not support the requested data encoding for the node.</summary>
            BadDataEncodingUnsupported = 0x80390000,
            /// <summary>The access level does not allow reading or subscribing to the Node.</summary>
            BadNotReadable = 0x803a0000,
            /// <summary>The access level does not allow writing to the Node.</summary>
            BadNotWritable = 0x803b0000,
            /// <summary>The value was out of range.</summary>
            BadOutOfRange = 0x803c0000,
            /// <summary>The requested operation is not supported.</summary>
            BadNotSupported = 0x803d0000,
            /// <summary>A requested item was not found or a search operation ended without success.</summary>
            BadNotFound = 0x803e0000,
            /// <summary>The object cannot be used because it has been deleted.</summary>
            BadObjectDeleted = 0x803f0000,
            /// <summary>Requested operation is not implemented.</summary>
            BadNotImplemented = 0x80400000,
            /// <summary>The monitoring mode is invalid.</summary>
            BadMonitoringModeInvalid = 0x80410000,
            /// <summary>The monitoring item id does not refer to a valid monitored item.</summary>
            BadMonitoredItemIdInvalid = 0x80420000,
            /// <summary>The monitored item filter parameter is not valid.</summary>
            BadMonitoredItemFilterInvalid = 0x80430000,
            /// <summary>The server does not support the requested monitored item filter.</summary>
            BadMonitoredItemFilterUnsupported = 0x80440000,
            /// <summary>A monitoring filter cannot be used in combination with the attribute specified.</summary>
            BadFilterNotAllowed = 0x80450000,
            /// <summary>A mandatory structured parameter was missing or null.</summary>
            BadStructureMissing = 0x80460000,
            /// <summary>The event filter is not valid.</summary>
            BadEventFilterInvalid = 0x80470000,
            /// <summary>The content filter is not valid.</summary>
            BadContentFilterInvalid = 0x80480000,
            /// <summary>An unrecognized operator was provided in a filter.</summary>
            BadFilterOperatorInvalid = 0x80c10000,
            /// <summary>A valid operator was provided, but the server does not provide support for this filter operator.</summary>
            BadFilterOperatorUnsupported = 0x80c20000,
            /// <summary>The number of operands provided for the filter operator was less then expected for the operand provided.</summary>
            BadFilterOperandCountMismatch = 0x80c30000,
            /// <summary>The operand used in a content filter is not valid.</summary>
            BadFilterOperandInvalid = 0x80490000,
            /// <summary>The referenced element is not a valid element in the content filter.</summary>
            BadFilterElementInvalid = 0x80c40000,
            /// <summary>The referenced literal is not a valid value.</summary>
            BadFilterLiteralInvalid = 0x80c50000,
            /// <summary>The continuation point provide is longer valid.</summary>
            BadContinuationPointInvalid = 0x804a0000,
            /// <summary>The operation could not be processed because all continuation points have been allocated.</summary>
            BadNoContinuationPoints = 0x804b0000,
            /// <summary>The reference type id does not refer to a valid reference type node.</summary>
            BadReferenceTypeIdInvalid = 0x804c0000,
            /// <summary>The browse direction is not valid.</summary>
            BadBrowseDirectionInvalid = 0x804d0000,
            /// <summary>The node is not part of the view.</summary>
            BadNodeNotInView = 0x804e0000,
            /// <summary>The number was not accepted because of a numeric overflow.</summary>
            BadNumericOverflow = 0x81120000,
            /// <summary>The ServerUri is not a valid URI.</summary>
            BadServerUriInvalid = 0x804f0000,
            /// <summary>No ServerName was specified.</summary>
            BadServerNameMissing = 0x80500000,
            /// <summary>No DiscoveryUrl was specified.</summary>
            BadDiscoveryUrlMissing = 0x80510000,
            /// <summary>The semaphore file specified by the client is not valid.</summary>
            BadSempahoreFileMissing = 0x80520000,
            /// <summary>The security token request type is not valid.</summary>
            BadRequestTypeInvalid = 0x80530000,
            /// <summary>The security mode does not meet the requirements set by the server.</summary>
            BadSecurityModeRejected = 0x80540000,
            /// <summary>The security policy does not meet the requirements set by the server.</summary>
            BadSecurityPolicyRejected = 0x80550000,
            /// <summary>The server has reached its maximum number of sessions.</summary>
            BadTooManySessions = 0x80560000,
            /// <summary>The user token signature is missing or invalid.</summary>
            BadUserSignatureInvalid = 0x80570000,
            /// <summary>The signature generated with the client certificate is missing or invalid.</summary>
            BadApplicationSignatureInvalid = 0x80580000,
            /// <summary>The client did not provide at least one software certificate that is valid and meets the profile requirements for the server.</summary>
            BadNoValidCertificates = 0x80590000,
            /// <summary>The server does not support changing the user identity assigned to the session.</summary>
            BadIdentityChangeNotSupported = 0x80c60000,
            /// <summary>The request was cancelled by the client with the Cancel service.</summary>
            BadRequestCancelledByRequest = 0x805a0000,
            /// <summary>The parent node id does not to refer to a valid node.</summary>
            BadParentNodeIdInvalid = 0x805b0000,
            /// <summary>The reference could not be created because it violates constraints imposed by the data model.</summary>
            BadReferenceNotAllowed = 0x805c0000,
            /// <summary>The requested node id was reject because it was either invalid or server does not allow node ids to be specified by the client.</summary>
            BadNodeIdRejected = 0x805d0000,
            /// <summary>The requested node id is already used by another node.</summary>
            BadNodeIdExists = 0x805e0000,
            /// <summary>The node class is not valid.</summary>
            BadNodeClassInvalid = 0x805f0000,
            /// <summary>The browse name is invalid.</summary>
            BadBrowseNameInvalid = 0x80600000,
            /// <summary>The browse name is not unique among nodes that share the same relationship with the parent.</summary>
            BadBrowseNameDuplicated = 0x80610000,
            /// <summary>The node attributes are not valid for the node class.</summary>
            BadNodeAttributesInvalid = 0x80620000,
            /// <summary>The type definition node id does not reference an appropriate type node.</summary>
            BadTypeDefinitionInvalid = 0x80630000,
            /// <summary>The source node id does not reference a valid node.</summary>
            BadSourceNodeIdInvalid = 0x80640000,
            /// <summary>The target node id does not reference a valid node.</summary>
            BadTargetNodeIdInvalid = 0x80650000,
            /// <summary>The reference type between the nodes is already defined.</summary>
            BadDuplicateReferenceNotAllowed = 0x80660000,
            /// <summary>The server does not allow this type of self reference on this node.</summary>
            BadInvalidSelfReference = 0x80670000,
            /// <summary>The reference type is not valid for a reference to a remote server.</summary>
            BadReferenceLocalOnly = 0x80680000,
            /// <summary>The server will not allow the node to be deleted.</summary>
            BadNoDeleteRights = 0x80690000,
            /// <summary>The server was not able to delete all target references.</summary>
            UncertainReferenceNotDeleted = 0x40bc0000,
            /// <summary>The server index is not valid.</summary>
            BadServerIndexInvalid = 0x806a0000,
            /// <summary>The view id does not refer to a valid view node.</summary>
            BadViewIdUnknown = 0x806b0000,
            /// <summary>The view timestamp is not available or not supported.</summary>
            BadViewTimestampInvalid = 0x80c90000,
            /// <summary>The view parameters are not consistent with each other.</summary>
            BadViewParameterMismatch = 0x80ca0000,
            /// <summary>The view version is not available or not supported.</summary>
            BadViewVersionInvalid = 0x80cb0000,
            /// <summary>The list of references may not be complete because the underlying system is not available.</summary>
            UncertainNotAllNodesAvailable = 0x40c00000,
            /// <summary>The server should have followed a reference to a node in a remote server but did not. The result set may be incomplete.</summary>
            GoodResultsMayBeIncomplete = 0xba0000,
            /// <summary>The provided Nodeid was not a type definition nodeid.</summary>
            BadNotTypeDefinition = 0x80c80000,
            /// <summary>One of the references to follow in the relative path references to a node in the address space in another server.</summary>
            UncertainReferenceOutOfServer = 0x406c0000,
            /// <summary>The requested operation has too many matches to return.</summary>
            BadTooManyMatches = 0x806d0000,
            /// <summary>The requested operation requires too many resources in the server.</summary>
            BadQueryTooComplex = 0x806e0000,
            /// <summary>The requested operation has no match to return.</summary>
            BadNoMatch = 0x806f0000,
            /// <summary>The max age parameter is invalid.</summary>
            BadMaxAgeInvalid = 0x80700000,
            /// <summary>The operation is not permitted over the current secure channel.</summary>
            BadSecurityModeInsufficient = 0x80e60000,
            /// <summary>The history details parameter is not valid.</summary>
            BadHistoryOperationInvalid = 0x80710000,
            /// <summary>The server does not support the requested operation.</summary>
            BadHistoryOperationUnsupported = 0x80720000,
            /// <summary>The defined timestamp to return was invalid.</summary>
            BadInvalidTimestampArgument = 0x80bd0000,
            /// <summary>The server does not support writing the combination of value, status and timestamps provided.</summary>
            BadWriteNotSupported = 0x80730000,
            /// <summary>The value supplied for the attribute is not of the same type as the attribute's value.</summary>
            BadTypeMismatch = 0x80740000,
            /// <summary>The method id does not refer to a method for the specified object.</summary>
            BadMethodInvalid = 0x80750000,
            /// <summary>The client did not specify all of the input arguments for the method.</summary>
            BadArgumentsMissing = 0x80760000,
            /// <summary>The executable attribute does not allow the execution of the method.</summary>
            BadNotExecutable = 0x81110000,
            /// <summary>The server has reached its maximum number of subscriptions.</summary>
            BadTooManySubscriptions = 0x80770000,
            /// <summary>The server has reached the maximum number of queued publish requests.</summary>
            BadTooManyPublishRequests = 0x80780000,
            /// <summary>There is no subscription available for this session.</summary>
            BadNoSubscription = 0x80790000,
            /// <summary>The sequence number is unknown to the server.</summary>
            BadSequenceNumberUnknown = 0x807a0000,
            /// <summary>The requested notification message is no longer available.</summary>
            BadMessageNotAvailable = 0x807b0000,
            /// <summary>The client of the current session does not support one or more Profiles that are necessary for the subscription.</summary>
            BadInsufficientClientProfile = 0x807c0000,
            /// <summary>The sub-state machine is not currently active.</summary>
            BadStateNotActive = 0x80bf0000,
            /// <summary>An equivalent rule already exists.</summary>
            BadAlreadyExists = 0x81150000,
            /// <summary>The server cannot process the request because it is too busy.</summary>
            BadTcpServerTooBusy = 0x807d0000,
            /// <summary>The type of the message specified in the header invalid.</summary>
            BadTcpMessageTypeInvalid = 0x807e0000,
            /// <summary>The SecureChannelId and/or TokenId are not currently in use.</summary>
            BadTcpSecureChannelUnknown = 0x807f0000,
            /// <summary>The size of the message specified in the header is too large.</summary>
            BadTcpMessageTooLarge = 0x80800000,
            /// <summary>There are not enough resources to process the request.</summary>
            BadTcpNotEnoughResources = 0x80810000,
            /// <summary>An internal error occurred.</summary>
            BadTcpInternalError = 0x80820000,
            /// <summary>The server does not recognize the QueryString specified.</summary>
            BadTcpEndpointUrlInvalid = 0x80830000,
            /// <summary>The request could not be sent because of a network interruption.</summary>
            BadRequestInterrupted = 0x80840000,
            /// <summary>Timeout occurred while processing the request.</summary>
            BadRequestTimeout = 0x80850000,
            /// <summary>The secure channel has been closed.</summary>
            BadSecureChannelClosed = 0x80860000,
            /// <summary>The token has expired or is not recognized.</summary>
            BadSecureChannelTokenUnknown = 0x80870000,
            /// <summary>The sequence number is not valid.</summary>
            BadSequenceNumberInvalid = 0x80880000,
            /// <summary>The applications do not have compatible protocol versions.</summary>
            BadProtocolVersionUnsupported = 0x80be0000,
            /// <summary>There is a problem with the configuration that affects the usefulness of the value.</summary>
            BadConfigurationError = 0x80890000,
            /// <summary>The variable should receive its value from another variable, but has never been configured to do so.</summary>
            BadNotConnected = 0x808a0000,
            /// <summary>There has been a failure in the device/data source that generates the value that has affected the value.</summary>
            BadDeviceFailure = 0x808b0000,
            /// <summary>There has been a failure in the sensor from which the value is derived by the device/data source.</summary>
            BadSensorFailure = 0x808c0000,
            /// <summary>The source of the data is not operational.</summary>
            BadOutOfService = 0x808d0000,
            /// <summary>The deadband filter is not valid.</summary>
            BadDeadbandFilterInvalid = 0x808e0000,
            /// <summary>Communication to the data source has failed. The variable value is the last value that had a good quality.</summary>
            UncertainNoCommunicationLastUsableValue = 0x408f0000,
            /// <summary>Whatever was updating this value has stopped doing so.</summary>
            UncertainLastUsableValue = 0x40900000,
            /// <summary>The value is an operational value that was manually overwritten.</summary>
            UncertainSubstituteValue = 0x40910000,
            /// <summary>The value is an initial value for a variable that normally receives its value from another variable.</summary>
            UncertainInitialValue = 0x40920000,
            /// <summary>The value is at one of the sensor limits.</summary>
            UncertainSensorNotAccurate = 0x40930000,
            /// <summary>The value is outside of the range of values defined for this parameter.</summary>
            UncertainEngineeringUnitsExceeded = 0x40940000,
            /// <summary>The value is derived from multiple sources and has less than the required number of Good sources.</summary>
            UncertainSubNormal = 0x40950000,
            /// <summary>The value has been overridden.</summary>
            GoodLocalOverride = 0x960000,
            /// <summary>This Condition refresh failed, a Condition refresh operation is already in progress.</summary>
            BadRefreshInProgress = 0x80970000,
            /// <summary>This condition has already been disabled.</summary>
            BadConditionAlreadyDisabled = 0x80980000,
            /// <summary>This condition has already been enabled.</summary>
            BadConditionAlreadyEnabled = 0x80cc0000,
            /// <summary>Property not available, this condition is disabled.</summary>
            BadConditionDisabled = 0x80990000,
            /// <summary>The specified event id is not recognized.</summary>
            BadEventIdUnknown = 0x809a0000,
            /// <summary>The event cannot be acknowledged.</summary>
            BadEventNotAcknowledgeable = 0x80bb0000,
            /// <summary>The dialog condition is not active.</summary>
            BadDialogNotActive = 0x80cd0000,
            /// <summary>The response is not valid for the dialog.</summary>
            BadDialogResponseInvalid = 0x80ce0000,
            /// <summary>The condition branch has already been acknowledged.</summary>
            BadConditionBranchAlreadyAcked = 0x80cf0000,
            /// <summary>The condition branch has already been confirmed.</summary>
            BadConditionBranchAlreadyConfirmed = 0x80d00000,
            /// <summary>The condition has already been shelved.</summary>
            BadConditionAlreadyShelved = 0x80d10000,
            /// <summary>The condition is not currently shelved.</summary>
            BadConditionNotShelved = 0x80d20000,
            /// <summary>The shelving time not within an acceptable range.</summary>
            BadShelvingTimeOutOfRange = 0x80d30000,
            /// <summary>No data exists for the requested time range or event filter.</summary>
            BadNoData = 0x809b0000,
            /// <summary>No data found to provide upper or lower bound value.</summary>
            BadBoundNotFound = 0x80d70000,
            /// <summary>The server cannot retrieve a bound for the variable.</summary>
            BadBoundNotSupported = 0x80d80000,
            /// <summary>Data is missing due to collection started/stopped/lost.</summary>
            BadDataLost = 0x809d0000,
            /// <summary>Expected data is unavailable for the requested time range due to an un-mounted volume, an off-line archive or tape, or similar reason for temporary unavailability.</summary>
            BadDataUnavailable = 0x809e0000,
            /// <summary>The data or event was not successfully inserted because a matching entry exists.</summary>
            BadEntryExists = 0x809f0000,
            /// <summary>The data or event was not successfully updated because no matching entry exists.</summary>
            BadNoEntryExists = 0x80a00000,
            /// <summary>The client requested history using a timestamp format the server does not support (i.e requested ServerTimestamp when server only supports SourceTimestamp).</summary>
            BadTimestampNotSupported = 0x80a10000,
            /// <summary>The data or event was successfully inserted into the historical database.</summary>
            GoodEntryInserted = 0xa20000,
            /// <summary>The data or event field was successfully replaced in the historical database.</summary>
            GoodEntryReplaced = 0xa30000,
            /// <summary>The value is derived from multiple values and has less than the required number of Good values.</summary>
            UncertainDataSubNormal = 0x40a40000,
            /// <summary>No data exists for the requested time range or event filter.</summary>
            GoodNoData = 0xa50000,
            /// <summary>The data or event field was successfully replaced in the historical database.</summary>
            GoodMoreData = 0xa60000,
            /// <summary>The requested number of Aggregates does not match the requested number of NodeIds.</summary>
            BadAggregateListMismatch = 0x80d40000,
            /// <summary>The requested Aggregate is not support by the server.</summary>
            BadAggregateNotSupported = 0x80d50000,
            /// <summary>The aggregate value could not be derived due to invalid data inputs.</summary>
            BadAggregateInvalidInputs = 0x80d60000,
            /// <summary>The aggregate configuration is not valid for specified node.</summary>
            BadAggregateConfigurationRejected = 0x80da0000,
            /// <summary>The request specifies fields which are not valid for the EventType or cannot be saved by the historian.</summary>
            GoodDataIgnored = 0xd90000,
            /// <summary>The request was rejected by the server because it did not meet the criteria set by the server.</summary>
            BadRequestNotAllowed = 0x80e40000,
            /// <summary>The request has not been processed by the server yet.</summary>
            BadRequestNotComplete = 0x81130000,
            /// <summary>The value does not come from the real source and has been edited by the server.</summary>
            GoodEdited = 0xdc0000,
            /// <summary>There was an error in execution of these post-actions.</summary>
            GoodPostActionFailed = 0xdd0000,
            /// <summary>The related EngineeringUnit has been changed but the Variable Value is still provided based on the previous unit.</summary>
            UncertainDominantValueChanged = 0x40de0000,
            /// <summary>A dependent value has been changed but the change has not been applied to the device.</summary>
            GoodDependentValueChanged = 0xe00000,
            /// <summary>The related EngineeringUnit has been changed but this change has not been applied to the device. The Variable Value is still dependent on the previous unit but its status is currently Bad.</summary>
            BadDominantValueChanged = 0x80e10000,
            /// <summary>A dependent value has been changed but the change has not been applied to the device. The quality of the dominant variable is uncertain.</summary>
            UncertainDependentValueChanged = 0x40e20000,
            /// <summary>A dependent value has been changed but the change has not been applied to the device. The quality of the dominant variable is Bad.</summary>
            BadDependentValueChanged = 0x80e30000,
            /// <summary>The communication layer has raised an event.</summary>
            GoodCommunicationEvent = 0xa70000,
            /// <summary>The system is shutting down.</summary>
            GoodShutdownEvent = 0xa80000,
            /// <summary>The operation is not finished and needs to be called again.</summary>
            GoodCallAgain = 0xa90000,
            /// <summary>A non-critical timeout occurred.</summary>
            GoodNonCriticalTimeout = 0xaa0000,
            /// <summary>One or more arguments are invalid.</summary>
            BadInvalidArgument = 0x80ab0000,
            /// <summary>Could not establish a network connection to remote server.</summary>
            BadConnectionRejected = 0x80ac0000,
            /// <summary>The server has disconnected from the client.</summary>
            BadDisconnect = 0x80ad0000,
            /// <summary>The network connection has been closed.</summary>
            BadConnectionClosed = 0x80ae0000,
            /// <summary>The operation cannot be completed because the object is closed, uninitialized or in some other invalid state.</summary>
            BadInvalidState = 0x80af0000,
            /// <summary>Cannot move beyond end of the stream.</summary>
            BadEndOfStream = 0x80b00000,
            /// <summary>No data is currently available for reading from a non-blocking stream.</summary>
            BadNoDataAvailable = 0x80b10000,
            /// <summary>The asynchronous operation is waiting for a response.</summary>
            BadWaitingForResponse = 0x80b20000,
            /// <summary>The asynchronous operation was abandoned by the caller.</summary>
            BadOperationAbandoned = 0x80b30000,
            /// <summary>The stream did not return all data requested (possibly because it is a non-blocking stream).</summary>
            BadExpectedStreamToBlock = 0x80b40000,
            /// <summary>Non blocking behaviour is required and the operation would block.</summary>
            BadWouldBlock = 0x80b50000,
            /// <summary>A value had an invalid syntax.</summary>
            BadSyntaxError = 0x80b60000,
            /// <summary>The operation could not be finished because all available connections are in use.</summary>
            BadMaxConnectionsReached = 0x80b70000,
        }

        #endregion Public Enums

        #region Public Properties

        /// <summary>
        ///  A timestamp in milliseconds (Unix time).
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        ///  The numeric value of the StatusCode.
        /// </summary>
        public uint Code { get; set; } = (uint)Codes.Uncertain;

        /// <summary>
        ///  The symbolic name.
        /// </summary>
        public string Name { get; set; } = "Uncertain";

        /// <summary>
        ///  Short message explaining the StatusCode.
        /// </summary>
        public string Explanation { get; set; } = "The value is uncertain, but no specific reason is known.";

        /// <summary>
        ///  Returns true if the status code is good.
        /// </summary>
        [JsonIgnore]
        public bool IsGood { get => ((Code & 0xC0000000) == 0); }

        /// <summary>
        ///  Returns true if the status is bad or uncertain.
        /// </summary>
        [JsonIgnore]
        public bool IsNotGood { get => ((Code & 0xC0000000) != 0); }

        /// <summary>
        ///  Returns true if the status code is uncertain.
        /// </summary>
        [JsonIgnore]
        public bool IsUncertain { get => ((Code & 0x40000000) == 0x40000000); }

        /// <summary>
        ///  Returns true if the status is not uncertain.
        /// </summary>
        [JsonIgnore]
        public bool IsNotUncertain { get => ((Code & 0x40000000) != 0x40000000); }

        /// <summary>
        ///  Returns true if the status code is bad.
        /// </summary>
        [JsonIgnore]
        public bool IsBad { get => ((Code & 0x80000000) != 0); }

        /// <summary>
        ///  Returns true if the status is good or uncertain.
        /// </summary>
        [JsonIgnore]
        public bool IsNotBad { get => ((Code & 0x80000000) == 0); }

        #endregion Public Properties

        #region Static Fields

        /// <summary>
        ///   No Error.
        /// </summary>
        public static readonly DataStatus Good                             
	        = new DataStatus() { Code = (uint)Codes.Good, Name = "Good", Explanation = "No Error." };

        /// <summary>
        ///   The value is uncertain, but no specific reason is known.
        /// </summary>
        public static readonly DataStatus Uncertain                               
			= new DataStatus() { Code = (uint)Codes.Uncertain, Name = "Uncertain", Explanation = "The value is uncertain, but no specific reason is known." };

        /// <summary>
        ///   The value is bad, but no specific reason is known.
        /// </summary>
        public static readonly DataStatus Bad                                     
			= new DataStatus() { Code = (uint)Codes.Bad, Name = "Bad", Explanation = "The value is bad, but no specific reason is known." };

        /// <summary>
        ///   An unexpected error occurred.
        /// </summary>
        public static readonly DataStatus BadUnexpectedError                      
			= new DataStatus() { Code = (uint)Codes.BadUnexpectedError, Name = "BadUnexpectedError", Explanation = "An unexpected error occurred." };

        /// <summary>
        ///   An internal error occurred as a result of a programming or configuration error.
        /// </summary>
        public static readonly DataStatus BadInternalError                        
			= new DataStatus() { Code = (uint)Codes.BadInternalError, Name = "BadInternalError", Explanation = "An internal error occurred as a result of a programming or configuration error." };

        /// <summary>
        ///   Not enough memory to complete the operation.
        /// </summary>
        public static readonly DataStatus BadOutOfMemory                          
			= new DataStatus() { Code = (uint)Codes.BadOutOfMemory, Name = "BadOutOfMemory", Explanation = "Not enough memory to complete the operation." };

        /// <summary>
        ///   An operating system resource is not available.
        /// </summary>
        public static readonly DataStatus BadResourceUnavailable                  
			= new DataStatus() { Code = (uint)Codes.BadResourceUnavailable, Name = "BadResourceUnavailable", Explanation = "An operating system resource is not available." };

        /// <summary>
        ///   A low level communication error occurred.
        /// </summary>
        public static readonly DataStatus BadCommunicationError                   
			= new DataStatus() { Code = (uint)Codes.BadCommunicationError, Name = "BadCommunicationError", Explanation = "A low level communication error occurred." };

        /// <summary>
        ///   Encoding halted because of invalid data in the objects being serialized.
        /// </summary>
        public static readonly DataStatus BadEncodingError                        
			= new DataStatus() { Code = (uint)Codes.BadEncodingError, Name = "BadEncodingError", Explanation = "Encoding halted because of invalid data in the objects being serialized." };

        /// <summary>
        ///   Decoding halted because of invalid data in the stream.
        /// </summary>
        public static readonly DataStatus BadDecodingError                        
			= new DataStatus() { Code = (uint)Codes.BadDecodingError, Name = "BadDecodingError", Explanation = "Decoding halted because of invalid data in the stream." };

        /// <summary>
        ///   The message encoding/decoding limits imposed by the stack have been exceeded.
        /// </summary>
        public static readonly DataStatus BadEncodingLimitsExceeded               
			= new DataStatus() { Code = (uint)Codes.BadEncodingLimitsExceeded, Name = "BadEncodingLimitsExceeded", Explanation = "The message encoding/decoding limits imposed by the stack have been exceeded." };

        /// <summary>
        ///   The request message size exceeds limits set by the server.
        /// </summary>
        public static readonly DataStatus BadRequestTooLarge                      
			= new DataStatus() { Code = (uint)Codes.BadRequestTooLarge, Name = "BadRequestTooLarge", Explanation = "The request message size exceeds limits set by the server." };

        /// <summary>
        ///   The response message size exceeds limits set by the client.
        /// </summary>
        public static readonly DataStatus BadResponseTooLarge                     
			= new DataStatus() { Code = (uint)Codes.BadResponseTooLarge, Name = "BadResponseTooLarge", Explanation = "The response message size exceeds limits set by the client." };

        /// <summary>
        ///   An unrecognized response was received from the server.
        /// </summary>
        public static readonly DataStatus BadUnknownResponse                      
			= new DataStatus() { Code = (uint)Codes.BadUnknownResponse, Name = "BadUnknownResponse", Explanation = "An unrecognized response was received from the server." };

        /// <summary>
        ///   The operation timed out.
        /// </summary>
        public static readonly DataStatus BadTimeout                              
			= new DataStatus() { Code = (uint)Codes.BadTimeout, Name = "BadTimeout", Explanation = "The operation timed out." };

        /// <summary>
        ///   The server does not support the requested service.
        /// </summary>
        public static readonly DataStatus BadServiceUnsupported                   
			= new DataStatus() { Code = (uint)Codes.BadServiceUnsupported, Name = "BadServiceUnsupported", Explanation = "The server does not support the requested service." };

        /// <summary>
        ///   The operation was cancelled because the application is shutting down.
        /// </summary>
        public static readonly DataStatus BadShutdown                             
			= new DataStatus() { Code = (uint)Codes.BadShutdown, Name = "BadShutdown", Explanation = "The operation was cancelled because the application is shutting down." };

        /// <summary>
        ///   The operation could not complete because the client is not connected to the server.
        /// </summary>
        public static readonly DataStatus BadServerNotConnected                   
			= new DataStatus() { Code = (uint)Codes.BadServerNotConnected, Name = "BadServerNotConnected", Explanation = "The operation could not complete because the client is not connected to the server." };

        /// <summary>
        ///   The server has stopped and cannot process any requests.
        /// </summary>
        public static readonly DataStatus BadServerHalted                         
			= new DataStatus() { Code = (uint)Codes.BadServerHalted, Name = "BadServerHalted", Explanation = "The server has stopped and cannot process any requests." };

        /// <summary>
        ///   There was nothing to do because the client passed a list of operations with no elements.
        /// </summary>
        public static readonly DataStatus BadNothingToDo                          
			= new DataStatus() { Code = (uint)Codes.BadNothingToDo, Name = "BadNothingToDo", Explanation = "There was nothing to do because the client passed a list of operations with no elements." };

        /// <summary>
        ///   The request could not be processed because it specified too many operations.
        /// </summary>
        public static readonly DataStatus BadTooManyOperations                    
			= new DataStatus() { Code = (uint)Codes.BadTooManyOperations, Name = "BadTooManyOperations", Explanation = "The request could not be processed because it specified too many operations." };

        /// <summary>
        ///   The request could not be processed because there are too many monitored items in the subscription.
        /// </summary>
        public static readonly DataStatus BadTooManyMonitoredItems                
			= new DataStatus() { Code = (uint)Codes.BadTooManyMonitoredItems, Name = "BadTooManyMonitoredItems", Explanation = "The request could not be processed because there are too many monitored items in the subscription." };

        /// <summary>
        ///   The extension object cannot be (de)serialized because the data type id is not recognized.
        /// </summary>
        public static readonly DataStatus BadDataTypeIdUnknown                    
			= new DataStatus() { Code = (uint)Codes.BadDataTypeIdUnknown, Name = "BadDataTypeIdUnknown", Explanation = "The extension object cannot be (de)serialized because the data type id is not recognized." };

        /// <summary>
        ///   The certificate provided as a parameter is not valid.
        /// </summary>
        public static readonly DataStatus BadCertificateInvalid                   
			= new DataStatus() { Code = (uint)Codes.BadCertificateInvalid, Name = "BadCertificateInvalid", Explanation = "The certificate provided as a parameter is not valid." };

        /// <summary>
        ///   An error occurred verifying security.
        /// </summary>
        public static readonly DataStatus BadSecurityChecksFailed                 
			= new DataStatus() { Code = (uint)Codes.BadSecurityChecksFailed, Name = "BadSecurityChecksFailed", Explanation = "An error occurred verifying security." };

        /// <summary>
        ///   The certificate does not meet the requirements of the security policy.
        /// </summary>
        public static readonly DataStatus BadCertificatePolicyCheckFailed         
			= new DataStatus() { Code = (uint)Codes.BadCertificatePolicyCheckFailed, Name = "BadCertificatePolicyCheckFailed", Explanation = "The certificate does not meet the requirements of the security policy." };

        /// <summary>
        ///   The certificate has expired or is not yet valid.
        /// </summary>
        public static readonly DataStatus BadCertificateTimeInvalid               
			= new DataStatus() { Code = (uint)Codes.BadCertificateTimeInvalid, Name = "BadCertificateTimeInvalid", Explanation = "The certificate has expired or is not yet valid." };

        /// <summary>
        ///   An issuer certificate has expired or is not yet valid.
        /// </summary>
        public static readonly DataStatus BadCertificateIssuerTimeInvalid         
			= new DataStatus() { Code = (uint)Codes.BadCertificateIssuerTimeInvalid, Name = "BadCertificateIssuerTimeInvalid", Explanation = "An issuer certificate has expired or is not yet valid." };

        /// <summary>
        ///   The HostName used to connect to a server does not match a HostName in the certificate.
        /// </summary>
        public static readonly DataStatus BadCertificateHostNameInvalid           
			= new DataStatus() { Code = (uint)Codes.BadCertificateHostNameInvalid, Name = "BadCertificateHostNameInvalid", Explanation = "The HostName used to connect to a server does not match a HostName in the certificate." };

        /// <summary>
        ///   The URI specified in the ApplicationDescription does not match the URI in the certificate.
        /// </summary>
        public static readonly DataStatus BadCertificateUriInvalid                
			= new DataStatus() { Code = (uint)Codes.BadCertificateUriInvalid, Name = "BadCertificateUriInvalid", Explanation = "The URI specified in the ApplicationDescription does not match the URI in the certificate." };

        /// <summary>
        ///   The certificate may not be used for the requested operation.
        /// </summary>
        public static readonly DataStatus BadCertificateUseNotAllowed             
			= new DataStatus() { Code = (uint)Codes.BadCertificateUseNotAllowed, Name = "BadCertificateUseNotAllowed", Explanation = "The certificate may not be used for the requested operation." };

        /// <summary>
        ///   The issuer certificate may not be used for the requested operation.
        /// </summary>
        public static readonly DataStatus BadCertificateIssuerUseNotAllowed       
			= new DataStatus() { Code = (uint)Codes.BadCertificateIssuerUseNotAllowed, Name = "BadCertificateIssuerUseNotAllowed", Explanation = "The issuer certificate may not be used for the requested operation." };

        /// <summary>
        ///   The certificate is not trusted.
        /// </summary>
        public static readonly DataStatus BadCertificateUntrusted                 
			= new DataStatus() { Code = (uint)Codes.BadCertificateUntrusted, Name = "BadCertificateUntrusted", Explanation = "The certificate is not trusted." };

        /// <summary>
        ///   It was not possible to determine if the certificate has been revoked.
        /// </summary>
        public static readonly DataStatus BadCertificateRevocationUnknown         
			= new DataStatus() { Code = (uint)Codes.BadCertificateRevocationUnknown, Name = "BadCertificateRevocationUnknown", Explanation = "It was not possible to determine if the certificate has been revoked." };

        /// <summary>
        ///   It was not possible to determine if the issuer certificate has been revoked.
        /// </summary>
        public static readonly DataStatus BadCertificateIssuerRevocationUnknown   
			= new DataStatus() { Code = (uint)Codes.BadCertificateIssuerRevocationUnknown, Name = "BadCertificateIssuerRevocationUnknown", Explanation = "It was not possible to determine if the issuer certificate has been revoked." };

        /// <summary>
        ///   The certificate has been revoked.
        /// </summary>
        public static readonly DataStatus BadCertificateRevoked                   
			= new DataStatus() { Code = (uint)Codes.BadCertificateRevoked, Name = "BadCertificateRevoked", Explanation = "The certificate has been revoked." };

        /// <summary>
        ///   The issuer certificate has been revoked.
        /// </summary>
        public static readonly DataStatus BadCertificateIssuerRevoked             
			= new DataStatus() { Code = (uint)Codes.BadCertificateIssuerRevoked, Name = "BadCertificateIssuerRevoked", Explanation = "The issuer certificate has been revoked." };

        /// <summary>
        ///   The certificate chain is incomplete.
        /// </summary>
        public static readonly DataStatus BadCertificateChainIncomplete           
			= new DataStatus() { Code = (uint)Codes.BadCertificateChainIncomplete, Name = "BadCertificateChainIncomplete", Explanation = "The certificate chain is incomplete." };

        /// <summary>
        ///   User does not have permission to perform the requested operation.
        /// </summary>
        public static readonly DataStatus BadUserAccessDenied                     
			= new DataStatus() { Code = (uint)Codes.BadUserAccessDenied, Name = "BadUserAccessDenied", Explanation = "User does not have permission to perform the requested operation." };

        /// <summary>
        ///   The user identity token is not valid.
        /// </summary>
        public static readonly DataStatus BadIdentityTokenInvalid                 
			= new DataStatus() { Code = (uint)Codes.BadIdentityTokenInvalid, Name = "BadIdentityTokenInvalid", Explanation = "The user identity token is not valid." };

        /// <summary>
        ///   The user identity token is valid but the server has rejected it.
        /// </summary>
        public static readonly DataStatus BadIdentityTokenRejected                
			= new DataStatus() { Code = (uint)Codes.BadIdentityTokenRejected, Name = "BadIdentityTokenRejected", Explanation = "The user identity token is valid but the server has rejected it." };

        /// <summary>
        ///   The specified secure channel is no longer valid.
        /// </summary>
        public static readonly DataStatus BadSecureChannelIdInvalid               
			= new DataStatus() { Code = (uint)Codes.BadSecureChannelIdInvalid, Name = "BadSecureChannelIdInvalid", Explanation = "The specified secure channel is no longer valid." };

        /// <summary>
        ///   The timestamp is outside the range allowed by the server.
        /// </summary>
        public static readonly DataStatus BadInvalidTimestamp                     
			= new DataStatus() { Code = (uint)Codes.BadInvalidTimestamp, Name = "BadInvalidTimestamp", Explanation = "The timestamp is outside the range allowed by the server." };

        /// <summary>
        ///   The nonce does appear to be not a random value or it is not the correct length.
        /// </summary>
        public static readonly DataStatus BadNonceInvalid                         
			= new DataStatus() { Code = (uint)Codes.BadNonceInvalid, Name = "BadNonceInvalid", Explanation = "The nonce does appear to be not a random value or it is not the correct length." };

        /// <summary>
        ///   The session id is not valid.
        /// </summary>
        public static readonly DataStatus BadSessionIdInvalid                     
			= new DataStatus() { Code = (uint)Codes.BadSessionIdInvalid, Name = "BadSessionIdInvalid", Explanation = "The session id is not valid." };

        /// <summary>
        ///   The session was closed by the client.
        /// </summary>
        public static readonly DataStatus BadSessionClosed                        
			= new DataStatus() { Code = (uint)Codes.BadSessionClosed, Name = "BadSessionClosed", Explanation = "The session was closed by the client." };

        /// <summary>
        ///   The session cannot be used because ActivateSession has not been called.
        /// </summary>
        public static readonly DataStatus BadSessionNotActivated                  
			= new DataStatus() { Code = (uint)Codes.BadSessionNotActivated, Name = "BadSessionNotActivated", Explanation = "The session cannot be used because ActivateSession has not been called." };

        /// <summary>
        ///   The subscription id is not valid.
        /// </summary>
        public static readonly DataStatus BadSubscriptionIdInvalid                
			= new DataStatus() { Code = (uint)Codes.BadSubscriptionIdInvalid, Name = "BadSubscriptionIdInvalid", Explanation = "The subscription id is not valid." };

        /// <summary>
        ///   The header for the request is missing or invalid.
        /// </summary>
        public static readonly DataStatus BadRequestHeaderInvalid                 
			= new DataStatus() { Code = (uint)Codes.BadRequestHeaderInvalid, Name = "BadRequestHeaderInvalid", Explanation = "The header for the request is missing or invalid." };

        /// <summary>
        ///   The timestamps to return parameter is invalid.
        /// </summary>
        public static readonly DataStatus BadTimestampsToReturnInvalid            
			= new DataStatus() { Code = (uint)Codes.BadTimestampsToReturnInvalid, Name = "BadTimestampsToReturnInvalid", Explanation = "The timestamps to return parameter is invalid." };

        /// <summary>
        ///   The request was cancelled by the client.
        /// </summary>
        public static readonly DataStatus BadRequestCancelledByClient             
			= new DataStatus() { Code = (uint)Codes.BadRequestCancelledByClient, Name = "BadRequestCancelledByClient", Explanation = "The request was cancelled by the client." };

        /// <summary>
        ///   Too many arguments were provided.
        /// </summary>
        public static readonly DataStatus BadTooManyArguments                     
			= new DataStatus() { Code = (uint)Codes.BadTooManyArguments, Name = "BadTooManyArguments", Explanation = "Too many arguments were provided." };

        /// <summary>
        ///   The server requires a license to operate in general or to perform a service or operation, but existing license is expired.
        /// </summary>
        public static readonly DataStatus BadLicenseExpired                       
			= new DataStatus() { Code = (uint)Codes.BadLicenseExpired, Name = "BadLicenseExpired", Explanation = "The server requires a license to operate in general or to perform a service or operation, but existing license is expired." };

        /// <summary>
        ///   The server has limits on number of allowed operations / objects, based on installed licenses, and these limits where exceeded.
        /// </summary>
        public static readonly DataStatus BadLicenseLimitsExceeded                
			= new DataStatus() { Code = (uint)Codes.BadLicenseLimitsExceeded, Name = "BadLicenseLimitsExceeded", Explanation = "The server has limits on number of allowed operations / objects, based on installed licenses, and these limits where exceeded." };

        /// <summary>
        ///   The server does not have a license which is required to operate in general or to perform a service or operation.
        /// </summary>
        public static readonly DataStatus BadLicenseNotAvailable                  
			= new DataStatus() { Code = (uint)Codes.BadLicenseNotAvailable, Name = "BadLicenseNotAvailable", Explanation = "The server does not have a license which is required to operate in general or to perform a service or operation." };

        /// <summary>
        ///   The subscription was transferred to another session.
        /// </summary>
        public static readonly DataStatus GoodSubscriptionTransferred             
			= new DataStatus() { Code = (uint)Codes.GoodSubscriptionTransferred, Name = "GoodSubscriptionTransferred", Explanation = "The subscription was transferred to another session." };

        /// <summary>
        ///   The processing will complete asynchronously.
        /// </summary>
        public static readonly DataStatus GoodCompletesAsynchronously             
			= new DataStatus() { Code = (uint)Codes.GoodCompletesAsynchronously, Name = "GoodCompletesAsynchronously", Explanation = "The processing will complete asynchronously." };

        /// <summary>
        ///   Sampling has slowed down due to resource limitations.
        /// </summary>
        public static readonly DataStatus GoodOverload                            
			= new DataStatus() { Code = (uint)Codes.GoodOverload, Name = "GoodOverload", Explanation = "Sampling has slowed down due to resource limitations." };

        /// <summary>
        ///   The value written was accepted but was clamped.
        /// </summary>
        public static readonly DataStatus GoodClamped                             
			= new DataStatus() { Code = (uint)Codes.GoodClamped, Name = "GoodClamped", Explanation = "The value written was accepted but was clamped." };

        /// <summary>
        ///   Communication with the data source is defined, but not established, and there is no last known value available.
        /// </summary>
        public static readonly DataStatus BadNoCommunication                      
			= new DataStatus() { Code = (uint)Codes.BadNoCommunication, Name = "BadNoCommunication", Explanation = "Communication with the data source is defined, but not established, and there is no last known value available." };

        /// <summary>
        ///   Waiting for the server to obtain values from the underlying data source.
        /// </summary>
        public static readonly DataStatus BadWaitingForInitialData                
			= new DataStatus() { Code = (uint)Codes.BadWaitingForInitialData, Name = "BadWaitingForInitialData", Explanation = "Waiting for the server to obtain values from the underlying data source." };

        /// <summary>
        ///   The syntax of the node id is not valid.
        /// </summary>
        public static readonly DataStatus BadNodeIdInvalid                        
			= new DataStatus() { Code = (uint)Codes.BadNodeIdInvalid, Name = "BadNodeIdInvalid", Explanation = "The syntax of the node id is not valid." };

        /// <summary>
        ///   The node id refers to a node that does not exist in the server address space.
        /// </summary>
        public static readonly DataStatus BadNodeIdUnknown                        
			= new DataStatus() { Code = (uint)Codes.BadNodeIdUnknown, Name = "BadNodeIdUnknown", Explanation = "The node id refers to a node that does not exist in the server address space." };

        /// <summary>
        ///   The attribute is not supported for the specified Node.
        /// </summary>
        public static readonly DataStatus BadAttributeIdInvalid                   
			= new DataStatus() { Code = (uint)Codes.BadAttributeIdInvalid, Name = "BadAttributeIdInvalid", Explanation = "The attribute is not supported for the specified Node." };

        /// <summary>
        ///   The syntax of the index range parameter is invalid.
        /// </summary>
        public static readonly DataStatus BadIndexRangeInvalid                    
			= new DataStatus() { Code = (uint)Codes.BadIndexRangeInvalid, Name = "BadIndexRangeInvalid", Explanation = "The syntax of the index range parameter is invalid." };

        /// <summary>
        ///   No data exists within the range of indexes specified.
        /// </summary>
        public static readonly DataStatus BadIndexRangeNoData                     
			= new DataStatus() { Code = (uint)Codes.BadIndexRangeNoData, Name = "BadIndexRangeNoData", Explanation = "No data exists within the range of indexes specified." };

        /// <summary>
        ///   The data encoding is invalid.
        /// </summary>
        public static readonly DataStatus BadDataEncodingInvalid                  
			= new DataStatus() { Code = (uint)Codes.BadDataEncodingInvalid, Name = "BadDataEncodingInvalid", Explanation = "The data encoding is invalid." };

        /// <summary>
        ///   The server does not support the requested data encoding for the node.
        /// </summary>
        public static readonly DataStatus BadDataEncodingUnsupported              
			= new DataStatus() { Code = (uint)Codes.BadDataEncodingUnsupported, Name = "BadDataEncodingUnsupported", Explanation = "The server does not support the requested data encoding for the node." };

        /// <summary>
        ///   The access level does not allow reading or subscribing to the Node.
        /// </summary>
        public static readonly DataStatus BadNotReadable                          
			= new DataStatus() { Code = (uint)Codes.BadNotReadable, Name = "BadNotReadable", Explanation = "The access level does not allow reading or subscribing to the Node." };

        /// <summary>
        ///   The access level does not allow writing to the Node.
        /// </summary>
        public static readonly DataStatus BadNotWritable                          
			= new DataStatus() { Code = (uint)Codes.BadNotWritable, Name = "BadNotWritable", Explanation = "The access level does not allow writing to the Node." };

        /// <summary>
        ///   The value was out of range.
        /// </summary>
        public static readonly DataStatus BadOutOfRange                           
			= new DataStatus() { Code = (uint)Codes.BadOutOfRange, Name = "BadOutOfRange", Explanation = "The value was out of range." };

        /// <summary>
        ///   The requested operation is not supported.
        /// </summary>
        public static readonly DataStatus BadNotSupported                         
			= new DataStatus() { Code = (uint)Codes.BadNotSupported, Name = "BadNotSupported", Explanation = "The requested operation is not supported." };

        /// <summary>
        ///   A requested item was not found or a search operation ended without success.
        /// </summary>
        public static readonly DataStatus BadNotFound                             
			= new DataStatus() { Code = (uint)Codes.BadNotFound, Name = "BadNotFound", Explanation = "A requested item was not found or a search operation ended without success." };

        /// <summary>
        ///   The object cannot be used because it has been deleted.
        /// </summary>
        public static readonly DataStatus BadObjectDeleted                        
			= new DataStatus() { Code = (uint)Codes.BadObjectDeleted, Name = "BadObjectDeleted", Explanation = "The object cannot be used because it has been deleted." };

        /// <summary>
        ///   Requested operation is not implemented.
        /// </summary>
        public static readonly DataStatus BadNotImplemented                       
			= new DataStatus() { Code = (uint)Codes.BadNotImplemented, Name = "BadNotImplemented", Explanation = "Requested operation is not implemented." };

        /// <summary>
        ///   The monitoring mode is invalid.
        /// </summary>
        public static readonly DataStatus BadMonitoringModeInvalid                
			= new DataStatus() { Code = (uint)Codes.BadMonitoringModeInvalid, Name = "BadMonitoringModeInvalid", Explanation = "The monitoring mode is invalid." };

        /// <summary>
        ///   The monitoring item id does not refer to a valid monitored item.
        /// </summary>
        public static readonly DataStatus BadMonitoredItemIdInvalid               
			= new DataStatus() { Code = (uint)Codes.BadMonitoredItemIdInvalid, Name = "BadMonitoredItemIdInvalid", Explanation = "The monitoring item id does not refer to a valid monitored item." };

        /// <summary>
        ///   The monitored item filter parameter is not valid.
        /// </summary>
        public static readonly DataStatus BadMonitoredItemFilterInvalid           
			= new DataStatus() { Code = (uint)Codes.BadMonitoredItemFilterInvalid, Name = "BadMonitoredItemFilterInvalid", Explanation = "The monitored item filter parameter is not valid." };

        /// <summary>
        ///   The server does not support the requested monitored item filter.
        /// </summary>
        public static readonly DataStatus BadMonitoredItemFilterUnsupported       
			= new DataStatus() { Code = (uint)Codes.BadMonitoredItemFilterUnsupported, Name = "BadMonitoredItemFilterUnsupported", Explanation = "The server does not support the requested monitored item filter." };

        /// <summary>
        ///   A monitoring filter cannot be used in combination with the attribute specified.
        /// </summary>
        public static readonly DataStatus BadFilterNotAllowed                     
			= new DataStatus() { Code = (uint)Codes.BadFilterNotAllowed, Name = "BadFilterNotAllowed", Explanation = "A monitoring filter cannot be used in combination with the attribute specified." };

        /// <summary>
        ///   A mandatory structured parameter was missing or null.
        /// </summary>
        public static readonly DataStatus BadStructureMissing                     
			= new DataStatus() { Code = (uint)Codes.BadStructureMissing, Name = "BadStructureMissing", Explanation = "A mandatory structured parameter was missing or null." };

        /// <summary>
        ///   The event filter is not valid.
        /// </summary>
        public static readonly DataStatus BadEventFilterInvalid                   
			= new DataStatus() { Code = (uint)Codes.BadEventFilterInvalid, Name = "BadEventFilterInvalid", Explanation = "The event filter is not valid." };

        /// <summary>
        ///   The content filter is not valid.
        /// </summary>
        public static readonly DataStatus BadContentFilterInvalid                 
			= new DataStatus() { Code = (uint)Codes.BadContentFilterInvalid, Name = "BadContentFilterInvalid", Explanation = "The content filter is not valid." };

        /// <summary>
        ///   An unrecognized operator was provided in a filter.
        /// </summary>
        public static readonly DataStatus BadFilterOperatorInvalid                
			= new DataStatus() { Code = (uint)Codes.BadFilterOperatorInvalid, Name = "BadFilterOperatorInvalid", Explanation = "An unrecognized operator was provided in a filter." };

        /// <summary>
        ///   A valid operator was provided, but the server does not provide support for this filter operator.
        /// </summary>
        public static readonly DataStatus BadFilterOperatorUnsupported            
			= new DataStatus() { Code = (uint)Codes.BadFilterOperatorUnsupported, Name = "BadFilterOperatorUnsupported", Explanation = "A valid operator was provided, but the server does not provide support for this filter operator." };

        /// <summary>
        ///   The number of operands provided for the filter operator was less then expected for the operand provided.
        /// </summary>
        public static readonly DataStatus BadFilterOperandCountMismatch           
			= new DataStatus() { Code = (uint)Codes.BadFilterOperandCountMismatch, Name = "BadFilterOperandCountMismatch", Explanation = "The number of operands provided for the filter operator was less then expected for the operand provided." };

        /// <summary>
        ///   The operand used in a content filter is not valid.
        /// </summary>
        public static readonly DataStatus BadFilterOperandInvalid                 
			= new DataStatus() { Code = (uint)Codes.BadFilterOperandInvalid, Name = "BadFilterOperandInvalid", Explanation = "The operand used in a content filter is not valid." };

        /// <summary>
        ///   The referenced element is not a valid element in the content filter.
        /// </summary>
        public static readonly DataStatus BadFilterElementInvalid                 
			= new DataStatus() { Code = (uint)Codes.BadFilterElementInvalid, Name = "BadFilterElementInvalid", Explanation = "The referenced element is not a valid element in the content filter." };

        /// <summary>
        ///   The referenced literal is not a valid value.
        /// </summary>
        public static readonly DataStatus BadFilterLiteralInvalid                 
			= new DataStatus() { Code = (uint)Codes.BadFilterLiteralInvalid, Name = "BadFilterLiteralInvalid", Explanation = "The referenced literal is not a valid value." };

        /// <summary>
        ///   The continuation point provide is longer valid.
        /// </summary>
        public static readonly DataStatus BadContinuationPointInvalid             
			= new DataStatus() { Code = (uint)Codes.BadContinuationPointInvalid, Name = "BadContinuationPointInvalid", Explanation = "The continuation point provide is longer valid." };

        /// <summary>
        ///   The operation could not be processed because all continuation points have been allocated.
        /// </summary>
        public static readonly DataStatus BadNoContinuationPoints                 
			= new DataStatus() { Code = (uint)Codes.BadNoContinuationPoints, Name = "BadNoContinuationPoints", Explanation = "The operation could not be processed because all continuation points have been allocated." };

        /// <summary>
        ///   The reference type id does not refer to a valid reference type node.
        /// </summary>
        public static readonly DataStatus BadReferenceTypeIdInvalid               
			= new DataStatus() { Code = (uint)Codes.BadReferenceTypeIdInvalid, Name = "BadReferenceTypeIdInvalid", Explanation = "The reference type id does not refer to a valid reference type node." };

        /// <summary>
        ///   The browse direction is not valid.
        /// </summary>
        public static readonly DataStatus BadBrowseDirectionInvalid               
			= new DataStatus() { Code = (uint)Codes.BadBrowseDirectionInvalid, Name = "BadBrowseDirectionInvalid", Explanation = "The browse direction is not valid." };

        /// <summary>
        ///   The node is not part of the view.
        /// </summary>
        public static readonly DataStatus BadNodeNotInView                        
			= new DataStatus() { Code = (uint)Codes.BadNodeNotInView, Name = "BadNodeNotInView", Explanation = "The node is not part of the view." };

        /// <summary>
        ///   The number was not accepted because of a numeric overflow.
        /// </summary>
        public static readonly DataStatus BadNumericOverflow                      
			= new DataStatus() { Code = (uint)Codes.BadNumericOverflow, Name = "BadNumericOverflow", Explanation = "The number was not accepted because of a numeric overflow." };

        /// <summary>
        ///   The ServerUri is not a valid URI.
        /// </summary>
        public static readonly DataStatus BadServerUriInvalid                     
			= new DataStatus() { Code = (uint)Codes.BadServerUriInvalid, Name = "BadServerUriInvalid", Explanation = "The ServerUri is not a valid URI." };

        /// <summary>
        ///   No ServerName was specified.
        /// </summary>
        public static readonly DataStatus BadServerNameMissing                    
			= new DataStatus() { Code = (uint)Codes.BadServerNameMissing, Name = "BadServerNameMissing", Explanation = "No ServerName was specified." };

        /// <summary>
        ///   No DiscoveryUrl was specified.
        /// </summary>
        public static readonly DataStatus BadDiscoveryUrlMissing                  
			= new DataStatus() { Code = (uint)Codes.BadDiscoveryUrlMissing, Name = "BadDiscoveryUrlMissing", Explanation = "No DiscoveryUrl was specified." };

        /// <summary>
        ///   The semaphore file specified by the client is not valid.
        /// </summary>
        public static readonly DataStatus BadSempahoreFileMissing                 
			= new DataStatus() { Code = (uint)Codes.BadSempahoreFileMissing, Name = "BadSempahoreFileMissing", Explanation = "The semaphore file specified by the client is not valid." };

        /// <summary>
        ///   The security token request type is not valid.
        /// </summary>
        public static readonly DataStatus BadRequestTypeInvalid                   
			= new DataStatus() { Code = (uint)Codes.BadRequestTypeInvalid, Name = "BadRequestTypeInvalid", Explanation = "The security token request type is not valid." };

        /// <summary>
        ///   The security mode does not meet the requirements set by the server.
        /// </summary>
        public static readonly DataStatus BadSecurityModeRejected                 
			= new DataStatus() { Code = (uint)Codes.BadSecurityModeRejected, Name = "BadSecurityModeRejected", Explanation = "The security mode does not meet the requirements set by the server." };

        /// <summary>
        ///   The security policy does not meet the requirements set by the server.
        /// </summary>
        public static readonly DataStatus BadSecurityPolicyRejected               
			= new DataStatus() { Code = (uint)Codes.BadSecurityPolicyRejected, Name = "BadSecurityPolicyRejected", Explanation = "The security policy does not meet the requirements set by the server." };

        /// <summary>
        ///   The server has reached its maximum number of sessions.
        /// </summary>
        public static readonly DataStatus BadTooManySessions                      
			= new DataStatus() { Code = (uint)Codes.BadTooManySessions, Name = "BadTooManySessions", Explanation = "The server has reached its maximum number of sessions." };

        /// <summary>
        ///   The user token signature is missing or invalid.
        /// </summary>
        public static readonly DataStatus BadUserSignatureInvalid                 
			= new DataStatus() { Code = (uint)Codes.BadUserSignatureInvalid, Name = "BadUserSignatureInvalid", Explanation = "The user token signature is missing or invalid." };

        /// <summary>
        ///   The signature generated with the client certificate is missing or invalid.
        /// </summary>
        public static readonly DataStatus BadApplicationSignatureInvalid          
			= new DataStatus() { Code = (uint)Codes.BadApplicationSignatureInvalid, Name = "BadApplicationSignatureInvalid", Explanation = "The signature generated with the client certificate is missing or invalid." };

        /// <summary>
        ///   The client did not provide at least one software certificate that is valid and meets the profile requirements for the server.
        /// </summary>
        public static readonly DataStatus BadNoValidCertificates                  
			= new DataStatus() { Code = (uint)Codes.BadNoValidCertificates, Name = "BadNoValidCertificates", Explanation = "The client did not provide at least one software certificate that is valid and meets the profile requirements for the server." };

        /// <summary>
        ///   The server does not support changing the user identity assigned to the session.
        /// </summary>
        public static readonly DataStatus BadIdentityChangeNotSupported           
			= new DataStatus() { Code = (uint)Codes.BadIdentityChangeNotSupported, Name = "BadIdentityChangeNotSupported", Explanation = "The server does not support changing the user identity assigned to the session." };

        /// <summary>
        ///   The request was cancelled by the client with the Cancel service.
        /// </summary>
        public static readonly DataStatus BadRequestCancelledByRequest            
			= new DataStatus() { Code = (uint)Codes.BadRequestCancelledByRequest, Name = "BadRequestCancelledByRequest", Explanation = "The request was cancelled by the client with the Cancel service." };

        /// <summary>
        ///   The parent node id does not to refer to a valid node.
        /// </summary>
        public static readonly DataStatus BadParentNodeIdInvalid                  
			= new DataStatus() { Code = (uint)Codes.BadParentNodeIdInvalid, Name = "BadParentNodeIdInvalid", Explanation = "The parent node id does not to refer to a valid node." };

        /// <summary>
        ///   The reference could not be created because it violates constraints imposed by the data model.
        /// </summary>
        public static readonly DataStatus BadReferenceNotAllowed                  
			= new DataStatus() { Code = (uint)Codes.BadReferenceNotAllowed, Name = "BadReferenceNotAllowed", Explanation = "The reference could not be created because it violates constraints imposed by the data model." };

        /// <summary>
        ///   The requested node id was reject because it was either invalid or server does not allow node ids to be specified by the client.
        /// </summary>
        public static readonly DataStatus BadNodeIdRejected                       
			= new DataStatus() { Code = (uint)Codes.BadNodeIdRejected, Name = "BadNodeIdRejected", Explanation = "The requested node id was reject because it was either invalid or server does not allow node ids to be specified by the client." };

        /// <summary>
        ///   The requested node id is already used by another node.
        /// </summary>
        public static readonly DataStatus BadNodeIdExists                         
			= new DataStatus() { Code = (uint)Codes.BadNodeIdExists, Name = "BadNodeIdExists", Explanation = "The requested node id is already used by another node." };

        /// <summary>
        ///   The node class is not valid.
        /// </summary>
        public static readonly DataStatus BadNodeClassInvalid                     
			= new DataStatus() { Code = (uint)Codes.BadNodeClassInvalid, Name = "BadNodeClassInvalid", Explanation = "The node class is not valid." };

        /// <summary>
        ///   The browse name is invalid.
        /// </summary>
        public static readonly DataStatus BadBrowseNameInvalid                    
			= new DataStatus() { Code = (uint)Codes.BadBrowseNameInvalid, Name = "BadBrowseNameInvalid", Explanation = "The browse name is invalid." };

        /// <summary>
        ///   The browse name is not unique among nodes that share the same relationship with the parent.
        /// </summary>
        public static readonly DataStatus BadBrowseNameDuplicated                 
			= new DataStatus() { Code = (uint)Codes.BadBrowseNameDuplicated, Name = "BadBrowseNameDuplicated", Explanation = "The browse name is not unique among nodes that share the same relationship with the parent." };

        /// <summary>
        ///   The node attributes are not valid for the node class.
        /// </summary>
        public static readonly DataStatus BadNodeAttributesInvalid                
			= new DataStatus() { Code = (uint)Codes.BadNodeAttributesInvalid, Name = "BadNodeAttributesInvalid", Explanation = "The node attributes are not valid for the node class." };

        /// <summary>
        ///   The type definition node id does not reference an appropriate type node.
        /// </summary>
        public static readonly DataStatus BadTypeDefinitionInvalid                
			= new DataStatus() { Code = (uint)Codes.BadTypeDefinitionInvalid, Name = "BadTypeDefinitionInvalid", Explanation = "The type definition node id does not reference an appropriate type node." };

        /// <summary>
        ///   The source node id does not reference a valid node.
        /// </summary>
        public static readonly DataStatus BadSourceNodeIdInvalid                  
			= new DataStatus() { Code = (uint)Codes.BadSourceNodeIdInvalid, Name = "BadSourceNodeIdInvalid", Explanation = "The source node id does not reference a valid node." };

        /// <summary>
        ///   The target node id does not reference a valid node.
        /// </summary>
        public static readonly DataStatus BadTargetNodeIdInvalid                  
			= new DataStatus() { Code = (uint)Codes.BadTargetNodeIdInvalid, Name = "BadTargetNodeIdInvalid", Explanation = "The target node id does not reference a valid node." };

        /// <summary>
        ///   The reference type between the nodes is already defined.
        /// </summary>
        public static readonly DataStatus BadDuplicateReferenceNotAllowed         
			= new DataStatus() { Code = (uint)Codes.BadDuplicateReferenceNotAllowed, Name = "BadDuplicateReferenceNotAllowed", Explanation = "The reference type between the nodes is already defined." };

        /// <summary>
        ///   The server does not allow this type of self reference on this node.
        /// </summary>
        public static readonly DataStatus BadInvalidSelfReference                 
			= new DataStatus() { Code = (uint)Codes.BadInvalidSelfReference, Name = "BadInvalidSelfReference", Explanation = "The server does not allow this type of self reference on this node." };

        /// <summary>
        ///   The reference type is not valid for a reference to a remote server.
        /// </summary>
        public static readonly DataStatus BadReferenceLocalOnly                   
			= new DataStatus() { Code = (uint)Codes.BadReferenceLocalOnly, Name = "BadReferenceLocalOnly", Explanation = "The reference type is not valid for a reference to a remote server." };

        /// <summary>
        ///   The server will not allow the node to be deleted.
        /// </summary>
        public static readonly DataStatus BadNoDeleteRights                       
			= new DataStatus() { Code = (uint)Codes.BadNoDeleteRights, Name = "BadNoDeleteRights", Explanation = "The server will not allow the node to be deleted." };

        /// <summary>
        ///   The server was not able to delete all target references.
        /// </summary>
        public static readonly DataStatus UncertainReferenceNotDeleted            
			= new DataStatus() { Code = (uint)Codes.UncertainReferenceNotDeleted, Name = "UncertainReferenceNotDeleted", Explanation = "The server was not able to delete all target references." };

        /// <summary>
        ///   The server index is not valid.
        /// </summary>
        public static readonly DataStatus BadServerIndexInvalid                   
			= new DataStatus() { Code = (uint)Codes.BadServerIndexInvalid, Name = "BadServerIndexInvalid", Explanation = "The server index is not valid." };

        /// <summary>
        ///   The view id does not refer to a valid view node.
        /// </summary>
        public static readonly DataStatus BadViewIdUnknown                        
			= new DataStatus() { Code = (uint)Codes.BadViewIdUnknown, Name = "BadViewIdUnknown", Explanation = "The view id does not refer to a valid view node." };

        /// <summary>
        ///   The view timestamp is not available or not supported.
        /// </summary>
        public static readonly DataStatus BadViewTimestampInvalid                 
			= new DataStatus() { Code = (uint)Codes.BadViewTimestampInvalid, Name = "BadViewTimestampInvalid", Explanation = "The view timestamp is not available or not supported." };

        /// <summary>
        ///   The view parameters are not consistent with each other.
        /// </summary>
        public static readonly DataStatus BadViewParameterMismatch                
			= new DataStatus() { Code = (uint)Codes.BadViewParameterMismatch, Name = "BadViewParameterMismatch", Explanation = "The view parameters are not consistent with each other." };

        /// <summary>
        ///   The view version is not available or not supported.
        /// </summary>
        public static readonly DataStatus BadViewVersionInvalid                   
			= new DataStatus() { Code = (uint)Codes.BadViewVersionInvalid, Name = "BadViewVersionInvalid", Explanation = "The view version is not available or not supported." };

        /// <summary>
        ///   The list of references may not be complete because the underlying system is not available.
        /// </summary>
        public static readonly DataStatus UncertainNotAllNodesAvailable           
			= new DataStatus() { Code = (uint)Codes.UncertainNotAllNodesAvailable, Name = "UncertainNotAllNodesAvailable", Explanation = "The list of references may not be complete because the underlying system is not available." };

        /// <summary>
        ///   The server should have followed a reference to a node in a remote server but did not. The result set may be incomplete.
        /// </summary>
        public static readonly DataStatus GoodResultsMayBeIncomplete              
			= new DataStatus() { Code = (uint)Codes.GoodResultsMayBeIncomplete, Name = "GoodResultsMayBeIncomplete", Explanation = "The server should have followed a reference to a node in a remote server but did not. The result set may be incomplete." };

        /// <summary>
        ///   The provided Nodeid was not a type definition nodeid.
        /// </summary>
        public static readonly DataStatus BadNotTypeDefinition                    
			= new DataStatus() { Code = (uint)Codes.BadNotTypeDefinition, Name = "BadNotTypeDefinition", Explanation = "The provided Nodeid was not a type definition nodeid." };

        /// <summary>
        ///   One of the references to follow in the relative path references to a node in the address space in another server.
        /// </summary>
        public static readonly DataStatus UncertainReferenceOutOfServer           
			= new DataStatus() { Code = (uint)Codes.UncertainReferenceOutOfServer, Name = "UncertainReferenceOutOfServer", Explanation = "One of the references to follow in the relative path references to a node in the address space in another server." };

        /// <summary>
        ///   The requested operation has too many matches to return.
        /// </summary>
        public static readonly DataStatus BadTooManyMatches                       
			= new DataStatus() { Code = (uint)Codes.BadTooManyMatches, Name = "BadTooManyMatches", Explanation = "The requested operation has too many matches to return." };

        /// <summary>
        ///   The requested operation requires too many resources in the server.
        /// </summary>
        public static readonly DataStatus BadQueryTooComplex                      
			= new DataStatus() { Code = (uint)Codes.BadQueryTooComplex, Name = "BadQueryTooComplex", Explanation = "The requested operation requires too many resources in the server." };

        /// <summary>
        ///   The requested operation has no match to return.
        /// </summary>
        public static readonly DataStatus BadNoMatch                              
			= new DataStatus() { Code = (uint)Codes.BadNoMatch, Name = "BadNoMatch", Explanation = "The requested operation has no match to return." };

        /// <summary>
        ///   The max age parameter is invalid.
        /// </summary>
        public static readonly DataStatus BadMaxAgeInvalid                        
			= new DataStatus() { Code = (uint)Codes.BadMaxAgeInvalid, Name = "BadMaxAgeInvalid", Explanation = "The max age parameter is invalid." };

        /// <summary>
        ///   The operation is not permitted over the current secure channel.
        /// </summary>
        public static readonly DataStatus BadSecurityModeInsufficient             
			= new DataStatus() { Code = (uint)Codes.BadSecurityModeInsufficient, Name = "BadSecurityModeInsufficient", Explanation = "The operation is not permitted over the current secure channel." };

        /// <summary>
        ///   The history details parameter is not valid.
        /// </summary>
        public static readonly DataStatus BadHistoryOperationInvalid              
			= new DataStatus() { Code = (uint)Codes.BadHistoryOperationInvalid, Name = "BadHistoryOperationInvalid", Explanation = "The history details parameter is not valid." };

        /// <summary>
        ///   The server does not support the requested operation.
        /// </summary>
        public static readonly DataStatus BadHistoryOperationUnsupported          
			= new DataStatus() { Code = (uint)Codes.BadHistoryOperationUnsupported, Name = "BadHistoryOperationUnsupported", Explanation = "The server does not support the requested operation." };

        /// <summary>
        ///   The defined timestamp to return was invalid.
        /// </summary>
        public static readonly DataStatus BadInvalidTimestampArgument             
			= new DataStatus() { Code = (uint)Codes.BadInvalidTimestampArgument, Name = "BadInvalidTimestampArgument", Explanation = "The defined timestamp to return was invalid." };

        /// <summary>
        ///   The server does not support writing the combination of value, status and timestamps provided.
        /// </summary>
        public static readonly DataStatus BadWriteNotSupported                    
			= new DataStatus() { Code = (uint)Codes.BadWriteNotSupported, Name = "BadWriteNotSupported", Explanation = "The server does not support writing the combination of value, status and timestamps provided." };

        /// <summary>
        ///   The value supplied for the attribute is not of the same type as the attribute's value.
        /// </summary>
        public static readonly DataStatus BadTypeMismatch                         
			= new DataStatus() { Code = (uint)Codes.BadTypeMismatch, Name = "BadTypeMismatch", Explanation = "The value supplied for the attribute is not of the same type as the attribute's value." };

        /// <summary>
        ///   The method id does not refer to a method for the specified object.
        /// </summary>
        public static readonly DataStatus BadMethodInvalid                        
			= new DataStatus() { Code = (uint)Codes.BadMethodInvalid, Name = "BadMethodInvalid", Explanation = "The method id does not refer to a method for the specified object." };

        /// <summary>
        ///   The client did not specify all of the input arguments for the method.
        /// </summary>
        public static readonly DataStatus BadArgumentsMissing                     
			= new DataStatus() { Code = (uint)Codes.BadArgumentsMissing, Name = "BadArgumentsMissing", Explanation = "The client did not specify all of the input arguments for the method." };

        /// <summary>
        ///   The executable attribute does not allow the execution of the method.
        /// </summary>
        public static readonly DataStatus BadNotExecutable                        
			= new DataStatus() { Code = (uint)Codes.BadNotExecutable, Name = "BadNotExecutable", Explanation = "The executable attribute does not allow the execution of the method." };

        /// <summary>
        ///   The server has reached its maximum number of subscriptions.
        /// </summary>
        public static readonly DataStatus BadTooManySubscriptions                 
			= new DataStatus() { Code = (uint)Codes.BadTooManySubscriptions, Name = "BadTooManySubscriptions", Explanation = "The server has reached its maximum number of subscriptions." };

        /// <summary>
        ///   The server has reached the maximum number of queued publish requests.
        /// </summary>
        public static readonly DataStatus BadTooManyPublishRequests               
			= new DataStatus() { Code = (uint)Codes.BadTooManyPublishRequests, Name = "BadTooManyPublishRequests", Explanation = "The server has reached the maximum number of queued publish requests." };

        /// <summary>
        ///   There is no subscription available for this session.
        /// </summary>
        public static readonly DataStatus BadNoSubscription                       
			= new DataStatus() { Code = (uint)Codes.BadNoSubscription, Name = "BadNoSubscription", Explanation = "There is no subscription available for this session." };

        /// <summary>
        ///   The sequence number is unknown to the server.
        /// </summary>
        public static readonly DataStatus BadSequenceNumberUnknown                
			= new DataStatus() { Code = (uint)Codes.BadSequenceNumberUnknown, Name = "BadSequenceNumberUnknown", Explanation = "The sequence number is unknown to the server." };

        /// <summary>
        ///   The requested notification message is no longer available.
        /// </summary>
        public static readonly DataStatus BadMessageNotAvailable                  
			= new DataStatus() { Code = (uint)Codes.BadMessageNotAvailable, Name = "BadMessageNotAvailable", Explanation = "The requested notification message is no longer available." };

        /// <summary>
        ///   The client of the current session does not support one or more Profiles that are necessary for the subscription.
        /// </summary>
        public static readonly DataStatus BadInsufficientClientProfile            
			= new DataStatus() { Code = (uint)Codes.BadInsufficientClientProfile, Name = "BadInsufficientClientProfile", Explanation = "The client of the current session does not support one or more Profiles that are necessary for the subscription." };

        /// <summary>
        ///   The sub-state machine is not currently active.
        /// </summary>
        public static readonly DataStatus BadStateNotActive                       
			= new DataStatus() { Code = (uint)Codes.BadStateNotActive, Name = "BadStateNotActive", Explanation = "The sub-state machine is not currently active." };

        /// <summary>
        ///   An equivalent rule already exists.
        /// </summary>
        public static readonly DataStatus BadAlreadyExists                        
			= new DataStatus() { Code = (uint)Codes.BadAlreadyExists, Name = "BadAlreadyExists", Explanation = "An equivalent rule already exists." };

        /// <summary>
        ///   The server cannot process the request because it is too busy.
        /// </summary>
        public static readonly DataStatus BadTcpServerTooBusy                     
			= new DataStatus() { Code = (uint)Codes.BadTcpServerTooBusy, Name = "BadTcpServerTooBusy", Explanation = "The server cannot process the request because it is too busy." };

        /// <summary>
        ///   The type of the message specified in the header invalid.
        /// </summary>
        public static readonly DataStatus BadTcpMessageTypeInvalid                
			= new DataStatus() { Code = (uint)Codes.BadTcpMessageTypeInvalid, Name = "BadTcpMessageTypeInvalid", Explanation = "The type of the message specified in the header invalid." };

        /// <summary>
        ///   The SecureChannelId and/or TokenId are not currently in use.
        /// </summary>
        public static readonly DataStatus BadTcpSecureChannelUnknown              
			= new DataStatus() { Code = (uint)Codes.BadTcpSecureChannelUnknown, Name = "BadTcpSecureChannelUnknown", Explanation = "The SecureChannelId and/or TokenId are not currently in use." };

        /// <summary>
        ///   The size of the message specified in the header is too large.
        /// </summary>
        public static readonly DataStatus BadTcpMessageTooLarge                   
			= new DataStatus() { Code = (uint)Codes.BadTcpMessageTooLarge, Name = "BadTcpMessageTooLarge", Explanation = "The size of the message specified in the header is too large." };

        /// <summary>
        ///   There are not enough resources to process the request.
        /// </summary>
        public static readonly DataStatus BadTcpNotEnoughResources                
			= new DataStatus() { Code = (uint)Codes.BadTcpNotEnoughResources, Name = "BadTcpNotEnoughResources", Explanation = "There are not enough resources to process the request." };

        /// <summary>
        ///   An internal error occurred.
        /// </summary>
        public static readonly DataStatus BadTcpInternalError                     
			= new DataStatus() { Code = (uint)Codes.BadTcpInternalError, Name = "BadTcpInternalError", Explanation = "An internal error occurred." };

        /// <summary>
        ///   The server does not recognize the QueryString specified.
        /// </summary>
        public static readonly DataStatus BadTcpEndpointUrlInvalid                
			= new DataStatus() { Code = (uint)Codes.BadTcpEndpointUrlInvalid, Name = "BadTcpEndpointUrlInvalid", Explanation = "The server does not recognize the QueryString specified." };

        /// <summary>
        ///   The request could not be sent because of a network interruption.
        /// </summary>
        public static readonly DataStatus BadRequestInterrupted                   
			= new DataStatus() { Code = (uint)Codes.BadRequestInterrupted, Name = "BadRequestInterrupted", Explanation = "The request could not be sent because of a network interruption." };

        /// <summary>
        ///   Timeout occurred while processing the request.
        /// </summary>
        public static readonly DataStatus BadRequestTimeout                       
			= new DataStatus() { Code = (uint)Codes.BadRequestTimeout, Name = "BadRequestTimeout", Explanation = "Timeout occurred while processing the request." };

        /// <summary>
        ///   The secure channel has been closed.
        /// </summary>
        public static readonly DataStatus BadSecureChannelClosed                  
			= new DataStatus() { Code = (uint)Codes.BadSecureChannelClosed, Name = "BadSecureChannelClosed", Explanation = "The secure channel has been closed." };

        /// <summary>
        ///   The token has expired or is not recognized.
        /// </summary>
        public static readonly DataStatus BadSecureChannelTokenUnknown            
			= new DataStatus() { Code = (uint)Codes.BadSecureChannelTokenUnknown, Name = "BadSecureChannelTokenUnknown", Explanation = "The token has expired or is not recognized." };

        /// <summary>
        ///   The sequence number is not valid.
        /// </summary>
        public static readonly DataStatus BadSequenceNumberInvalid                
			= new DataStatus() { Code = (uint)Codes.BadSequenceNumberInvalid, Name = "BadSequenceNumberInvalid", Explanation = "The sequence number is not valid." };

        /// <summary>
        ///   The applications do not have compatible protocol versions.
        /// </summary>
        public static readonly DataStatus BadProtocolVersionUnsupported           
			= new DataStatus() { Code = (uint)Codes.BadProtocolVersionUnsupported, Name = "BadProtocolVersionUnsupported", Explanation = "The applications do not have compatible protocol versions." };

        /// <summary>
        ///   There is a problem with the configuration that affects the usefulness of the value.
        /// </summary>
        public static readonly DataStatus BadConfigurationError                   
			= new DataStatus() { Code = (uint)Codes.BadConfigurationError, Name = "BadConfigurationError", Explanation = "There is a problem with the configuration that affects the usefulness of the value." };

        /// <summary>
        ///   The variable should receive its value from another variable, but has never been configured to do so.
        /// </summary>
        public static readonly DataStatus BadNotConnected                         
			= new DataStatus() { Code = (uint)Codes.BadNotConnected, Name = "BadNotConnected", Explanation = "The variable should receive its value from another variable, but has never been configured to do so." };

        /// <summary>
        ///   There has been a failure in the device/data source that generates the value that has affected the value.
        /// </summary>
        public static readonly DataStatus BadDeviceFailure                        
			= new DataStatus() { Code = (uint)Codes.BadDeviceFailure, Name = "BadDeviceFailure", Explanation = "There has been a failure in the device/data source that generates the value that has affected the value." };

        /// <summary>
        ///   There has been a failure in the sensor from which the value is derived by the device/data source.
        /// </summary>
        public static readonly DataStatus BadSensorFailure                        
			= new DataStatus() { Code = (uint)Codes.BadSensorFailure, Name = "BadSensorFailure", Explanation = "There has been a failure in the sensor from which the value is derived by the device/data source." };

        /// <summary>
        ///   The source of the data is not operational.
        /// </summary>
        public static readonly DataStatus BadOutOfService                         
			= new DataStatus() { Code = (uint)Codes.BadOutOfService, Name = "BadOutOfService", Explanation = "The source of the data is not operational." };

        /// <summary>
        ///   The deadband filter is not valid.
        /// </summary>
        public static readonly DataStatus BadDeadbandFilterInvalid                
			= new DataStatus() { Code = (uint)Codes.BadDeadbandFilterInvalid, Name = "BadDeadbandFilterInvalid", Explanation = "The deadband filter is not valid." };

        /// <summary>
        ///   Communication to the data source has failed. The variable value is the last value that had a good quality.
        /// </summary>
        public static readonly DataStatus UncertainNoCommunicationLastUsableValue 
			= new DataStatus() { Code = (uint)Codes.UncertainNoCommunicationLastUsableValue, Name = "UncertainNoCommunicationLastUsableValue", Explanation = "Communication to the data source has failed. The variable value is the last value that had a good quality." };

        /// <summary>
        ///   Whatever was updating this value has stopped doing so.
        /// </summary>
        public static readonly DataStatus UncertainLastUsableValue                
			= new DataStatus() { Code = (uint)Codes.UncertainLastUsableValue, Name = "UncertainLastUsableValue", Explanation = "Whatever was updating this value has stopped doing so." };

        /// <summary>
        ///   The value is an operational value that was manually overwritten.
        /// </summary>
        public static readonly DataStatus UncertainSubstituteValue                
			= new DataStatus() { Code = (uint)Codes.UncertainSubstituteValue, Name = "UncertainSubstituteValue", Explanation = "The value is an operational value that was manually overwritten." };

        /// <summary>
        ///   The value is an initial value for a variable that normally receives its value from another variable.
        /// </summary>
        public static readonly DataStatus UncertainInitialValue                   
			= new DataStatus() { Code = (uint)Codes.UncertainInitialValue, Name = "UncertainInitialValue", Explanation = "The value is an initial value for a variable that normally receives its value from another variable." };

        /// <summary>
        ///   The value is at one of the sensor limits.
        /// </summary>
        public static readonly DataStatus UncertainSensorNotAccurate              
			= new DataStatus() { Code = (uint)Codes.UncertainSensorNotAccurate, Name = "UncertainSensorNotAccurate", Explanation = "The value is at one of the sensor limits." };

        /// <summary>
        ///   The value is outside of the range of values defined for this parameter.
        /// </summary>
        public static readonly DataStatus UncertainEngineeringUnitsExceeded       
			= new DataStatus() { Code = (uint)Codes.UncertainEngineeringUnitsExceeded, Name = "UncertainEngineeringUnitsExceeded", Explanation = "The value is outside of the range of values defined for this parameter." };

        /// <summary>
        ///   The value is derived from multiple sources and has less than the required number of Good sources.
        /// </summary>
        public static readonly DataStatus UncertainSubNormal                      
			= new DataStatus() { Code = (uint)Codes.UncertainSubNormal, Name = "UncertainSubNormal", Explanation = "The value is derived from multiple sources and has less than the required number of Good sources." };

        /// <summary>
        ///   The value has been overridden.
        /// </summary>
        public static readonly DataStatus GoodLocalOverride                       
			= new DataStatus() { Code = (uint)Codes.GoodLocalOverride, Name = "GoodLocalOverride", Explanation = "The value has been overridden." };

        /// <summary>
        ///   This Condition refresh failed, a Condition refresh operation is already in progress.
        /// </summary>
        public static readonly DataStatus BadRefreshInProgress                    
			= new DataStatus() { Code = (uint)Codes.BadRefreshInProgress, Name = "BadRefreshInProgress", Explanation = "This Condition refresh failed, a Condition refresh operation is already in progress." };

        /// <summary>
        ///   This condition has already been disabled.
        /// </summary>
        public static readonly DataStatus BadConditionAlreadyDisabled             
			= new DataStatus() { Code = (uint)Codes.BadConditionAlreadyDisabled, Name = "BadConditionAlreadyDisabled", Explanation = "This condition has already been disabled." };

        /// <summary>
        ///   This condition has already been enabled.
        /// </summary>
        public static readonly DataStatus BadConditionAlreadyEnabled              
			= new DataStatus() { Code = (uint)Codes.BadConditionAlreadyEnabled, Name = "BadConditionAlreadyEnabled", Explanation = "This condition has already been enabled." };

        /// <summary>
        ///   Property not available, this condition is disabled.
        /// </summary>
        public static readonly DataStatus BadConditionDisabled                    
			= new DataStatus() { Code = (uint)Codes.BadConditionDisabled, Name = "BadConditionDisabled", Explanation = "Property not available, this condition is disabled." };

        /// <summary>
        ///   The specified event id is not recognized.
        /// </summary>
        public static readonly DataStatus BadEventIdUnknown                       
			= new DataStatus() { Code = (uint)Codes.BadEventIdUnknown, Name = "BadEventIdUnknown", Explanation = "The specified event id is not recognized." };

        /// <summary>
        ///   The event cannot be acknowledged.
        /// </summary>
        public static readonly DataStatus BadEventNotAcknowledgeable              
			= new DataStatus() { Code = (uint)Codes.BadEventNotAcknowledgeable, Name = "BadEventNotAcknowledgeable", Explanation = "The event cannot be acknowledged." };

        /// <summary>
        ///   The dialog condition is not active.
        /// </summary>
        public static readonly DataStatus BadDialogNotActive                      
			= new DataStatus() { Code = (uint)Codes.BadDialogNotActive, Name = "BadDialogNotActive", Explanation = "The dialog condition is not active." };

        /// <summary>
        ///   The response is not valid for the dialog.
        /// </summary>
        public static readonly DataStatus BadDialogResponseInvalid                
			= new DataStatus() { Code = (uint)Codes.BadDialogResponseInvalid, Name = "BadDialogResponseInvalid", Explanation = "The response is not valid for the dialog." };

        /// <summary>
        ///   The condition branch has already been acknowledged.
        /// </summary>
        public static readonly DataStatus BadConditionBranchAlreadyAcked          
			= new DataStatus() { Code = (uint)Codes.BadConditionBranchAlreadyAcked, Name = "BadConditionBranchAlreadyAcked", Explanation = "The condition branch has already been acknowledged." };

        /// <summary>
        ///   The condition branch has already been confirmed.
        /// </summary>
        public static readonly DataStatus BadConditionBranchAlreadyConfirmed      
			= new DataStatus() { Code = (uint)Codes.BadConditionBranchAlreadyConfirmed, Name = "BadConditionBranchAlreadyConfirmed", Explanation = "The condition branch has already been confirmed." };

        /// <summary>
        ///   The condition has already been shelved.
        /// </summary>
        public static readonly DataStatus BadConditionAlreadyShelved              
			= new DataStatus() { Code = (uint)Codes.BadConditionAlreadyShelved, Name = "BadConditionAlreadyShelved", Explanation = "The condition has already been shelved." };

        /// <summary>
        ///   The condition is not currently shelved.
        /// </summary>
        public static readonly DataStatus BadConditionNotShelved                  
			= new DataStatus() { Code = (uint)Codes.BadConditionNotShelved, Name = "BadConditionNotShelved", Explanation = "The condition is not currently shelved." };

        /// <summary>
        ///   The shelving time not within an acceptable range.
        /// </summary>
        public static readonly DataStatus BadShelvingTimeOutOfRange               
			= new DataStatus() { Code = (uint)Codes.BadShelvingTimeOutOfRange, Name = "BadShelvingTimeOutOfRange", Explanation = "The shelving time not within an acceptable range." };

        /// <summary>
        ///   No data exists for the requested time range or event filter.
        /// </summary>
        public static readonly DataStatus BadNoData                               
			= new DataStatus() { Code = (uint)Codes.BadNoData, Name = "BadNoData", Explanation = "No data exists for the requested time range or event filter." };

        /// <summary>
        ///   No data found to provide upper or lower bound value.
        /// </summary>
        public static readonly DataStatus BadBoundNotFound                        
			= new DataStatus() { Code = (uint)Codes.BadBoundNotFound, Name = "BadBoundNotFound", Explanation = "No data found to provide upper or lower bound value." };

        /// <summary>
        ///   The server cannot retrieve a bound for the variable.
        /// </summary>
        public static readonly DataStatus BadBoundNotSupported                    
			= new DataStatus() { Code = (uint)Codes.BadBoundNotSupported, Name = "BadBoundNotSupported", Explanation = "The server cannot retrieve a bound for the variable." };

        /// <summary>
        ///   Data is missing due to collection started/stopped/lost.
        /// </summary>
        public static readonly DataStatus BadDataLost                             
			= new DataStatus() { Code = (uint)Codes.BadDataLost, Name = "BadDataLost", Explanation = "Data is missing due to collection started/stopped/lost." };

        /// <summary>
        ///   Expected data is unavailable for the requested time range due to an un-mounted volume, an off-line archive or tape, or similar reason for temporary unavailability.
        /// </summary>
        public static readonly DataStatus BadDataUnavailable                      
			= new DataStatus() { Code = (uint)Codes.BadDataUnavailable, Name = "BadDataUnavailable", Explanation = "Expected data is unavailable for the requested time range due to an un-mounted volume, an off-line archive or tape, or similar reason for temporary unavailability." };

        /// <summary>
        ///   The data or event was not successfully inserted because a matching entry exists.
        /// </summary>
        public static readonly DataStatus BadEntryExists                          
			= new DataStatus() { Code = (uint)Codes.BadEntryExists, Name = "BadEntryExists", Explanation = "The data or event was not successfully inserted because a matching entry exists." };

        /// <summary>
        ///   The data or event was not successfully updated because no matching entry exists.
        /// </summary>
        public static readonly DataStatus BadNoEntryExists                        
			= new DataStatus() { Code = (uint)Codes.BadNoEntryExists, Name = "BadNoEntryExists", Explanation = "The data or event was not successfully updated because no matching entry exists." };

        /// <summary>
        ///   The client requested history using a timestamp format the server does not support (i.e requested ServerTimestamp when server only supports SourceTimestamp).
        /// </summary>
        public static readonly DataStatus BadTimestampNotSupported                
			= new DataStatus() { Code = (uint)Codes.BadTimestampNotSupported, Name = "BadTimestampNotSupported", Explanation = "The client requested history using a timestamp format the server does not support (i.e requested ServerTimestamp when server only supports SourceTimestamp)." };

        /// <summary>
        ///   The data or event was successfully inserted into the historical database.
        /// </summary>
        public static readonly DataStatus GoodEntryInserted                       
			= new DataStatus() { Code = (uint)Codes.GoodEntryInserted, Name = "GoodEntryInserted", Explanation = "The data or event was successfully inserted into the historical database." };

        /// <summary>
        ///   The data or event field was successfully replaced in the historical database.
        /// </summary>
        public static readonly DataStatus GoodEntryReplaced                       
			= new DataStatus() { Code = (uint)Codes.GoodEntryReplaced, Name = "GoodEntryReplaced", Explanation = "The data or event field was successfully replaced in the historical database." };

        /// <summary>
        ///   The value is derived from multiple values and has less than the required number of Good values.
        /// </summary>
        public static readonly DataStatus UncertainDataSubNormal                  
			= new DataStatus() { Code = (uint)Codes.UncertainDataSubNormal, Name = "UncertainDataSubNormal", Explanation = "The value is derived from multiple values and has less than the required number of Good values." };

        /// <summary>
        ///   No data exists for the requested time range or event filter.
        /// </summary>
        public static readonly DataStatus GoodNoData                              
			= new DataStatus() { Code = (uint)Codes.GoodNoData, Name = "GoodNoData", Explanation = "No data exists for the requested time range or event filter." };

        /// <summary>
        ///   The data or event field was successfully replaced in the historical database.
        /// </summary>
        public static readonly DataStatus GoodMoreData                            
			= new DataStatus() { Code = (uint)Codes.GoodMoreData, Name = "GoodMoreData", Explanation = "The data or event field was successfully replaced in the historical database." };

        /// <summary>
        ///   The requested number of Aggregates does not match the requested number of NodeIds.
        /// </summary>
        public static readonly DataStatus BadAggregateListMismatch                
			= new DataStatus() { Code = (uint)Codes.BadAggregateListMismatch, Name = "BadAggregateListMismatch", Explanation = "The requested number of Aggregates does not match the requested number of NodeIds." };

        /// <summary>
        ///   The requested Aggregate is not support by the server.
        /// </summary>
        public static readonly DataStatus BadAggregateNotSupported                
			= new DataStatus() { Code = (uint)Codes.BadAggregateNotSupported, Name = "BadAggregateNotSupported", Explanation = "The requested Aggregate is not support by the server." };

        /// <summary>
        ///   The aggregate value could not be derived due to invalid data inputs.
        /// </summary>
        public static readonly DataStatus BadAggregateInvalidInputs               
			= new DataStatus() { Code = (uint)Codes.BadAggregateInvalidInputs, Name = "BadAggregateInvalidInputs", Explanation = "The aggregate value could not be derived due to invalid data inputs." };

        /// <summary>
        ///   The aggregate configuration is not valid for specified node.
        /// </summary>
        public static readonly DataStatus BadAggregateConfigurationRejected       
			= new DataStatus() { Code = (uint)Codes.BadAggregateConfigurationRejected, Name = "BadAggregateConfigurationRejected", Explanation = "The aggregate configuration is not valid for specified node." };

        /// <summary>
        ///   The request specifies fields which are not valid for the EventType or cannot be saved by the historian.
        /// </summary>
        public static readonly DataStatus GoodDataIgnored                         
			= new DataStatus() { Code = (uint)Codes.GoodDataIgnored, Name = "GoodDataIgnored", Explanation = "The request specifies fields which are not valid for the EventType or cannot be saved by the historian." };

        /// <summary>
        ///   The request was rejected by the server because it did not meet the criteria set by the server.
        /// </summary>
        public static readonly DataStatus BadRequestNotAllowed                    
			= new DataStatus() { Code = (uint)Codes.BadRequestNotAllowed, Name = "BadRequestNotAllowed", Explanation = "The request was rejected by the server because it did not meet the criteria set by the server." };

        /// <summary>
        ///   The request has not been processed by the server yet.
        /// </summary>
        public static readonly DataStatus BadRequestNotComplete                   
			= new DataStatus() { Code = (uint)Codes.BadRequestNotComplete, Name = "BadRequestNotComplete", Explanation = "The request has not been processed by the server yet." };

        /// <summary>
        ///   The value does not come from the real source and has been edited by the server.
        /// </summary>
        public static readonly DataStatus GoodEdited                              
			= new DataStatus() { Code = (uint)Codes.GoodEdited, Name = "GoodEdited", Explanation = "The value does not come from the real source and has been edited by the server." };

        /// <summary>
        ///   There was an error in execution of these post-actions.
        /// </summary>
        public static readonly DataStatus GoodPostActionFailed                    
			= new DataStatus() { Code = (uint)Codes.GoodPostActionFailed, Name = "GoodPostActionFailed", Explanation = "There was an error in execution of these post-actions." };

        /// <summary>
        ///   The related EngineeringUnit has been changed but the Variable Value is still provided based on the previous unit.
        /// </summary>
        public static readonly DataStatus UncertainDominantValueChanged           
			= new DataStatus() { Code = (uint)Codes.UncertainDominantValueChanged, Name = "UncertainDominantValueChanged", Explanation = "The related EngineeringUnit has been changed but the Variable Value is still provided based on the previous unit." };

        /// <summary>
        ///   A dependent value has been changed but the change has not been applied to the device.
        /// </summary>
        public static readonly DataStatus GoodDependentValueChanged               
			= new DataStatus() { Code = (uint)Codes.GoodDependentValueChanged, Name = "GoodDependentValueChanged", Explanation = "A dependent value has been changed but the change has not been applied to the device." };

        /// <summary>
        ///   The related EngineeringUnit has been changed but this change has not been applied to the device. The Variable Value is still dependent on the previous unit but its status is currently Bad.
        /// </summary>
        public static readonly DataStatus BadDominantValueChanged                 
			= new DataStatus() { Code = (uint)Codes.BadDominantValueChanged, Name = "BadDominantValueChanged", Explanation = "The related EngineeringUnit has been changed but this change has not been applied to the device. The Variable Value is still dependent on the previous unit but its status is currently Bad." };

        /// <summary>
        ///   A dependent value has been changed but the change has not been applied to the device. The quality of the dominant variable is uncertain.
        /// </summary>
        public static readonly DataStatus UncertainDependentValueChanged          
			= new DataStatus() { Code = (uint)Codes.UncertainDependentValueChanged, Name = "UncertainDependentValueChanged", Explanation = "A dependent value has been changed but the change has not been applied to the device. The quality of the dominant variable is uncertain." };

        /// <summary>
        ///   A dependent value has been changed but the change has not been applied to the device. The quality of the dominant variable is Bad.
        /// </summary>
        public static readonly DataStatus BadDependentValueChanged                
			= new DataStatus() { Code = (uint)Codes.BadDependentValueChanged, Name = "BadDependentValueChanged", Explanation = "A dependent value has been changed but the change has not been applied to the device. The quality of the dominant variable is Bad." };

        /// <summary>
        ///   The communication layer has raised an event.
        /// </summary>
        public static readonly DataStatus GoodCommunicationEvent                  
			= new DataStatus() { Code = (uint)Codes.GoodCommunicationEvent, Name = "GoodCommunicationEvent", Explanation = "The communication layer has raised an event." };

        /// <summary>
        ///   The system is shutting down.
        /// </summary>
        public static readonly DataStatus GoodShutdownEvent                       
			= new DataStatus() { Code = (uint)Codes.GoodShutdownEvent, Name = "GoodShutdownEvent", Explanation = "The system is shutting down." };

        /// <summary>
        ///   The operation is not finished and needs to be called again.
        /// </summary>
        public static readonly DataStatus GoodCallAgain                           
			= new DataStatus() { Code = (uint)Codes.GoodCallAgain, Name = "GoodCallAgain", Explanation = "The operation is not finished and needs to be called again." };

        /// <summary>
        ///   A non-critical timeout occurred.
        /// </summary>
        public static readonly DataStatus GoodNonCriticalTimeout                  
			= new DataStatus() { Code = (uint)Codes.GoodNonCriticalTimeout, Name = "GoodNonCriticalTimeout", Explanation = "A non-critical timeout occurred." };

        /// <summary>
        ///   One or more arguments are invalid.
        /// </summary>
        public static readonly DataStatus BadInvalidArgument                      
			= new DataStatus() { Code = (uint)Codes.BadInvalidArgument, Name = "BadInvalidArgument", Explanation = "One or more arguments are invalid." };

        /// <summary>
        ///   Could not establish a network connection to remote server.
        /// </summary>
        public static readonly DataStatus BadConnectionRejected                   
			= new DataStatus() { Code = (uint)Codes.BadConnectionRejected, Name = "BadConnectionRejected", Explanation = "Could not establish a network connection to remote server." };

        /// <summary>
        ///   The server has disconnected from the client.
        /// </summary>
        public static readonly DataStatus BadDisconnect                           
			= new DataStatus() { Code = (uint)Codes.BadDisconnect, Name = "BadDisconnect", Explanation = "The server has disconnected from the client." };

        /// <summary>
        ///   The network connection has been closed.
        /// </summary>
        public static readonly DataStatus BadConnectionClosed                     
			= new DataStatus() { Code = (uint)Codes.BadConnectionClosed, Name = "BadConnectionClosed", Explanation = "The network connection has been closed." };

        /// <summary>
        ///   The operation cannot be completed because the object is closed, uninitialized or in some other invalid state.
        /// </summary>
        public static readonly DataStatus BadInvalidState                         
			= new DataStatus() { Code = (uint)Codes.BadInvalidState, Name = "BadInvalidState", Explanation = "The operation cannot be completed because the object is closed, uninitialized or in some other invalid state." };

        /// <summary>
        ///   Cannot move beyond end of the stream.
        /// </summary>
        public static readonly DataStatus BadEndOfStream                          
			= new DataStatus() { Code = (uint)Codes.BadEndOfStream, Name = "BadEndOfStream", Explanation = "Cannot move beyond end of the stream." };

        /// <summary>
        ///   No data is currently available for reading from a non-blocking stream.
        /// </summary>
        public static readonly DataStatus BadNoDataAvailable                      
			= new DataStatus() { Code = (uint)Codes.BadNoDataAvailable, Name = "BadNoDataAvailable", Explanation = "No data is currently available for reading from a non-blocking stream." };

        /// <summary>
        ///   The asynchronous operation is waiting for a response.
        /// </summary>
        public static readonly DataStatus BadWaitingForResponse                   
			= new DataStatus() { Code = (uint)Codes.BadWaitingForResponse, Name = "BadWaitingForResponse", Explanation = "The asynchronous operation is waiting for a response." };

        /// <summary>
        ///   The asynchronous operation was abandoned by the caller.
        /// </summary>
        public static readonly DataStatus BadOperationAbandoned                   
			= new DataStatus() { Code = (uint)Codes.BadOperationAbandoned, Name = "BadOperationAbandoned", Explanation = "The asynchronous operation was abandoned by the caller." };

        /// <summary>
        ///   The stream did not return all data requested (possibly because it is a non-blocking stream).
        /// </summary>
        public static readonly DataStatus BadExpectedStreamToBlock                
			= new DataStatus() { Code = (uint)Codes.BadExpectedStreamToBlock, Name = "BadExpectedStreamToBlock", Explanation = "The stream did not return all data requested (possibly because it is a non-blocking stream)." };

        /// <summary>
        ///   Non blocking behaviour is required and the operation would block.
        /// </summary>
        public static readonly DataStatus BadWouldBlock                           
			= new DataStatus() { Code = (uint)Codes.BadWouldBlock, Name = "BadWouldBlock", Explanation = "Non blocking behaviour is required and the operation would block." };

        /// <summary>
        ///   A value had an invalid syntax.
        /// </summary>
        public static readonly DataStatus BadSyntaxError                          
			= new DataStatus() { Code = (uint)Codes.BadSyntaxError, Name = "BadSyntaxError", Explanation = "A value had an invalid syntax." };

        /// <summary>
        ///   The operation could not be finished because all available connections are in use.
        /// </summary>
        public static readonly DataStatus BadMaxConnectionsReached                
			= new DataStatus() { Code = (uint)Codes.BadMaxConnectionsReached, Name = "BadMaxConnectionsReached", Explanation = "The operation could not be finished because all available connections are in use." };

        #endregion
    }
}
