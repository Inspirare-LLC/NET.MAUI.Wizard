using NET.MAUI.Wizard.Abstractions;
using NET.MAUI.Wizard.EventHandlers;
using NET.MAUI.Wizard.ViewModels;

namespace NET.MAUI.Wizard.ContentViews;

public partial class WizardContentView : ContentView
{
    public event EventHandler<WizardFinishedEventArgs> OnFinished
    {
        add
        {
            if (_viewModel == null)
                return;

            _viewModel.OnFinished += value;
        }

        remove
        {
            if (_viewModel == null)
                return;

            _viewModel.OnFinished -= value;
        }
    }

    private WizardViewModel? _viewModel;

    private WizardContentView(bool allowSwipeGestures = true)
    {
        InitializeComponent();

        if (allowSwipeGestures)
        {
            var swipeLeftGestureRecognizer = new SwipeGestureRecognizer();
            swipeLeftGestureRecognizer.Direction = SwipeDirection.Left;
            swipeLeftGestureRecognizer.Threshold = 40;
            swipeLeftGestureRecognizer.Swiped += (args, obj) => _viewModel?.NextCommand?.Execute(null);

            var swipeRightGestureRecognizer = new SwipeGestureRecognizer();
            swipeRightGestureRecognizer.Direction = SwipeDirection.Right;
            swipeRightGestureRecognizer.Threshold = 40;
            swipeRightGestureRecognizer.Swiped += (args, obj) => _viewModel?.BackCommand?.Execute(null);

            Content.GestureRecognizers.Add(swipeLeftGestureRecognizer);
            Content.GestureRecognizers.Add(swipeRightGestureRecognizer);
        }
    }

    public WizardContentView(IEnumerable<WizardItemViewModel> items, bool isAnimationEnabled = true, string? nextLabelText = null,
                             string? backLabelText = null, string? finishLabelText = null, string? skipLabelText = null,
                             Color? progressBarColor = null, bool allowSwipeGestures = true, IServiceProvider? serviceProvider = null) : this(allowSwipeGestures)
    {
        var translateContentToBeforeFunc = new Func<bool, Task>((isNext) => StepContent.TranslateTo(isNext ? -1000 : 1000, StepContent.Y));
        var translateContentToAfterFunc = new Func<Task>(() => StepContent.TranslateTo(0, 0));

        _viewModel = new WizardViewModel(items, translateContentToBeforeFunc, translateContentToAfterFunc,
                                         isAnimationEnabled, nextLabelText, backLabelText, finishLabelText, 
                                         skipLabelText, progressBarColor, serviceProvider);

        BindingContext = _viewModel;
    }

    /// <summary>
    /// Call this to call OnAppearing on first item appearance
    /// </summary>
    /// <returns></returns>
    public Task OnAppearing()
    {
        if (_viewModel == null)
            return Task.CompletedTask;

        if (_viewModel.CurrentItem == null)
            return Task.CompletedTask;

        if (_viewModel.CurrentItem.View == null)
            return Task.CompletedTask;

        return (_viewModel.CurrentItem.View as IWizardContentView)?.OnAppearing();
    }
}