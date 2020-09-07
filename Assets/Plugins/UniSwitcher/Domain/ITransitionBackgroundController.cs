namespace UniSwitcher.Domain
{
    public interface ITransitionBackgroundController
    {
        void DetectMainCamera();
        void TriggerTransitionIn();
        void TriggerTransitionOut();
        void ForceTransitionWait();
        bool IsTransitionAllowed();
        TransitionState GetTransitionState();
    }

    public enum TransitionState
    {
        Ready,
        In,
        Wait,
        Out,
    }
}