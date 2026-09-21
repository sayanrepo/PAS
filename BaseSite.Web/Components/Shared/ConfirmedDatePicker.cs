using MudBlazor;

namespace BaseSite.Web.Components.Shared;

public sealed class ConfirmedDatePicker : MudDatePicker
{
    private bool confirming;
    private bool clearPending;

    public async Task SelectTodayAsync()
    {
        if (GetDisabledState() || GetReadOnlyState()) return;
        var today = DateTime.Today;
        await GoToDate(today, submitDate: false);
        CurrentView = OpenTo.Date;
        await OnDayClickedAsync(today);
        StateHasChanged();
    }

    public Task SelectEmptyAsync()
    {
        if (GetDisabledState() || GetReadOnlyState()) return Task.CompletedTask;
        clearPending = true;
        StateHasChanged();
        return Task.CompletedTask;
    }

    public async Task ConfirmAsync()
    {
        if (GetDisabledState() || GetReadOnlyState()) return;
        confirming = true;
        try
        {
            if (clearPending) await base.ClearAsync(close: false);
            await CloseAsync(submit: !clearPending);
        }
        finally
        {
            confirming = false;
        }
    }

    protected override Task OnOpenedAsync()
    {
        clearPending = false;
        return base.OnOpenedAsync();
    }

    protected override Task OnDayClickedAsync(DateTime dateTime)
    {
        clearPending = false;
        return base.OnDayClickedAsync(dateTime);
    }

    protected override string GetTitleDateString() => clearPending ? "بدون تاریخ" : base.GetTitleDateString();

    // MudBlazor also submits on some keyboard shortcuts; only confirmation may commit.
    protected override Task SubmitAsync() => confirming ? base.SubmitAsync() : Task.CompletedTask;

    protected override Task OnClosedAsync()
    {
        clearPending = false;
        return base.OnClosedAsync();
    }
}
