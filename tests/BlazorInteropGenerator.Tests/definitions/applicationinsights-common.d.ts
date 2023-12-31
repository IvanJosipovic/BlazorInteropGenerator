/*
 * Microsoft Application Insights Common JavaScript Library, 3.0.2
 * Copyright (c) Microsoft and contributors. All rights reserved.
 *
 * Microsoft Application Insights Team
 * https://github.com/microsoft/ApplicationInsights-JS#readme
 *
 * ---------------------------------------------------------------------------
 * This is a single combined (rollup) declaration file for the package,
 * if you require a namespace wrapped version it is also available.
 * - Namespaced version: types/applicationinsights-common.namespaced.d.ts
 * ---------------------------------------------------------------------------
 */

import { createTraceParent } from '@microsoft/applicationinsights-core-js';
import { _eInternalMessageId } from '@microsoft/applicationinsights-core-js';
import { eLoggingSeverity } from '@microsoft/applicationinsights-core-js';
import { EnumValue } from '@microsoft/applicationinsights-core-js';
import { findW3cTraceParent } from '@microsoft/applicationinsights-core-js';
import { formatTraceParent } from '@microsoft/applicationinsights-core-js';
import { IAppInsightsCore } from '@microsoft/applicationinsights-core-js';
import { IConfiguration } from '@microsoft/applicationinsights-core-js';
import { ICookieMgr } from '@microsoft/applicationinsights-core-js';
import { ICustomProperties } from '@microsoft/applicationinsights-core-js';
import { IDiagnosticLogger } from '@microsoft/applicationinsights-core-js';
import { IDistributedTraceContext } from '@microsoft/applicationinsights-core-js';
import { IPlugin } from '@microsoft/applicationinsights-core-js';
import { isBeaconsSupported as isBeaconApiSupported } from '@microsoft/applicationinsights-core-js';
import { isSampledFlag } from '@microsoft/applicationinsights-core-js';
import { isValidSpanId } from '@microsoft/applicationinsights-core-js';
import { isValidTraceId } from '@microsoft/applicationinsights-core-js';
import { isValidTraceParent } from '@microsoft/applicationinsights-core-js';
import { ITelemetryItem } from '@microsoft/applicationinsights-core-js';
import { ITraceParent } from '@microsoft/applicationinsights-core-js';
import { IUnloadHookContainer } from '@microsoft/applicationinsights-core-js';
import { parseTraceParent } from '@microsoft/applicationinsights-core-js';

/**
 * Data struct to contain only C section with custom fields.
 */
export declare interface AIBase {
    /**
     * Name of item (B section) if any. If telemetry data is derived straight from this, this should be null.
     */
    baseType: string;
}

/**
 * Data struct to contain both B and C sections.
 */
export declare interface AIData<TDomain> extends AIBase {
    /**
     * Name of item (B section) if any. If telemetry data is derived straight from this, this should be null.
     */
    baseType: string;
    /**
     * Container for data item (B section).
     */
    baseData: TDomain;
}

export declare const AnalyticsPluginIdentifier = "ApplicationInsightsAnalytics";

export declare const BreezeChannelIdentifier = "AppInsightsChannelPlugin";

export declare class ConfigurationManager {
    static getConfig(config: IConfiguration & IConfig, field: string, identifier?: string, defaultValue?: number | string | boolean): number | string | boolean;
}

declare type ConnectionString = {
    [key in ConnectionStringKey]?: string;
};

declare type ConnectionStringKey = "authorization" | "instrumentationkey" | "ingestionendpoint" | "location" | "endpointsuffix";

export declare const ConnectionStringParser: {
    parse: typeof parseConnectionString;
};

export declare class ContextTagKeys extends ContextTagKeys_base {
    constructor();
}

declare const ContextTagKeys_base: new () => IContextTagKeys;

/**
 * Checks if a request url is not on a excluded domain list and if it is safe to add correlation headers.
 * Headers are always included if the current domain matches the request domain. If they do not match (CORS),
 * they are regex-ed across correlationHeaderDomains and correlationHeaderExcludedDomains to determine if headers are included.
 * Some environments don't give information on currentHost via window.location.host (e.g. Cordova). In these cases, the user must
 * manually supply domains to include correlation headers on. Else, no headers will be included at all.
 */
export declare function correlationIdCanIncludeCorrelationHeader(config: ICorrelationConfig, requestUrl: string, currentHost?: string): boolean;

/**
 * Combines target appId and target role name from response header.
 */
export declare function correlationIdGetCorrelationContext(responseHeader: string): string;

/**
 * Gets key from correlation response header
 */
export declare function correlationIdGetCorrelationContextValue(responseHeader: string, key: string): string;

export declare function correlationIdGetPrefix(): string;

export declare function correlationIdSetPrefix(prefix: string): void;

/**
 * Creates a IDistributedTraceContext from an optional telemetryTrace
 * @param telemetryTrace - The telemetryTrace instance that is being wrapped
 * @param parentCtx - An optional parent distributed trace instance, almost always undefined as this scenario is only used in the case of multiple property handlers.
 * @returns A new IDistributedTraceContext instance that is backed by the telemetryTrace or temporary object
 */
export declare function createDistributedTraceContextFromTrace(telemetryTrace?: ITelemetryTrace, parentCtx?: IDistributedTraceContext): IDistributedTraceContext;

export declare function createDomEvent(eventName: string): Event;

/**
 * Create a telemetry item that the 1DS channel understands
 * @param item - domain specific properties; part B
 * @param baseType - telemetry item type. ie PageViewData
 * @param envelopeName - name of the envelope. ie Microsoft.ApplicationInsights.<instrumentation key>.PageView
 * @param customProperties - user defined custom properties; part C
 * @param systemProperties - system properties that are added to the context; part A
 * @returns ITelemetryItem that is sent to channel
 */
export declare function createTelemetryItem<T>(item: T, baseType: string, envelopeName: string, logger: IDiagnosticLogger, customProperties?: {
    [key: string]: any;
}, systemProperties?: {
    [key: string]: any;
}): ITelemetryItem;

export { createTraceParent }

export declare let CtxTagKeys: ContextTagKeys;

export declare class Data<TDomain> implements AIData<TDomain>, ISerializable {
    /**
     * The data contract for serializing this object.
     */
    aiDataContract: {
        baseType: FieldType;
        baseData: FieldType;
    };
    /**
     * Name of item (B section) if any. If telemetry data is derived straight from this, this should be null.
     */
    baseType: string;
    /**
     * Container for data item (B section).
     */
    baseData: TDomain;
    /**
     * Constructs a new instance of telemetry data.
     */
    constructor(baseType: string, data: TDomain);
}

declare class DataPoint implements IDataPoint, ISerializable {
    /**
     * The data contract for serializing this object.
     */
    aiDataContract: {
        name: FieldType;
        kind: FieldType;
        value: FieldType;
        count: FieldType;
        min: FieldType;
        max: FieldType;
        stdDev: FieldType;
    };
    /**
     * Name of the metric.
     */
    name: string;
    /**
     * Metric type. Single measurement or the aggregated value.
     */
    kind: DataPointType;
    /**
     * Single value for measurement. Sum of individual measurements for the aggregation.
     */
    value: number;
    /**
     * Metric weight of the aggregated metric. Should not be set for a measurement.
     */
    count: number;
    /**
     * Minimum value of the aggregated metric. Should not be set for a measurement.
     */
    min: number;
    /**
     * Maximum value of the aggregated metric. Should not be set for a measurement.
     */
    max: number;
    /**
     * Standard deviation of the aggregated metric. Should not be set for a measurement.
     */
    stdDev: number;
}

/**
 * Type of the metric data measurement.
 */
declare const enum DataPointType {
    Measurement = 0,
    Aggregation = 1
}

export declare function dataSanitizeException(logger: IDiagnosticLogger, exception: any): any;

export declare function dataSanitizeId(logger: IDiagnosticLogger, id: string): string;

export declare function dataSanitizeInput(logger: IDiagnosticLogger, input: any, maxLength: number, _msgId: _eInternalMessageId): any;

export declare function dataSanitizeKey(logger: IDiagnosticLogger, name: any): any;

export declare function dataSanitizeKeyAndAddUniqueness(logger: IDiagnosticLogger, key: any, map: any): any;

export declare function dataSanitizeMeasurements(logger: IDiagnosticLogger, measurements: any): any;

export declare function dataSanitizeMessage(logger: IDiagnosticLogger, message: any): any;

export declare function dataSanitizeProperties(logger: IDiagnosticLogger, properties: any): any;

export declare const enum DataSanitizerValues {
    /**
     * Max length allowed for custom names.
     */
    MAX_NAME_LENGTH = 150,
    /**
     * Max length allowed for Id field in page views.
     */
    MAX_ID_LENGTH = 128,
    /**
     * Max length allowed for custom values.
     */
    MAX_PROPERTY_LENGTH = 8192,
    /**
     * Max length allowed for names
     */
    MAX_STRING_LENGTH = 1024,
    /**
     * Max length allowed for url.
     */
    MAX_URL_LENGTH = 2048,
    /**
     * Max length allowed for messages.
     */
    MAX_MESSAGE_LENGTH = 32768,
    /**
     * Max length allowed for exceptions.
     */
    MAX_EXCEPTION_LENGTH = 32768
}

export declare function dataSanitizeString(logger: IDiagnosticLogger, value: any, maxLength?: number): any;

export declare function dataSanitizeUrl(logger: IDiagnosticLogger, url: any): any;

export declare function dateTimeUtilsDuration(start: number, end: number): number;

export declare function dateTimeUtilsNow(): number;

