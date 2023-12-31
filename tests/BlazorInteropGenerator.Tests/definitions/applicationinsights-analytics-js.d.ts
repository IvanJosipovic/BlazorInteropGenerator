﻿/*
 * Microsoft Application Insights JavaScript SDK - Web Analytics, 3.0.2
 * Copyright (c) Microsoft and contributors. All rights reserved.
 *
 * Microsoft Application Insights Team
 * https://github.com/microsoft/ApplicationInsights-JS#readme
 *
 * ---------------------------------------------------------------------------
 * This is a single combined (rollup) declaration file for the package,
 * if you require a namespace wrapped version it is also available.
 * - Namespaced version: types/applicationinsights-analytics-js.namespaced.d.ts
 * ---------------------------------------------------------------------------
 */

import { BaseTelemetryPlugin } from '@microsoft/applicationinsights-core-js';
import { IAppInsights } from '@microsoft/applicationinsights-common';
import { IAppInsightsCore } from '@microsoft/applicationinsights-core-js';
import { IAutoExceptionTelemetry } from '@microsoft/applicationinsights-common';
import { IConfig } from '@microsoft/applicationinsights-common';
import { IConfiguration } from '@microsoft/applicationinsights-core-js';
import { ICookieMgr } from '@microsoft/applicationinsights-core-js';
import { ICustomProperties } from '@microsoft/applicationinsights-core-js';
import { IEventTelemetry } from '@microsoft/applicationinsights-common';
import { IExceptionTelemetry } from '@microsoft/applicationinsights-common';
import { IMetricTelemetry } from '@microsoft/applicationinsights-common';
import { IPageViewPerformanceTelemetry } from '@microsoft/applicationinsights-common';
import { IPageViewPerformanceTelemetryInternal } from '@microsoft/applicationinsights-common';
import { IPageViewTelemetry } from '@microsoft/applicationinsights-common';
import { IPageViewTelemetryInternal } from '@microsoft/applicationinsights-common';
import { IPlugin } from '@microsoft/applicationinsights-core-js';
import { IProcessTelemetryContext } from '@microsoft/applicationinsights-core-js';
import { ITelemetryInitializerHandler } from '@microsoft/applicationinsights-core-js';
import { ITelemetryItem } from '@microsoft/applicationinsights-core-js';
import { ITelemetryPluginChain } from '@microsoft/applicationinsights-core-js';
import { ITraceTelemetry } from '@microsoft/applicationinsights-common';

