using System;

namespace UniSwitcher.Domain
{
    /// <summary>
    /// Interface for scene object
    /// </summary>
    /// <remarks>This object should hold a relative path (starting from Assets/) to the current Unity scene.</remarks>
    public interface IScene
    {
        string RawValue { get; }

        // UNITY_ANALYTICS: Defined by Unity Analytics module.
        // UGS_ANALYTICS: Defined conditionally in the Assembly Definition. Compiled only if com.unity.services.analytics >=4.0.0 is present.
#if UNITY_ANALYTICS || UGS_ANALYTICS
        /// <summary>
        /// If true, suppresses warning raised whenever you don't implement <see cref="IReportable"/>
        /// and/or (in case of Unity Gaming Service Analytics) override <see cref="ScreenVisitEventName"/> and <see cref="ScreenVisitEventParameterName"/>.
        /// </summary>
        bool SuppressEvent { get; }
#endif
        
#if UGS_ANALYTICS
        /// <summary>
        /// Event type name for the screen visit. You must define an event type with this name in your project's "Event Manager."
        /// If this or <see cref="ScreenVisitPropertyName"/> is null or the Scene definition doesn't implement <see cref="IReportable"/>,
        /// UniSwitcher will not send the event.
        /// </summary>
        string ScreenVisitEventName { get; }
        /// <summary>
        /// Name of the parameter that goes with the "Screen Visit" event. You must define an event property with this name in your project's "Event Manager"
        /// and associate it with the custom event type with the name specified above.
        /// If this or <see cref="ScreenVisitEventName"/> is null or the Scene definition doesn't implement <see cref="IReportable"/>,
        /// UniSwitcher will not send the event.
        /// </summary>
        string ScreenVisitEventParameterName { get; }
        
        [Obsolete("Please use ScreenVisitEventParameterName instead. This method was a typo.")]
        string ScreenVisitEventPropertyName { get; }
#endif
    }
}