export declare const DEFAULT_BREEZE_ENDPOINT = "https://dc.services.visualstudio.com";

export declare const DEFAULT_BREEZE_PATH = "/v2/track";

/**
 * This is an internal property used to cause internal (reporting) requests to be ignored from reporting
 * additional telemetry, to handle polyfil implementations ALL urls used with a disabled request will
 * also be ignored for future requests even when this property is not provided.
 * Tagging as Ignore as this is an internal value and is not expected to be used outside of the SDK
 * @ignore
 */
export declare const DisabledPropertyName: string;

export declare const DistributedTracingModes: EnumValue<typeof eDistributedTracingModes>;

export declare type DistributedTracingModes = number | eDistributedTracingModes;

export declare function dsPadNumber(num: number): string;

export declare const enum eDistributedTracingModes {
    /**
     * (Default) Send Application Insights correlation headers
     */
    AI = 0,
    /**
     * Send both W3C Trace Context headers and back-compatibility Application Insights headers
     */
    AI_AND_W3C = 1,
    /**
     * Send W3C Trace Context headers
     */
    W3C = 2
}

export declare class Envelope implements IEnvelope {
    /**
     * The data contract for serializing this object.
     */
    aiDataContract: any;
    /**
     * Envelope version. For internal use only. By assigning this the default, it will not be serialized within the payload unless changed to a value other than #1.
     */
    ver: number;
    /**
     * Type name of telemetry data item.
     */
    name: string;
    /**
     * Event date time when telemetry item was created. This is the wall clock time on the client when the event was generated. There is no guarantee that the client's time is accurate. This field must be formatted in UTC ISO 8601 format, with a trailing 'Z' character, as described publicly on https://en.wikipedia.org/wiki/ISO_8601#UTC. Note: the number of decimal seconds digits provided are variable (and unspecified). Consumers should handle this, i.e. managed code consumers should not use format 'O' for parsing as it specifies a fixed length. Example: 2009-06-15T13:45:30.0000000Z.
     */
    time: string;
    /**
     * Sampling rate used in application. This telemetry item represents 1 / sampleRate actual telemetry items.
     */
    sampleRate: number;
    /**
     * Sequence field used to track absolute order of uploaded events.
     */
    seq: string;
    /**
     * The application's instrumentation key. The key is typically represented as a GUID, but there are cases when it is not a guid. No code should rely on iKey being a GUID. Instrumentation key is case insensitive.
     */
    iKey: string;
    /**
     * Key/value collection of context properties. See ContextTagKeys for information on available properties.
     */
    tags: any;
    /**
     * Telemetry data item.
     */
    data: AIBase;
    /**
     * Constructs a new instance of telemetry data.
     */
    constructor(logger: IDiagnosticLogger, data: AIBase, name: string);
}

export declare const enum eRequestHeaders {
    requestContextHeader = 0,
    requestContextTargetKey = 1,
    requestContextAppIdFormat = 2,
    requestIdHeader = 3,
    traceParentHeader = 4,
    traceStateHeader = 5,
    sdkContextHeader = 6,
    sdkContextHeaderAppIdRequest = 7,
    requestContextHeaderLowerCase = 8
}

/**
 * Defines the level of severity for the event.
 */
export declare const enum eSeverityLevel {
    Verbose = 0,
    Information = 1,
    Warning = 2,
    Error = 3,
    Critical = 4
}

declare class Event_2 implements IEventData, ISerializable {
    static envelopeType: string;
    static dataType: string;
    aiDataContract: {
        ver: FieldType;
        name: FieldType;
        properties: FieldType;
        measurements: FieldType;
    };
    /**
     * Schema version
     */
    ver: number;
    /**
     * Event name. Keep it low cardinality to allow proper grouping and useful metrics.
     */
    name: string;
    /**
     * Collection of custom properties.
     */
    properties: any;
    /**
     * Collection of custom measurements.
     */
    measurements: any;
    /**
     * Constructs a new instance of the EventTelemetry object
     */
    constructor(logger: IDiagnosticLogger, name: string, properties?: any, measurements?: any);
}
export { Event_2 as Event }

export declare class Exception implements IExceptionData, ISerializable {
    static envelopeType: string;
    static dataType: string;
    id?: string;
    problemGroup?: string;
    isManual?: boolean;
    aiDataContract: {
        ver: FieldType;
        exceptions: FieldType;
        severityLevel: FieldType;
        properties: FieldType;
        measurements: FieldType;
    };
    /**
     * Schema version
     */
    ver: number;
    /**
     * Exception chain - list of inner exceptions.
     */
    exceptions: IExceptionDetails[];
    /**
     * Severity level. Mostly used to indicate exception severity level when it is reported by logging library.
     */
    severityLevel: SeverityLevel;
    /**
     * Collection of custom properties.
     */
    properties: any;
    /**
     * Collection of custom measurements.
     */
    measurements: any;
    /**
     * Constructs a new instance of the ExceptionTelemetry object
     */
    constructor(logger: IDiagnosticLogger, exception: Error | IExceptionInternal | IAutoExceptionTelemetry, properties?: {
        [key: string]: any;
    }, measurements?: {
        [key: string]: number;
    }, severityLevel?: SeverityLevel, id?: string);
    static CreateAutoException(message: string | Event, url: string, lineNumber: number, columnNumber: number, error: any, evt?: Event | string, stack?: string, errorSrc?: string): IAutoExceptionTelemetry;
    static CreateFromInterface(logger: IDiagnosticLogger, exception: IExceptionInternal, properties?: any, measurements?: {
        [key: string]: number;
    }): Exception;
    toInterface(): IExceptionInternal;
    /**
     * Creates a simple exception with 1 stack frame. Useful for manual constracting of exception.
     */
    static CreateSimpleException(message: string, typeName: string, assembly: string, fileName: string, details: string, line: number): Exception;
    static formatError: typeof _formatErrorCode;
}

export declare const Extensions: {
    UserExt: string;
    DeviceExt: string;
    TraceExt: string;
    WebExt: string;
    AppExt: string;
    OSExt: string;
    SessionExt: string;
    SDKExt: string;
};

/**
 * Enum is used in aiDataContract to describe how fields are serialized.
 * For instance: (Fieldtype.Required | FieldType.Array) will mark the field as required and indicate it's an array
 */
export declare const enum FieldType {
    Default = 0,
    Required = 1,
    Array = 2,
    Hidden = 4
}

export { findW3cTraceParent }

/**
 * Formats the provided errorObj for display and reporting, it may be a String, Object, integer or undefined depending on the browser.
 * @param errorObj - The supplied errorObj
 */
declare function _formatErrorCode(errorObj: any): any;

export { formatTraceParent }

export declare function getExtensionByName(extensions: IPlugin[], identifier: string): IPlugin | null;

export declare const HttpMethod = "http.method";

export declare interface IAppInsights {
    /**
     * Get the current cookie manager for this instance
     */
    getCookieMgr(): ICookieMgr;
    trackEvent(event: IEventTelemetry, customProperties?: {
        [key: string]: any;
    }): void;
    trackPageView(pageView: IPageViewTelemetry, customProperties?: {
        [key: string]: any;
    }): void;
    trackException(exception: IExceptionTelemetry, customProperties?: {
        [key: string]: any;
    }): void;
    _onerror(exception: IAutoExceptionTelemetry): void;
    trackTrace(trace: ITraceTelemetry, customProperties?: {
        [key: string]: any;
    }): void;
    trackMetric(metric: IMetricTelemetry, customProperties?: {
        [key: string]: any;
    }): void;
    startTrackPage(name?: string): void;
    stopTrackPage(name?: string, url?: string, customProperties?: Object): void;
    startTrackEvent(name: string): void;
    stopTrackEvent(name: string, properties?: Object, measurements?: Object): void;
    addTelemetryInitializer(telemetryInitializer: (item: ITelemetryItem) => boolean | void): void;
    trackPageViewPerformance(pageViewPerformance: IPageViewPerformanceTelemetry, customProperties?: {
        [key: string]: any;
    }): void;
}

export declare interface IApplication {
    /**
     * The application version.
     */
    ver: string;
    /**
     * The application build version.
     */
    build: string;
}

/**
 * @description window.onerror function parameters
 * @export
 * @interface IAutoExceptionTelemetry
 */
export declare interface IAutoExceptionTelemetry {
    /**
     * @description error message. Available as event in HTML onerror="" handler
     * @type {string}
     * @memberof IAutoExceptionTelemetry
     */
    message: string;
    /**
     * @description URL of the script where the error was raised
     * @type {string}
     * @memberof IAutoExceptionTelemetry
     */
    url: string;
    /**
     * @description Line number where error was raised
     * @type {number}
     * @memberof IAutoExceptionTelemetry
     */
    lineNumber: number;
    /**
     * @description Column number for the line where the error occurred
     * @type {number}
     * @memberof IAutoExceptionTelemetry
     */
    columnNumber: number;
    /**
     * @description Error Object (object)
     * @type {any}
     * @memberof IAutoExceptionTelemetry
     */
    error: any;
    /**
     * @description The event at the time of the exception (object)
     * @type {Event|string}
     * @memberof IAutoExceptionTelemetry
     */
    evt?: Event | string;
    /**
     * @description The provided stack for the error
     * @type {IStackDetails}
     * @memberof IAutoExceptionTelemetry
     */
    stackDetails?: IStackDetails;
    /**
     * @description The calculated type of the error
     * @type {string}
     * @memberof IAutoExceptionTelemetry
     */
    typeName?: string;
    /**
     * @description The descriptive source of the error
     * @type {string}
     * @memberof IAutoExceptionTelemetry
     */
    errorSrc?: string;
}

