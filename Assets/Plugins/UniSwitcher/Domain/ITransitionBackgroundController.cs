namespace UniSwitcher.Domain
{
    /// <summary>
    /// Interface for transition animation object. This is supposed to be a canvas.
    /// </summary>
    public interface ITransitionBackgroundController
    {
        /// <summary>
        /// Since this controller is instantiated in a canvas, it has no idea what the main camera is.
        /// Implement this method to find the main camera in scene.
        /// </summary>
        void DetectMainCamera();

        /// <summary>
        /// Triggers the transition In (normal state to transitioning state) animation.
        /// </summary>
        void TriggerTransitionIn();

        /// <summary>
        /// Triggers the transition Out (transitioning state to next scene state) animation.
        /// </summary>
        void TriggerTransitionOut();

        /// <summary>
        /// Forces the transition to be 'Wait' status.
        /// </summary>
        void ForceTransitionWait();

        /// <summary>
        /// If it is safe to do another transition, return true. Otherwise (e.g. the previous transition is still going,) false.
        /// </summary>
        /// <returns></returns>
        bool IsTransitionAllowed();

        /// <summary>
        /// Return the current transition state. This might need polling into Animator.
        /// </summary>
        /// <returns></returns>
        TransitionState GetTransitionState();
    }

    /// <summary>
    /// Enum for transition state
    /// </summary>
    public enum TransitionState
    {
        /// <summary>
        /// Transition has not started. This is the default state.
        /// </summary>
        Ready,

        /// <summary>
        /// Transitioning to the scene change.
        /// </summary>
        In,

        /// <summary>
        /// After transition in, waiting for the next scene to load.
        /// </summary>
        Wait,

        /// <summary>
        /// Transitioning to the next scene.
        /// </summary>
        Out,
    }
}