declare class AnalyticsPlugin extends BaseTelemetryPlugin implements IAppInsights, IAppInsightsInternal {
    static Version: string;
    identifier: string;
    priority: number;
    readonly config: IConfig;
    queue: Array<() => void>;
    autoRoutePVDelay: number;
    constructor();
    /**
     * Get the current cookie manager for this instance
     */
    getCookieMgr(): ICookieMgr;
    processTelemetry(env: ITelemetryItem, itemCtx?: IProcessTelemetryContext): void;
    trackEvent(event: IEventTelemetry, customProperties?: ICustomProperties): void;
    /**
     * Start timing an extended event. Call `stopTrackEvent` to log the event when it ends.
     * @param   name    A string that identifies this event uniquely within the document.
     */
    startTrackEvent(name: string): void;
    /**
     * Log an extended event that you started timing with `startTrackEvent`.
     * @param   name    The string you used to identify this event in `startTrackEvent`.
     * @param   properties  map[string, string] - additional data used to filter events and metrics in the portal. Defaults to empty.
     * @param   measurements    map[string, number] - metrics associated with this event, displayed in Metrics Explorer on the portal. Defaults to empty.
     */
    stopTrackEvent(name: string, properties?: {
        [key: string]: string;
    }, measurements?: {
        [key: string]: number;
    }): void;
    /**
     * @description Log a diagnostic message
     * @param trace
     * @param ICustomProperties.
     * @memberof ApplicationInsights
     */
    trackTrace(trace: ITraceTelemetry, customProperties?: ICustomProperties): void;
    /**
     * @description Log a numeric value that is not associated with a specific event. Typically
     * used to send regular reports of performance indicators. To send single measurement, just
     * use the name and average fields of {@link IMetricTelemetry}. If you take measurements
     * frequently, you can reduce the telemetry bandwidth by aggregating multiple measurements
     * and sending the resulting average at intervals
     * @param metric - input object argument. Only name and average are mandatory.
     * @param } customProperties additional data used to filter metrics in the
     * portal. Defaults to empty.
     * @memberof ApplicationInsights
     */
    trackMetric(metric: IMetricTelemetry, customProperties?: ICustomProperties): void;
    /**
     * Logs that a page or other item was viewed.
     * @param IPageViewTelemetry - The string you used as the name in startTrackPage. Defaults to the document title.
     * @param customProperties - Additional data used to filter events and metrics. Defaults to empty.
     * If a user wants to provide duration for pageLoad, it'll have to be in pageView.properties.duration
     */
    trackPageView(pageView?: IPageViewTelemetry, customProperties?: ICustomProperties): void;
    /**
     * Create a page view telemetry item and send it to the SDK pipeline through the core.track API
     * @param pageView - Page view item to be sent
     * @param properties - Custom properties (Part C) that a user can add to the telemetry item
     * @param systemProperties - System level properties (Part A) that a user can add to the telemetry item
     */
    sendPageViewInternal(pageView: IPageViewTelemetryInternal, properties?: {
        [key: string]: any;
    }, systemProperties?: {
        [key: string]: any;
    }): void;
    /**
     * @ignore INTERNAL ONLY
     * @param pageViewPerformance
     * @param properties
     */
    sendPageViewPerformanceInternal(pageViewPerformance: IPageViewPerformanceTelemetryInternal, properties?: {
        [key: string]: any;
    }, systemProperties?: {
        [key: string]: any;
    }): void;
    /**
     * Send browser performance metrics.
     * @param pageViewPerformance
     * @param customProperties
     */
    trackPageViewPerformance(pageViewPerformance: IPageViewPerformanceTelemetry, customProperties?: ICustomProperties): void;
    /**
     * Starts the timer for tracking a page load time. Use this instead of `trackPageView` if you want to control when the page view timer starts and stops,
     * but don't want to calculate the duration yourself. This method doesn't send any telemetry. Call `stopTrackPage` to log the end of the page view
     * and send the event.
     * @param name - A string that idenfities this item, unique within this HTML document. Defaults to the document title.
     */
    startTrackPage(name?: string): void;
    /**
     * Stops the timer that was started by calling `startTrackPage` and sends the pageview load time telemetry with the specified properties and measurements.
     * The duration of the page view will be the time between calling `startTrackPage` and `stopTrackPage`.
     * @param   name  The string you used as the name in startTrackPage. Defaults to the document title.
     * @param   url   String - a relative or absolute URL that identifies the page or other item. Defaults to the window location.
     * @param   properties  map[string, string] - additional data used to filter pages and metrics in the portal. Defaults to empty.
     * @param   measurements    map[string, number] - metrics associated with this page, displayed in Metrics Explorer on the portal. Defaults to empty.
     */
    stopTrackPage(name?: string, url?: string, properties?: {
        [key: string]: string;
    }, measurement?: {
        [key: string]: number;
    }): void;
    /**
     * @ignore INTERNAL ONLY
     * @param exception
     * @param properties
     * @param systemProperties
     */
    sendExceptionInternal(exception: IExceptionTelemetry, customProperties?: {
        [key: string]: any;
    }, systemProperties?: {
        [key: string]: any;
    }): void;
    /**
     * Log an exception you have caught.
     *
     * @param exception -   Object which contains exception to be sent
     * @param } customProperties   Additional data used to filter pages and metrics in the portal. Defaults to empty.
     *
     * Any property of type double will be considered a measurement, and will be treated by Application Insights as a metric.
     * @memberof ApplicationInsights
     */
    trackException(exception: IExceptionTelemetry, customProperties?: ICustomProperties): void;
    /**
     * @description Custom error handler for Application Insights Analytics
     * @param exception
     * @memberof ApplicationInsights
     */
    _onerror(exception: IAutoExceptionTelemetry): void;
    addTelemetryInitializer(telemetryInitializer: (item: ITelemetryItem) => boolean | void): ITelemetryInitializerHandler;
    initialize(config: IConfiguration & IConfig, core: IAppInsightsCore, extensions: IPlugin[], pluginChain?: ITelemetryPluginChain): void;
}
export { AnalyticsPlugin }
export { AnalyticsPlugin as ApplicationInsights }

/**
 * Internal interface to pass appInsights object to subcomponents without coupling
 */
export declare interface IAppInsightsInternal {
    sendPageViewInternal(pageViewItem: IPageViewTelemetryInternal, properties?: Object, systemProperties?: Object): void;
    sendPageViewPerformanceInternal(pageViewPerformance: IPageViewPerformanceTelemetryInternal, properties?: Object, systemProperties?: Object): void;
}

export { }