/**
 * Configuration settings for how telemetry is sent
 * @export
 * @interface IConfig
 */
export declare interface IConfig {
    /**
     * The JSON format (normal vs line delimited). True means line delimited JSON.
     */
    emitLineDelimitedJson?: boolean;
    /**
     * An optional account id, if your app groups users into accounts. No spaces, commas, semicolons, equals, or vertical bars.
     */
    accountId?: string;
    /**
     * A session is logged if the user is inactive for this amount of time in milliseconds. Default 30 mins.
     * @default 30*60*1000
     */
    sessionRenewalMs?: number;
    /**
     * A session is logged if it has continued for this amount of time in milliseconds. Default 24h.
     * @default 24*60*60*1000
     */
    sessionExpirationMs?: number;
    /**
     * Max size of telemetry batch. If batch exceeds limit, it is sent and a new batch is started
     * @default 100000
     */
    maxBatchSizeInBytes?: number;
    /**
     * How long to batch telemetry for before sending (milliseconds)
     * @default 15 seconds
     */
    maxBatchInterval?: number;
    /**
     * If true, debugging data is thrown as an exception by the logger. Default false
     * @defaultValue false
     */
    enableDebug?: boolean;
    /**
     * If true, exceptions are not autocollected. Default is false
     * @defaultValue false
     */
    disableExceptionTracking?: boolean;
    /**
     * If true, telemetry is not collected or sent. Default is false
     * @defaultValue false
     */
    disableTelemetry?: boolean;
    /**
     * Percentage of events that will be sent. Default is 100, meaning all events are sent.
     * @defaultValue 100
     */
    samplingPercentage?: number;
    /**
     * If true, on a pageview, the previous instrumented page's view time is tracked and sent as telemetry and a new timer is started for the current pageview. It is sent as a custom metric named PageVisitTime in milliseconds and is calculated via the Date now() function (if available) and falls back to (new Date()).getTime() if now() is unavailable (IE8 or less). Default is false.
     */
    autoTrackPageVisitTime?: boolean;
    /**
     * Automatically track route changes in Single Page Applications (SPA). If true, each route change will send a new Pageview to Application Insights.
     */
    enableAutoRouteTracking?: boolean;
    /**
     * If true, Ajax calls are not autocollected. Default is false
     * @defaultValue false
     */
    disableAjaxTracking?: boolean;
    /**
     * If true, Fetch requests are not autocollected. Default is false (Since 2.8.0, previously true).
     * @defaultValue false
     */
    disableFetchTracking?: boolean;
    /**
     * Provide a way to exclude specific route from automatic tracking for XMLHttpRequest or Fetch request. For an ajax / fetch request that the request url matches with the regex patterns, auto tracking is turned off.
     * @defaultValue undefined.
     */
    excludeRequestFromAutoTrackingPatterns?: string[] | RegExp[];
    /**
     * Provide a way to enrich dependencies logs with context at the beginning of api call.
     * Default is undefined.
     */
    addRequestContext?: (requestContext?: IRequestContext) => ICustomProperties;
    /**
     * If true, default behavior of trackPageView is changed to record end of page view duration interval when trackPageView is called. If false and no custom duration is provided to trackPageView, the page view performance is calculated using the navigation timing API. Default is false
     * @defaultValue false
     */
    overridePageViewDuration?: boolean;
    /**
     * Default 500 - controls how many ajax calls will be monitored per page view. Set to -1 to monitor all (unlimited) ajax calls on the page.
     */
    maxAjaxCallsPerView?: number;
    /**
     * @ignore
     * If false, internal telemetry sender buffers will be checked at startup for items not yet sent. Default is true
     * @defaultValue true
     */
    disableDataLossAnalysis?: boolean;
    /**
     * If false, the SDK will add two headers ('Request-Id' and 'Request-Context') to all dependency requests to correlate them with corresponding requests on the server side. Default is false.
     * @defaultValue false
     */
    disableCorrelationHeaders?: boolean;
    /**
     * Sets the distributed tracing mode. If AI_AND_W3C mode or W3C mode is set, W3C trace context headers (traceparent/tracestate) will be generated and included in all outgoing requests.
     * AI_AND_W3C is provided for back-compatibility with any legacy Application Insights instrumented services
     * @defaultValue AI_AND_W3C
     */
    distributedTracingMode?: DistributedTracingModes;
    /**
     * Disable correlation headers for specific domain
     */
    correlationHeaderExcludedDomains?: string[];
    /**
     * Default false. If true, flush method will not be called when onBeforeUnload, onUnload, onPageHide or onVisibilityChange (hidden state) event(s) trigger.
     */
    disableFlushOnBeforeUnload?: boolean;
    /**
     * Default value of {@link #disableFlushOnBeforeUnload}. If true, flush method will not be called when onPageHide or onVisibilityChange (hidden state) event(s) trigger.
     */
    disableFlushOnUnload?: boolean;
    /**
     * [Optional] An array of the page unload events that you would like to be ignored, special note there must be at least one valid unload
     * event hooked, if you list all or the runtime environment only supports a listed "disabled" event it will still be hooked if required by the SDK.
     * (Some page unload functionality may be disabled via disableFlushOnBeforeUnload or disableFlushOnUnload config entries)
     * Unload events include "beforeunload", "unload", "visibilitychange" (with 'hidden' state) and "pagehide"
     */
    disablePageUnloadEvents?: string[];
    /**
     * [Optional] An array of page show events that you would like to be ignored, special note there must be at lease one valid show event
     * hooked, if you list all or the runtime environment only supports a listed (disabled) event it will STILL be hooked if required by the SDK.
     * Page Show events include "pageshow" and "visibilitychange" (with 'visible' state)
     */
    disablePageShowEvents?: string[];
    /**
     * If true, the buffer with all unsent telemetry is stored in session storage. The buffer is restored on page load. Default is true.
     * @defaultValue true
     */
    enableSessionStorageBuffer?: boolean;
    /**
     * If specified, overrides the storage & retrieval mechanism that is used to manage unsent telemetry.
     */
    bufferOverride?: IStorageBuffer;
    /**
     * @deprecated Use either disableCookiesUsage or specify a cookieCfg with the enabled value set.
     * If true, the SDK will not store or read any data from cookies. Default is false. As this field is being deprecated, when both
     * isCookieUseDisabled and disableCookiesUsage are used disableCookiesUsage will take precedent.
     * @defaultValue false
     */
    isCookieUseDisabled?: boolean;
    /**
     * If true, the SDK will not store or read any data from cookies. Default is false.
     * If you have also specified a cookieCfg then enabled property (if specified) will take precedent over this value.
     * @defaultValue false
     */
    disableCookiesUsage?: boolean;
    /**
     * Custom cookie domain. This is helpful if you want to share Application Insights cookies across subdomains.
     * @defaultValue ""
     */
    cookieDomain?: string;
    /**
     * Custom cookie path. This is helpful if you want to share Application Insights cookies behind an application gateway.
     * @defaultValue ""
     */
    cookiePath?: string;
    /**
     * Default false. If false, retry on 206 (partial success), 408 (timeout), 429 (too many requests), 500 (internal server error), 503 (service unavailable), and 0 (offline, only if detected)
     * @description
     * @defaultValue false
     */
    isRetryDisabled?: boolean;
    /**
     * @deprecated Used when initizialing from snippet only.
     *  The url from where the JS SDK will be downloaded.
     */
    url?: string;
    /**
     * If true, the SDK will not store or read any data from local and session storage. Default is false.
     * @defaultValue false
     */
    isStorageUseDisabled?: boolean;
    /**
     * If false, the SDK will send all telemetry using the [Beacon API](https://www.w3.org/TR/beacon)
     * @defaultValue true
     */
    isBeaconApiDisabled?: boolean;
    /**
     * Don't use XMLHttpRequest or XDomainRequest (for IE < 9) by default instead attempt to use fetch() or sendBeacon.
     * If no other transport is available it will still use XMLHttpRequest
     */
    disableXhr?: boolean;
    /**
     * If fetch keepalive is supported do not use it for sending events during unload, it may still fallback to fetch() without keepalive
     */
    onunloadDisableFetch?: boolean;
    /**
     * Sets the sdk extension name. Only alphabetic characters are allowed. The extension name is added as a prefix to the 'ai.internal.sdkVersion' tag (e.g. 'ext_javascript:2.0.0'). Default is null.
     * @defaultValue null
     */
    sdkExtension?: string;
    /**
     * Default is false. If true, the SDK will track all [Browser Link](https://docs.microsoft.com/en-us/aspnet/core/client-side/using-browserlink) requests.
     * @defaultValue false
     */
    isBrowserLinkTrackingEnabled?: boolean;
    /**
     * AppId is used for the correlation between AJAX dependencies happening on the client-side with the server-side requets. When Beacon API is enabled, it cannot be used automatically, but can be set manually in the configuration. Default is null
     * @defaultValue null
     */
    appId?: string;
    /**
     * If true, the SDK will add two headers ('Request-Id' and 'Request-Context') to all CORS requests to correlate outgoing AJAX dependencies with corresponding requests on the server side. Default is false
     * @defaultValue false
     */
    enableCorsCorrelation?: boolean;
    /**
     * An optional value that will be used as name postfix for localStorage and session cookie name.
     * @defaultValue null
     */
    namePrefix?: string;
    /**
     * An optional value that will be used as name postfix for session cookie name. If undefined, namePrefix is used as name postfix for session cookie name.
     * @defaultValue null
     */
    sessionCookiePostfix?: string;
    /**
     * An optional value that will be used as name postfix for user cookie name. If undefined, no postfix is added on user cookie name.
     * @defaultValue null
     */
    userCookiePostfix?: string;
    /**
     * An optional value that will track Request Header through trackDependency function.
     * @defaultValue false
     */
    enableRequestHeaderTracking?: boolean;
    /**
     * An optional value that will track Response Header through trackDependency function.
     * @defaultValue false
     */
    enableResponseHeaderTracking?: boolean;
    /**
     * An optional value that will track Response Error data through trackDependency function.
     * @defaultValue false
     */
    enableAjaxErrorStatusText?: boolean;
    /**
     * Flag to enable looking up and including additional browser window.performance timings
     * in the reported ajax (XHR and fetch) reported metrics.
     * Defaults to false.
     */
    enableAjaxPerfTracking?: boolean;
    /**
     * The maximum number of times to look for the window.performance timings (if available), this
     * is required as not all browsers populate the window.performance before reporting the
     * end of the XHR request and for fetch requests this is added after its complete
     * Defaults to 3
     */
    maxAjaxPerfLookupAttempts?: number;
    /**
     * The amount of time to wait before re-attempting to find the windows.performance timings
     * for an ajax request, time is in milliseconds and is passed directly to setTimeout()
     * Defaults to 25.
     */
    ajaxPerfLookupDelay?: number;
    /**
     * Default false. when tab is closed, the SDK will send all remaining telemetry using the [Beacon API](https://www.w3.org/TR/beacon)
     * @defaultValue false
     */
    onunloadDisableBeacon?: boolean;
    /**
     * @ignore
     * Internal only
     */
    autoExceptionInstrumented?: boolean;
    /**
     *
     */
    correlationHeaderDomains?: string[];
    /**
     * @ignore
     * Internal only
     */
    autoUnhandledPromiseInstrumented?: boolean;
    /**
     * Default false. Define whether to track unhandled promise rejections and report as JS errors.
     * When disableExceptionTracking is enabled (dont track exceptions) this value will be false.
     * @defaultValue false
     */
    enableUnhandledPromiseRejectionTracking?: boolean;
    /**
     * Disable correlation headers using regular expressions
     */
    correlationHeaderExcludePatterns?: RegExp[];
    /**
     * The ability for the user to provide extra headers
     */
    customHeaders?: [{
        header: string;
        value: string;
    }];
    /**
     * Provide user an option to convert undefined field to user defined value.
     */
    convertUndefined?: any;
    /**
     * [Optional] The number of events that can be kept in memory before the SDK starts to drop events. By default, this is 10,000.
     */
    eventsLimitInMem?: number;
    /**
     * [Optional] Disable iKey deprecation error message.
     * @defaultValue true
     */
    disableIkeyDeprecationMessage?: boolean;
    /**
     * [Optional] Flag to indicate whether the internal looking endpoints should be automatically
     * added to the `excludeRequestFromAutoTrackingPatterns` collection. (defaults to true).
     * This flag exists as the provided regex is generic and may unexpectedly match a domain that
     * should not be excluded.
     */
    addIntEndpoints?: boolean;
}

