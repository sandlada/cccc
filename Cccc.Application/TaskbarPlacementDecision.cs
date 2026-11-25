namespace Cccc.Application
{
    public enum TaskbarPlacementDecision
    {
        WaitingFirstShow,
        AboveNotOnMonitorOrHidden,
        AboveInsufficientSpace,
        InsideTrialAnchor,
        InsideAvoidingControls,
        InsideCachedRefreshing
    }
}