export declare interface IContextTagKeys {
    /**
     * Application version. Information in the application context fields is always about the application that is sending the telemetry.
     */
    readonly applicationVersion: string;
    /**
     * Application build.
     */
    readonly applicationBuild: string;
    /**
     * Application type id.
     */
    readonly applicationTypeId: string;
    /**
     * Application id.
     */
    readonly applicationId: string;
    /**
     * Application layer.
     */
    readonly applicationLayer: string;
    /**
     * Unique client device id. Computer name in most cases.
     */
    readonly deviceId: string;
    readonly deviceIp: string;
    readonly deviceLanguage: string;
    /**
     * Device locale using <language>-<REGION> pattern, following RFC 5646. Example 'en-US'.
     */
    readonly deviceLocale: string;
    /**
     * Model of the device the end user of the application is using. Used for client scenarios. If this field is empty then it is derived from the user agent.
     */
    readonly deviceModel: string;
    readonly deviceFriendlyName: string;
    readonly deviceNetwork: string;
    readonly deviceNetworkName: string;
    /**
     * Client device OEM name taken from the browser.
     */
    readonly deviceOEMName: string;
    readonly deviceOS: string;
    /**
     * Operating system name and version of the device the end user of the application is using. If this field is empty then it is derived from the user agent. Example 'Windows 10 Pro 10.0.10586.0'
     */
    readonly deviceOSVersion: string;
    /**
     * Name of the instance where application is running. Computer name for on-premisis, instance name for Azure.
     */
    readonly deviceRoleInstance: string;
    /**
     * Name of the role application is part of. Maps directly to the role name in azure.
     */
    readonly deviceRoleName: string;
    readonly deviceScreenResolution: string;
    /**
     * The type of the device the end user of the application is using. Used primarily to distinguish JavaScript telemetry from server side telemetry. Examples: 'PC', 'Phone', 'Browser'. 'PC' is the default value.
     */
    readonly deviceType: string;
    readonly deviceMachineName: string;
    readonly deviceVMName: string;
    readonly deviceBrowser: string;
    /**
     * The browser name and version as reported by the browser.
     */
    readonly deviceBrowserVersion: string;
    /**
     * The IP address of the client device. IPv4 and IPv6 are supported. Information in the location context fields is always about the end user. When telemetry is sent from a service, the location context is about the user that initiated the operation in the service.
     */
    readonly locationIp: string;
    /**
     * The country of the client device. If any of Country, Province, or City is specified, those values will be preferred over geolocation of the IP address field. Information in the location context fields is always about the end user. When telemetry is sent from a service, the location context is about the user that initiated the operation in the service.
     */
    readonly locationCountry: string;
    /**
     * The province/state of the client device. If any of Country, Province, or City is specified, those values will be preferred over geolocation of the IP address field. Information in the location context fields is always about the end user. When telemetry is sent from a service, the location context is about the user that initiated the operation in the service.
     */
    readonly locationProvince: string;
    /**
     * The city of the client device. If any of Country, Province, or City is specified, those values will be preferred over geolocation of the IP address field. Information in the location context fields is always about the end user. When telemetry is sent from a service, the location context is about the user that initiated the operation in the service.
     */
    readonly locationCity: string;
    /**
     * A unique identifier for the operation instance. The operation.id is created by either a request or a page view. All other telemetry sets this to the value for the containing request or page view. Operation.id is used for finding all the telemetry items for a specific operation instance.
     */
    readonly operationId: string;
    /**
     * The name (group) of the operation. The operation.name is created by either a request or a page view. All other telemetry items set this to the value for the containing request or page view. Operation.name is used for finding all the telemetry items for a group of operations (i.e. 'GET Home/Index').
     */
    readonly operationName: string;
    /**
     * The unique identifier of the telemetry item's immediate parent.
     */
    readonly operationParentId: string;
    readonly operationRootId: string;
    /**
     * Name of synthetic source. Some telemetry from the application may represent a synthetic traffic. It may be web crawler indexing the web site, site availability tests or traces from diagnostic libraries like Application Insights SDK itself.
     */
    readonly operationSyntheticSource: string;
    /**
     * The correlation vector is a light weight vector clock which can be used to identify and order related events across clients and services.
     */
    readonly operationCorrelationVector: string;
    /**
     * Session ID - the instance of the user's interaction with the app. Information in the session context fields is always about the end user. When telemetry is sent from a service, the session context is about the user that initiated the operation in the service.
     */
    readonly sessionId: string;
    /**
     * Boolean value indicating whether the session identified by ai.session.id is first for the user or not.
     */
    readonly sessionIsFirst: string;
    readonly sessionIsNew: string;
    readonly userAccountAcquisitionDate: string;
    /**
     * In multi-tenant applications this is the account ID or name which the user is acting with. Examples may be subscription ID for Azure portal or blog name blogging platform.
     */
    readonly userAccountId: string;
    /**
     * The browser's user agent string as reported by the browser. This property will be used to extract informaiton regarding the customer's browser but will not be stored. Use custom properties to store the original user agent.
     */
    readonly userAgent: string;
    /**
     * Anonymous user id. Represents the end user of the application. When telemetry is sent from a service, the user context is about the user that initiated the operation in the service.
     */
    readonly userId: string;
    /**
     * Store region for UWP applications.
     */
    readonly userStoreRegion: string;
    /**
     * Authenticated user id. The opposite of ai.user.id, this represents the user with a friendly name. Since it's PII information it is not collected by default by most SDKs.
     */
    readonly userAuthUserId: string;
    readonly userAnonymousUserAcquisitionDate: string;
    readonly userAuthenticatedUserAcquisitionDate: string;
    readonly cloudName: string;
    /**
     * Name of the role the application is a part of. Maps directly to the role name in azure.
     */
    readonly cloudRole: string;
    readonly cloudRoleVer: string;
    /**
     * Name of the instance where the application is running. Computer name for on-premisis, instance name for Azure.
     */
    readonly cloudRoleInstance: string;
    readonly cloudEnvironment: string;
    readonly cloudLocation: string;
    readonly cloudDeploymentUnit: string;
    /**
     * SDK version. See https://github.com/microsoft/ApplicationInsights-Home/blob/master/SDK-AUTHORING.md#sdk-version-specification for information.
     */
    readonly internalSdkVersion: string;
    /**
     * Agent version. Used to indicate the version of StatusMonitor installed on the computer if it is used for data collection.
     */
    readonly internalAgentVersion: string;
    /**
     * This is the node name used for billing purposes. Use it to override the standard detection of nodes.
     */
    readonly internalNodeName: string;
    /**
     * This identifies the version of the snippet that was used to initialize the SDK
     */
    readonly internalSnippet: string;
    /**
     * This identifies the source of the Sdk script (used to identify whether the SDK was loaded via the CDN)
     */
    readonly internalSdkSrc: string;
}

export declare interface ICorrelationConfig {
    enableCorsCorrelation: boolean;
    correlationHeaderExcludedDomains: string[];
    correlationHeaderExcludePatterns?: RegExp[];
    disableCorrelationHeaders: boolean;
    distributedTracingMode: DistributedTracingModes;
    maxAjaxCallsPerView: number;
    disableAjaxTracking: boolean;
    disableFetchTracking: boolean;
    appId?: string;
    enableRequestHeaderTracking?: boolean;
    enableResponseHeaderTracking?: boolean;
    enableAjaxErrorStatusText?: boolean;
    /**
     * Flag to enable looking up and including additional browser window.performance timings
     * in the reported ajax (XHR and fetch) reported metrics.
     * Defaults to false.
     */
    enableAjaxPerfTracking?: boolean;
    /**
     * The maximum number of times to look for the window.performance timings (if available), this
     * is required as not all browsers populate the window.performance before reporting the
     * end of the XHR request and for fetch requests this is added after its complete
     * Defaults to 3
     */
    maxAjaxPerfLookupAttempts?: number;
    /**
     * The amount of time to wait before re-attempting to find the windows.performance timings
     * for an ajax request, time is in milliseconds and is passed directly to setTimeout()
     * Defaults to 25.
     */
    ajaxPerfLookupDelay?: number;
    correlationHeaderDomains?: string[];
    /**
     * Response and request headers to be excluded from ajax tracking data.
     */
    ignoreHeaders?: string[];
    /**
     * Provide a way to exclude specific route from automatic tracking for XMLHttpRequest or Fetch request.
     * For an ajax / fetch request that the request url matches with the regex patterns, auto tracking is turned off.
     * Default is undefined.
     */
    excludeRequestFromAutoTrackingPatterns?: string[] | RegExp[];
    /**
     * Provide a way to enrich dependencies logs with context at the beginning of api call.
     * Default is undefined.
     */
    addRequestContext?: (requestContext?: IRequestContext) => ICustomProperties;
    /**
     * [Optional] Flag to indicate whether the internal looking endpoints should be automatically
     * added to the `excludeRequestFromAutoTrackingPatterns` collection. (defaults to true).
     * This flag exists as the provided regex is generic and may unexpectedly match a domain that
     * should not be excluded.
     */
    addIntEndpoints?: boolean;
}

/**
 * Metric data single measurement.
 */
declare interface IDataPoint {
    /**
     * Name of the metric.
     */
    name: string;
    /**
     * Metric type. Single measurement or the aggregated value.
     */
    kind: DataPointType;
    /**
     * Single value for measurement. Sum of individual measurements for the aggregation.
     */
    value: number;
    /**
     * Metric weight of the aggregated metric. Should not be set for a measurement.
     */
    count: number;
    /**
     * Minimum value of the aggregated metric. Should not be set for a measurement.
     */
    min: number;
    /**
     * Maximum value of the aggregated metric. Should not be set for a measurement.
     */
    max: number;
    /**
     * Standard deviation of the aggregated metric. Should not be set for a measurement.
     */
    stdDev: number;
}

/**
 * DependencyTelemetry telemetry interface
 */
export declare interface IDependencyTelemetry extends IPartC {
    id: string;
    name?: string;
    duration?: number;
    success?: boolean;
    startTime?: Date;
    responseCode: number;
    correlationContext?: string;
    type?: string;
    data?: string;
    target?: string;
    iKey?: string;
}

export declare interface IDevice {
    /**
     * The type for the current device.
     */
    deviceClass: string;
    /**
     * A device unique ID.
     */
    id: string;
    /**
     * The device model for the current device.
     */
    model: string;
    /**
     * The application screen resolution.
     */
    resolution: string;
    /**
     * The IP address.
     */
    ip: string;
}

/**
 * The abstract common base of all domains.
 */
declare interface IDomain {
}

export declare interface IEnvelope extends ISerializable {
    /**
     * Envelope version. For internal use only. By assigning this the default, it will not be serialized within the payload unless changed to a value other than #1.
     */
    ver: number;
    /**
     * Type name of telemetry data item.
     */
    name: string;
    /**
     * Event date time when telemetry item was created. This is the wall clock time on the client when the event was generated. There is no guarantee that the client's time is accurate. This field must be formatted in UTC ISO 8601 format, with a trailing 'Z' character, as described publicly on https://en.wikipedia.org/wiki/ISO_8601#UTC. Note: the number of decimal seconds digits provided are variable (and unspecified). Consumers should handle this, i.e. managed code consumers should not use format 'O' for parsing as it specifies a fixed length. Example: 2009-06-15T13:45:30.0000000Z.
     */
    time: string;
    /**
     * Sampling rate used in application. This telemetry item represents 1 / sampleRate actual telemetry items.
     */
    sampleRate: number;
    /**
     * Sequence field used to track absolute order of uploaded events.
     */
    seq: string;
    /**
     * The application's instrumentation key. The key is typically represented as a GUID, but there are cases when it is not a guid. No code should rely on iKey being a GUID. Instrumentation key is case insensitive.
     */
    iKey: string;
    /**
     * Key/value collection of context properties. See ContextTagKeys for information on available properties.
     */
    tags: {
        [name: string]: any;
    };
    /**
     * Telemetry data item.
     */
    data: any;
}

/**
 * Instances of Event represent structured event records that can be grouped and searched by their properties. Event data item also creates a metric of event count by name.
 */
declare interface IEventData extends IDomain {
    /**
     * Schema version
     */
    ver: number;
    /**
     * Event name. Keep it low cardinality to allow proper grouping and useful metrics.
     */
    name: string;
    /**
     * Collection of custom properties.
     */
    properties: any;
    /**
     * Collection of custom measurements.
     */
    measurements: any;
}

export declare interface IEventTelemetry extends IPartC {
    /**
     * @description An event name string
     * @type {string}
     * @memberof IEventTelemetry
     */
    name: string;
    /**
     * @description custom defined iKey
     * @type {string}
     * @memberof IEventTelemetry
     */
    iKey?: string;
}

/**
 * An instance of Exception represents a handled or unhandled exception that occurred during execution of the monitored application.
 */
declare interface IExceptionData extends IDomain {
    /**
     * Schema version
     */
    ver: number;
    /**
     * Exception chain - list of inner exceptions.
     */
    exceptions: IExceptionDetails[];
    /**
     * Severity level. Mostly used to indicate exception severity level when it is reported by logging library.
     */
    severityLevel: SeverityLevel;
    /**
     * Collection of custom properties.
     */
    properties: any;
    /**
     * Collection of custom measurements.
     */
    measurements: any;
}

/**
 * Exception details of the exception in a chain.
 */
declare interface IExceptionDetails {
    /**
     * In case exception is nested (outer exception contains inner one), the id and outerId properties are used to represent the nesting.
     */
    id: number;
    /**
     * The value of outerId is a reference to an element in ExceptionDetails that represents the outer exception
     */
    outerId: number;
    /**
     * Exception type name.
     */
    typeName: string;
    /**
     * Exception message.
     */
    message: string;
    /**
     * Indicates if full exception stack is provided in the exception. The stack may be trimmed, such as in the case of a StackOverflow exception.
     */
    hasFullStack: boolean;
    /**
     * Text describing the stack. Either stack or parsedStack should have a value.
     */
    stack: string;
    /**
     * List of stack frames. Either stack or parsedStack should have a value.
     */
    parsedStack: IStackFrame[];
}

declare interface IExceptionDetailsInternal {
    id: number;
    outerId: number;
    typeName: string;
    message: string;
    hasFullStack: boolean;
    stack: string;
    parsedStack: IExceptionStackFrameInternal[];
}

export declare interface IExceptionInternal extends IPartC {
    ver: string;
    id: string;
    exceptions: IExceptionDetailsInternal[];
    severityLevel?: SeverityLevel | number;
    problemGroup: string;
    isManual: boolean;
}

declare interface IExceptionStackFrameInternal {
    level: number;
    method: string;
    assembly: string;
    fileName: string;
    line: number;
    pos?: number;
}

/**
 * @export
 * @interface IExceptionTelemetry
 * @description Exception interface used as primary parameter to trackException
 */
export declare interface IExceptionTelemetry extends IPartC {
    /**
     * Unique guid identifying this error
     */
    id?: string;
    /**
     * @deprecated
     * @type {Error}
     * @memberof IExceptionTelemetry
     * @description DEPRECATED: Please use exception instead. Behavior/usage for exception remains the same as this field.
     */
    error?: Error;
    /**
     * @type {Error}
     * @memberof IExceptionTelemetry
     * @description Error Object(s)
     */
    exception?: IAutoExceptionTelemetry;
    /**
     * @description Specified severity of exception for use with
     * telemetry filtering in dashboard
     * @type {(SeverityLevel | number)}
     * @memberof IExceptionTelemetry
     */
    severityLevel?: SeverityLevel;
}

export declare interface IInternal {
    /**
     * The SDK version used to create this telemetry item.
     */
    sdkVersion: string;
    /**
     * The SDK agent version.
     */
    agentVersion: string;
    /**
     * The Snippet version used to initialize the sdk instance, this will contain either
     * undefined/null - Snippet not used
     * '-' - Version and legacy mode not determined
     * # - Version # of the snippet
     * #.l - Version # in legacy mode
     * .l - No defined version, but used legacy mode initialization
     */
    snippetVer: string;
    /**
     * Identifies the source of the sdk script
     */
    sdkSrc: string;
}

export declare interface ILocation {
    /**
     * Client IP address for reverse lookup
     */
    ip: string;
}

/**
 * Instances of Message represent printf-like trace statements that are text-searched. Log4Net, NLog and other text-based log file entries are translated into intances of this type. The message does not have measurements.
 */
declare interface IMessageData extends IDomain {
    /**
     * Schema version
     */
    ver: number;
    /**
     * Trace message
     */
    message: string;
    /**
     * Trace severity level.
     */
    severityLevel: SeverityLevel;
    /**
     * Collection of custom properties.
     */
    properties: any;
    /**
     * Collection of custom measurements.
     */
    measurements: any;
}

/**
 * An instance of the Metric item is a list of measurements (single data points) and/or aggregations.
 */
declare interface IMetricData extends IDomain {
    /**
     * Schema version
     */
    ver: number;
    /**
     * List of metrics. Only one metric in the list is currently supported by Application Insights storage. If multiple data points were sent only the first one will be used.
     */
    metrics: IDataPoint[];
    /**
     * Collection of custom properties.
     */
    properties: any;
    /**
     * Collection of custom measurements.
     */
    measurements: any;
}

export declare interface IMetricTelemetry extends IPartC {
    /**
     * @description (required) - name of this metric
     * @type {string}
     * @memberof IMetricTelemetry
     */
    name: string;
    /**
     * @description (required) - Recorded value/average for this metric
     * @type {number}
     * @memberof IMetricTelemetry
     */
    average: number;
    /**
     * @description (optional) Number of samples represented by the average.
     * @type {number=}
     * @memberof IMetricTelemetry
     * @default sampleCount=1
     */
    sampleCount?: number;
    /**
     * @description (optional) The smallest measurement in the sample. Defaults to the average
     * @type {number}
     * @memberof IMetricTelemetry
     * @default min=average
     */
    min?: number;
    /**
     * @description (optional) The largest measurement in the sample. Defaults to the average.
     * @type {number}
     * @memberof IMetricTelemetry
     * @default max=average
     */
    max?: number;
    /**
     * (optional) The standard deviation measurement in the sample, Defaults to undefined which results in zero.
     */
    stdDev?: number;
    /**
     * @description custom defined iKey
     * @type {string}
     * @memberof IMetricTelemetry
     */
    iKey?: string;
}

export declare interface IOperatingSystem {
    name: string;
}

/**
 * An instance of PageView represents a generic action on a page like a button click. It is also the base type for PageView.
 */
export declare interface IPageViewData extends IEventData {
    /**
     * Request URL with all query string parameters
     */
    url: string;
    /**
     * Request duration in format: DD.HH:MM:SS.MMMMMM. For a page view (PageViewData), this is the duration. For a page view with performance information (PageViewPerfData), this is the page load time. Must be less than 1000 days.
     */
    duration: string;
    /**
     * Identifier of a page view instance. Used for correlation between page view and other telemetry items.
     */
    id: string;
}

/**
 * An instance of PageViewPerf represents: a page view with no performance data, a page view with performance data, or just the performance data of an earlier page request.
 */
declare interface IPageViewPerfData extends IPageViewData {
    /**
     * Performance total in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff
     */
    perfTotal: string;
    /**
     * Network connection time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff
     */
    networkConnect: string;
    /**
     * Sent request time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff
     */
    sentRequest: string;
    /**
     * Received response time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff
     */
    receivedResponse: string;
    /**
     * DOM processing time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff
     */
    domProcessing: string;
}

export declare interface IPageViewPerformanceTelemetry extends IPartC {
    /**
     * name String - The name of the page. Defaults to the document title.
     */
    name?: string;
    /**
     * url String - a relative or absolute URL that identifies the page or other item. Defaults to the window location.
     */
    uri?: string;
    /**
     * Performance total in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff". This is total duration in timespan format.
     */
    perfTotal?: string;
    /**
     * Performance total in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff". This represents the total page load time.
     */
    duration?: string;
    /**
     * Sent request time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff
     */
    networkConnect?: string;
    /**
     * Sent request time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff.
     */
    sentRequest?: string;
    /**
     * Received response time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff.
     */
    receivedResponse?: string;
    /**
     * DOM processing time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff
     */
    domProcessing?: string;
}

export declare interface IPageViewPerformanceTelemetryInternal extends IPageViewPerformanceTelemetry {
    /**
     * An identifier assigned to each distinct impression for the purposes of correlating with pageview.
     * A new id is automatically generated on each pageview. You can manually specify this field if you
     * want to use a specific value instead.
     */
    id?: string;
    /**
     * Version of the part B schema, todo: set this value in trackpageView
     */
    ver?: string;
    /**
     * Field indicating whether this instance of PageViewPerformance is valid and should be sent
     */
    isValid?: boolean;
    /**
     * Duration in miliseconds
     */
    durationMs?: number;
}

/**
 * Pageview telemetry interface
 */
export declare interface IPageViewTelemetry extends IPartC {
    /**
     * name String - The string you used as the name in startTrackPage. Defaults to the document title.
     */
    name?: string;
    /**
     * uri  String - a relative or absolute URL that identifies the page or other item. Defaults to the window location.
     */
    uri?: string;
    /**
     * refUri  String - the URL of the source page where current page is loaded from
     */
    refUri?: string;
    /**
     * pageType  String - page type
     */
    pageType?: string;
    /**
     * isLoggedIn - boolean is user logged in
     */
    isLoggedIn?: boolean;
    /**
     * Property bag to contain additional custom properties (Part C)
     */
    properties?: {
        /**
         * The number of milliseconds it took to load the page. Defaults to undefined. If set to default value, page load time is calculated internally.
         */
        duration?: number;
        [key: string]: any;
    };
    /**
     * iKey String - custom defined iKey.
     */
    iKey?: string;
}

export declare interface IPageViewTelemetryInternal extends IPageViewTelemetry {
    /**
     * An identifier assigned to each distinct impression for the purposes of correlating with pageview.
     * A new id is automatically generated on each pageview. You can manually specify this field if you
     * want to use a specific value instead.
     */
    id?: string;
    /**
     * Version of the part B schema, todo: set this value in trackpageView
     */
    ver?: string;
}

/**
 * PartC  telemetry interface
 */
declare interface IPartC {
    /**
     * Property bag to contain additional custom properties (Part C)
     */
    properties?: {
        [key: string]: any;
    };
    /**
     * Property bag to contain additional custom measurements (Part C)
     * @deprecated -- please use properties instead
     */
    measurements?: {
        [key: string]: number;
    };
}

export declare interface IPropertiesPlugin {
    readonly context: ITelemetryContext;
}

/**
 * An instance of Remote Dependency represents an interaction of the monitored component with a remote component/service like SQL or an HTTP endpoint.
 */
declare interface IRemoteDependencyData extends IDomain {
    /**
     * Schema version
     */
    ver: number;
    /**
     * Name of the command initiated with this dependency call. Low cardinality value. Examples are stored procedure name and URL path template.
     */
    name: string;
    /**
     * Identifier of a dependency call instance. Used for correlation with the request telemetry item corresponding to this dependency call.
     */
    id: string;
    /**
     * Result code of a dependency call. Examples are SQL error code and HTTP status code.
     */
    resultCode: string;
    /**
     * Request duration in format: DD.HH:MM:SS.MMMMMM. Must be less than 1000 days.
     */
    duration: string;
    /**
     * Indication of successful or unsuccessful call.
     */
    success: boolean;
    /**
     * Command initiated by this dependency call. Examples are SQL statement and HTTP URL's with all query parameters.
     */
    data: string;
    /**
     * Target site of a dependency call. Examples are server name, host address.
     */
    target: string;
    /**
     * Dependency type name. Very low cardinality value for logical grouping of dependencies and interpretation of other fields like commandName and resultCode. Examples are SQL, Azure table, and HTTP.
     */
    type: string;
    /**
     * Collection of custom properties.
     */
    properties: any;
    /**
     * Collection of custom measurements.
     */
    measurements: any;
}

export declare interface IRequestContext {
    status?: number;
    xhr?: XMLHttpRequest;
    request?: Request;
    response?: Response | string;
}

export declare interface IRequestHeaders {
    /**
     * Request-Context header
     */
    requestContextHeader: string;
    /**
     * Target instrumentation header that is added to the response and retrieved by the
     * calling application when processing incoming responses.
     */
    requestContextTargetKey: string;
    /**
     * Request-Context appId format
     */
    requestContextAppIdFormat: string;
    /**
     * Request-Id header
     */
    requestIdHeader: string;
    /**
     * W3C distributed tracing protocol header
     */
    traceParentHeader: string;
    /**
     * W3C distributed tracing protocol state header
     */
    traceStateHeader: string;
    /**
     * Sdk-Context header
     * If this header passed with appId in content then appId will be returned back by the backend.
     */
    sdkContextHeader: string;
    /**
     * String to pass in header for requesting appId back from the backend.
     */
    sdkContextHeaderAppIdRequest: string;
    requestContextHeaderLowerCase: string;
}

export declare interface ISample {
    /**
     * Sample rate
     */
    sampleRate: number;
    isSampledIn(envelope: ITelemetryItem): boolean;
}

export { isBeaconApiSupported }

export declare function isCrossOriginError(message: string | Event, url: string, lineNumber: number, columnNumber: number, error: Error | Event): boolean;

export declare interface ISerializable {
    /**
     * The set of fields for a serializable object.
     * This defines the serialization order and a value of true/false
     * for each field defines whether the field is required or not.
     */
    aiDataContract: any;
}

export declare interface ISession {
    /**
     * The session ID.
     */
    id?: string;
    /**
     * The date at which this guid was genereated.
     * Per the spec the ID will be regenerated if more than acquisitionSpan milliseconds ellapse from this time.
     */
    acquisitionDate?: number;
    /**
     * The date at which this session ID was last reported.
     * This value should be updated whenever telemetry is sent using this ID.
     * Per the spec the ID will be regenerated if more than renewalSpan milliseconds elapse from this time with no activity.
     */
    renewalDate?: number;
}

export declare function isInternalApplicationInsightsEndpoint(endpointUrl: string): boolean;

export { isSampledFlag }

declare interface IStackDetails {
    src: string;
    obj: string[];
}

/**
 * Stack frame information.
 */
declare interface IStackFrame {
    /**
     * Level in the call stack. For the long stacks SDK may not report every function in a call stack.
     */
    level: number;
    /**
     * Method name.
     */
    method: string;
    /**
     * Name of the assembly (dll, jar, etc.) containing this function.
     */
    assembly: string;
    /**
     * File name or URL of the method implementation.
     */
    fileName: string;
    /**
     * Line number of the code implementation.
     */
    line: number;
}

/**
 * Identifies a simple interface to allow you to override the storage mechanism used
 * to track unsent and unacknowledged events. When provided it must provide both
 * the get and set item functions.
 * @since 2.8.12
 */
export declare interface IStorageBuffer {
    /**
     * Retrieves the stored value for a given key
     */
    getItem(logger: IDiagnosticLogger, name: string): string;
    /**
     * Sets the stored value for a given key
     */
    setItem(logger: IDiagnosticLogger, name: string, data: string): boolean;
}

export { isValidSpanId }

export { isValidTraceId }

export { isValidTraceParent }

export declare interface ITelemetryContext {
    /**
     * The object describing a component tracked by this object.
     */
    readonly application: IApplication;
    /**
     * The object describing a device tracked by this object.
     */
    readonly device: IDevice;
    /**
     * The object describing internal settings.
     */
    readonly internal: IInternal;
    /**
     * The object describing a location tracked by this object.
     */
    readonly location: ILocation;
    /**
     * The object describing a operation tracked by this object.
     */
    readonly telemetryTrace: ITelemetryTrace;
    /**
     * The object describing a user tracked by this object.
     */
    readonly user: IUserContext;
    /**
     * The object describing a session tracked by this object.
     */
    readonly session: ISession;
    /**
     * The object describing os details tracked by this object.
     */
    readonly os?: IOperatingSystem;
    /**
     * The object describing we details tracked by this object.
     */
    readonly web?: IWeb;
    /**
     * application id obtained from breeze responses. Is used if appId is not specified by root config
     */
    appId: () => string;
    /**
     * session id obtained from session manager.
     */
    getSessionId: () => string;
}

export declare interface ITelemetryTrace {
    /**
     * Trace id
     */
    traceID?: string;
    /**
     * Parent id
     */
    parentID?: string;
    /**
     * @deprecated Never Used
     * Trace state
     */
    traceState?: ITraceState;
    /**
     * An integer representation of the W3C TraceContext trace-flags. https://www.w3.org/TR/trace-context/#trace-flags
     */
    traceFlags?: number;
    /**
     * Name
     */
    name?: string;
}

/**
 * Identifies frequency of items sent
 * Default: send data on 28th every 3 month each year
 */
export declare interface IThrottleInterval {
    /**
     * Identifies month interval that items can be sent
     * For example, if it is set to 2 and start date is in Jan, items will be sent out every two months (Jan, March, May etc.)
     * If both monthInterval and dayInterval are undefined, it will be set to 3
     */
    monthInterval?: number;
    /**
     * Identifies days Interval from start date that items can be sent
     * Default: undefined
     */
    dayInterval?: number;
    /**
     * Identifies days within each month that items can be sent
     * If both monthInterval and dayInterval are undefined, it will be default to [28]
     */
    daysOfMonth?: number[];
}

/**
 * Identifies limit number/percentage of items sent per time
 * If both are provided, minimum number between the two will be used
 */
export declare interface IThrottleLimit {
    /**
     * Identifies sampling percentage of items per time
     * The percentage is set to 4 decimal places, for example: 1 means 0.0001%
     * Default: 100 (0.01%)
     */
    samplingRate?: number;
    /**
     * Identifies limit number of items per time
     * Default: 1
     */
    maxSendNumber?: number;
}

/**
 * Identifies object for local storage
 */
export declare interface IThrottleLocalStorageObj {
    /**
     * Identifies start date
     */
    date: Date;
    /**
     * Identifies current count
     */
    count: number;
    /**
     * identifies previous triggered throttle date
     */
    preTriggerDate?: Date;
}

/**
 * Identifies basic config
 */
export declare interface IThrottleMgrConfig {
    /**
     * Identifies message key to be used for local storage key
     * Default: IThrottleMsgKey.default
     */
    msgKey?: IThrottleMsgKey;
    /**
     * Identifies if throttle is disabled
     * Default: false
     */
    disabled?: boolean;
    /**
     * Identifies limit number/percentage of items sent per time
     * Default: sampling percentage 0.01% with one item sent per time
     */
    limit?: IThrottleLimit;
    /**
     * Identifies frequency of items sent
     * Default: send data on 28th every 3 month each year
     */
    interval?: IThrottleInterval;
}

export declare const enum IThrottleMsgKey {
    /**
     * Default Message key for non pre-defined message
     */
    default = 0,
    /**
     * Message key for ikey Deprecation
     */
    ikeyDeprecate = 1,
    /**
     * Message key for cdn Deprecation
     */
    cdnDeprecate = 2
}

/**
 * Identifies throttle result
 */
export declare interface IThrottleResult {
    /**
     * Identifies if items are sent
     */
    isThrottled: boolean;
    /**
     * Identifies numbers of items are sent
     * if isThrottled is false, it will be set to 0
     */
    throttleNum: number;
}

export { ITraceParent }

export declare interface ITraceState {
}

export declare interface ITraceTelemetry extends IPartC {
    /**
     * @description A message string
     * @type {string}
     * @memberof ITraceTelemetry
     */
    message: string;
    /**
     * @description Severity level of the logging message used for filtering in the portal
     * @type {SeverityLevel}
     * @memberof ITraceTelemetry
     */
    severityLevel?: SeverityLevel;
    /**
     * @description custom defiend iKey
     * @type {SeverityLevel}
     * @memberof ITraceTelemetry
     */
    iKey?: string;
}

export declare interface IUser {
    /**
     * The telemetry configuration.
     */
    config: any;
    /**
     * The user ID.
     */
    id: string;
    /**
     * Authenticated user id
     */
    authenticatedId: string;
    /**
     * The account ID.
     */
    accountId: string;
    /**
     * The account acquisition date.
     */
    accountAcquisitionDate: string;
    /**
     * The localId
     */
    localId: string;
    /**
     * A flag indicating whether this represents a new user
     */
    isNewUser?: boolean;
    /**
     * A flag indicating whether the user cookie has been set
     */
    isUserCookieSet?: boolean;
}

export declare interface IUserContext extends IUser {
    setAuthenticatedUserContext(authenticatedUserId: string, accountId?: string, storeInCookie?: boolean): void;
    clearAuthenticatedUserContext(): void;
    update(userId?: string): void;
}

export declare interface IWeb {
    /**
     * Browser name, set at ingestion
     */
    browser: string;
    /**
     * Browser ver, set at ingestion.
     */
    browserVer: string;
    /**
     * Language
     */
    browserLang: string;
    /**
     * User consent, populated to properties bag
     */
    userConsent: boolean;
    /**
     * Whether event was fired manually, populated to properties bag
     */
    isManual: boolean;
    /**
     * Screen resolution, populated to properties bag
     */
    screenRes: string;
    /**
     * Current domain. Leverages Window.location.hostname. populated to properties bag
     */
    domain: string;
}

export declare class Metric implements IMetricData, ISerializable {
    static envelopeType: string;
    static dataType: string;
    aiDataContract: {
        ver: FieldType;
        metrics: FieldType;
        properties: FieldType;
    };
    /**
     * Schema version
     */
    ver: number;
    /**
     * List of metrics. Only one metric in the list is currently supported by Application Insights storage. If multiple data points were sent only the first one will be used.
     */
    metrics: DataPoint[];
    /**
     * Collection of custom properties.
     */
    properties: any;
    /**
     * Collection of custom measurements.
     */
    measurements: any;
    /**
     * Constructs a new instance of the MetricTelemetry object
     */
    constructor(logger: IDiagnosticLogger, name: string, value: number, count?: number, min?: number, max?: number, stdDev?: number, properties?: any, measurements?: {
        [key: string]: number;
    });
}

/**
 * Convert ms to c# time span format
 */
export declare function msToTimeSpan(totalms: number): string;

export declare class PageView implements IPageViewData, ISerializable {
    static envelopeType: string;
    static dataType: string;
    aiDataContract: {
        ver: FieldType;
        name: FieldType;
        url: FieldType;
        duration: FieldType;
        properties: FieldType;
        measurements: FieldType;
        id: FieldType;
    };
    /**
     * Schema version
     */
    ver: number;
    /**
     * Event name. Keep it low cardinality to allow proper grouping and useful metrics.
     */
    name: string;
    /**
     * Collection of custom properties.
     */
    properties: any;
    /**
     * Collection of custom measurements.
     */
    measurements: any;
    /**
     * Request URL with all query string parameters
     */
    url: string;
    /**
     * Request duration in format: DD.HH:MM:SS.MMMMMM. For a page view (PageViewData), this is the duration. For a page view with performance information (PageViewPerfData), this is the page load time. Must be less than 1000 days.
     */
    duration: string;
    /**
     * Identifier of a page view instance. Used for correlation between page view and other telemetry items.
     */
    id: string;
    /**
     * Constructs a new instance of the PageEventTelemetry object
     */
    constructor(logger: IDiagnosticLogger, name?: string, url?: string, durationMs?: number, properties?: {
        [key: string]: string;
    }, measurements?: {
        [key: string]: number;
    }, id?: string);
}

export declare class PageViewPerformance implements IPageViewPerfData, ISerializable {
    static envelopeType: string;
    static dataType: string;
    aiDataContract: {
        ver: FieldType;
        name: FieldType;
        url: FieldType;
        duration: FieldType;
        perfTotal: FieldType;
        networkConnect: FieldType;
        sentRequest: FieldType;
        receivedResponse: FieldType;
        domProcessing: FieldType;
        properties: FieldType;
        measurements: FieldType;
    };
    /**
     * Schema version
     */
    ver: number;
    /**
     * Event name. Keep it low cardinality to allow proper grouping and useful metrics.
     */
    name: string;
    /**
     * Collection of custom properties.
     */
    properties: any;
    /**
     * Collection of custom measurements.
     */
    measurements: any;
    /**
     * Request URL with all query string parameters
     */
    url: string;
    /**
     * Request duration in format: DD.HH:MM:SS.MMMMMM. For a page view (PageViewData), this is the duration. For a page view with performance information (PageViewPerfData), this is the page load time. Must be less than 1000 days.
     */
    duration: string;
    /**
     * Identifier of a page view instance. Used for correlation between page view and other telemetry items.
     */
    id: string;
    /**
     * Performance total in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff
     */
    perfTotal: string;
    /**
     * Network connection time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff
     */
    networkConnect: string;
    /**
     * Sent request time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff
     */
    sentRequest: string;
    /**
     * Received response time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff
     */
    receivedResponse: string;
    /**
     * DOM processing time in TimeSpan 'G' (general long) format: d:hh:mm:ss.fffffff
     */
    domProcessing: string;
    /**
     * Constructs a new instance of the PageEventTelemetry object
     */
    constructor(logger: IDiagnosticLogger, name: string, url: string, unused: number, properties?: {
        [key: string]: string;
    }, measurements?: {
        [key: string]: number;
    }, cs4BaseData?: IPageViewPerformanceTelemetry);
}

export declare function parseConnectionString(connectionString?: string): ConnectionString;

export { parseTraceParent }

export declare const ProcessLegacy = "ProcessLegacy";

export declare const PropertiesPluginIdentifier = "AppInsightsPropertiesPlugin";

export declare class RemoteDependencyData implements IRemoteDependencyData, ISerializable {
    static envelopeType: string;
    static dataType: string;
    aiDataContract: {
        id: FieldType;
        ver: FieldType;
        name: FieldType;
        resultCode: FieldType;
        duration: FieldType;
        success: FieldType;
        data: FieldType;
        target: FieldType;
        type: FieldType;
        properties: FieldType;
        measurements: FieldType;
        kind: FieldType;
        value: FieldType;
        count: FieldType;
        min: FieldType;
        max: FieldType;
        stdDev: FieldType;
        dependencyKind: FieldType;
        dependencySource: FieldType;
        commandName: FieldType;
        dependencyTypeName: FieldType;
    };
    /**
     * Schema version
     */
    ver: number;
    /**
     * Name of the command initiated with this dependency call. Low cardinality value. Examples are stored procedure name and URL path template.
     */
    name: string;
    /**
     * Identifier of a dependency call instance. Used for correlation with the request telemetry item corresponding to this dependency call.
     */
    id: string;
    /**
     * Result code of a dependency call. Examples are SQL error code and HTTP status code.
     */
    resultCode: string;
    /**
     * Request duration in format: DD.HH:MM:SS.MMMMMM. Must be less than 1000 days.
     */
    duration: string;
    /**
     * Indication of successful or unsuccessful call.
     */
    success: boolean;
    /**
     * Command initiated by this dependency call. Examples are SQL statement and HTTP URL's with all query parameters.
     */
    data: string;
    /**
     * Target site of a dependency call. Examples are server name, host address.
     */
    target: string;
    /**
     * Dependency type name. Very low cardinality value for logical grouping of dependencies and interpretation of other fields like commandName and resultCode. Examples are SQL, Azure table, and HTTP.
     */
    type: string;
    /**
     * Collection of custom properties.
     */
    properties: any;
    /**
     * Collection of custom measurements.
     */
    measurements: any;
    /**
     * Constructs a new instance of the RemoteDependencyData object
     */
    constructor(logger: IDiagnosticLogger, id: string, absoluteUrl: string, commandName: string, value: number, success: boolean, resultCode: number, method?: string, requestAPI?: string, correlationContext?: string, properties?: Object, measurements?: Object);
}

export declare const RequestHeaders: IRequestHeaders & {
    requestContextHeader: "Request-Context";
    requestContextTargetKey: "appId";
    requestContextAppIdFormat: "appId=cid-v1:";
    requestIdHeader: "Request-Id";
    traceParentHeader: "traceparent";
    traceStateHeader: "tracestate";
    sdkContextHeader: "Sdk-Context";
    sdkContextHeaderAppIdRequest: "appId";
    requestContextHeaderLowerCase: "request-context";
    0: "Request-Context";
    1: "appId";
    2: "appId=cid-v1:";
    3: "Request-Id";
    4: "traceparent";
    5: "tracestate";
    6: "Sdk-Context";
    7: "appId";
    8: "request-context";
};

export declare const SampleRate = "sampleRate";

/**
 * Defines the level of severity for the event.
 */
export declare const SeverityLevel: EnumValue<typeof eSeverityLevel>;

export declare type SeverityLevel = number | eSeverityLevel;

export declare function stringToBoolOrDefault(str: any, defaultValue?: boolean): boolean;

export declare const strNotSpecified = "not_specified";

export declare class TelemetryItemCreator {
    /**
     * Create a telemetry item that the 1DS channel understands
     * @param item - domain specific properties; part B
     * @param baseType - telemetry item type. ie PageViewData
     * @param envelopeName - name of the envelope. ie Microsoft.ApplicationInsights.<instrumentation key>.PageView
     * @param customProperties - user defined custom properties; part C
     * @param systemProperties - system properties that are added to the context; part A
     * @returns ITelemetryItem that is sent to channel
     */
    static create: typeof createTelemetryItem;
}

export declare class ThrottleMgr {
    canThrottle: () => boolean;
    sendMessage: (msgID: _eInternalMessageId, message: string, severity?: eLoggingSeverity) => IThrottleResult | null;
    getConfig: () => IThrottleMgrConfig;
    isTriggered: () => boolean;
    isReady: () => boolean;
    onReadyState: (isReady?: boolean) => boolean;
    flush: () => boolean;
    config: IThrottleMgrConfig;
    constructor(config?: IThrottleMgrConfig, core?: IAppInsightsCore, namePrefix?: string, unloadHookContainer?: IUnloadHookContainer);
}

export declare class Trace implements IMessageData, ISerializable {
    static envelopeType: string;
    static dataType: string;
    aiDataContract: {
        ver: FieldType;
        message: FieldType;
        severityLevel: FieldType;
        properties: FieldType;
    };
    /**
     * Schema version
     */
    ver: number;
    /**
     * Trace message
     */
    message: string;
    /**
     * Trace severity level.
     */
    severityLevel: SeverityLevel;
    /**
     * Collection of custom properties.
     */
    properties: any;
    /**
     * Collection of custom measurements.
     */
    measurements: any;
    /**
     * Constructs a new instance of the TraceTelemetry object
     */
    constructor(logger: IDiagnosticLogger, message: string, severityLevel?: SeverityLevel, properties?: any, measurements?: {
        [key: string]: number;
    });
}

export declare function urlGetAbsoluteUrl(url: string): string;

export declare function urlGetCompleteUrl(method: string, absoluteUrl: string): string;

export declare function urlGetPathName(url: string): string;

export declare function urlParseFullHost(url: string, inclPort?: boolean): string;

export declare function urlParseHost(url: string, inclPort?: boolean): string;

export declare function urlParseUrl(url: string): HTMLAnchorElement;

/**
 * Returns whether LocalStorage can be used, if the reset parameter is passed a true this will override
 * any previous disable calls.
 * @param reset - Should the usage be reset and determined only based on whether LocalStorage is available
 */
export declare function utlCanUseLocalStorage(reset?: boolean): boolean;

export declare function utlCanUseSessionStorage(reset?: boolean): boolean;

/**
 * Disables the global SDK usage of local or session storage if available
 */
export declare function utlDisableStorage(): void;

/**
 * Re-enables the global SDK usage of local or session storage if available
 */
export declare function utlEnableStorage(): void;

export declare function utlGetLocalStorage(logger: IDiagnosticLogger, name: string): string;

export declare function utlGetSessionStorage(logger: IDiagnosticLogger, name: string): string;

export declare function utlGetSessionStorageKeys(): string[];

export declare function utlRemoveSessionStorage(logger: IDiagnosticLogger, name: string): boolean;

export declare function utlRemoveStorage(logger: IDiagnosticLogger, name: string): boolean;

export declare function utlSetLocalStorage(logger: IDiagnosticLogger, name: string, data: string): boolean;

export declare function utlSetSessionStorage(logger: IDiagnosticLogger, name: string, data: string): boolean;

export { }