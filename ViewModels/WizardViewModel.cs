using NET.MAUI.Wizard.Abstractions;
using NET.MAUI.Wizard.EventHandlers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NET.MAUI.Wizard.ViewModels
{
    public class WizardViewModel : INotifyPropertyChanged
    {
        public event EventHandler<WizardFinishedEventArgs>? OnFinished;

        private string? _title;
        public string? Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        private ObservableCollection<WizardItemViewModel>? _items;
        public ObservableCollection<WizardItemViewModel>? Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged(nameof(Items)); }
        }

        private WizardItemViewModel? _currentItem;
        public WizardItemViewModel? CurrentItem
        {
            get { return _currentItem; }
            set { _currentItem = value; OnPropertyChanged(nameof(CurrentItem)); }
        }

        private bool _isLastItem;
        public bool IsLastItem
        {
            get { return _isLastItem; }
            private set { _isLastItem = value; OnPropertyChanged(nameof(IsLastItem)); }
        }

        private bool _isNotFirstItem;
        public bool IsNotFirstItem
        {
            get { return _isNotFirstItem; }
            private set { _isNotFirstItem = value; OnPropertyChanged(nameof(IsNotFirstItem)); }
        }

        private bool _isSkippable;
        public bool IsSkippable
        {
            get { return _isSkippable; }
            set { _isSkippable = value; OnPropertyChanged(nameof(IsSkippable)); }
        }

        private string? _backButtonLabel;
        public string? BackButtonLabel
        {
            get { return _backButtonLabel; }
            set { _backButtonLabel = value; OnPropertyChanged(nameof(BackButtonLabel)); }
        }

        private string? _nextButtonLabel;
        public string? NextButtonLabel
        {
            get { return _nextButtonLabel; }
            set { _nextButtonLabel = value; OnPropertyChanged(nameof(NextButtonLabel)); }
        }

        private string? _skipButtonLabel;
        public string? SkipButtonLabel
        {
            get { return _skipButtonLabel; }
            set { _skipButtonLabel = value; OnPropertyChanged(nameof(SkipButtonLabel)); }
        }

        private double _progressBarProgress;
        public double ProgressBarProgress
        {
            get { return _progressBarProgress; }
            set { _progressBarProgress = value; OnPropertyChanged(nameof(ProgressBarProgress)); }
        }

        private Color? _progressBarColor;
        public Color? ProgressBarColor
        {
            get { return _progressBarColor; }
            set { _progressBarColor = value; OnPropertyChanged(nameof(ProgressBarColor)); }
        }

        private Func<Task>? _customButtonAction;
        public Func<Task>? CustomButtonAction
        {
            get { return _customButtonAction; }
            set { _customButtonAction = value; OnPropertyChanged(nameof(CustomButtonAction)); }
        }

        private bool _isCustomButtonVisible;
        public bool IsCustomButtonVisible
        {
            get { return _isCustomButtonVisible; }
            set { _isCustomButtonVisible = value; OnPropertyChanged(nameof(IsCustomButtonVisible)); }
        }

        private string? _customButtonLabel;
        public string? CustomButtonLabel
        {
            get { return _customButtonLabel; }
            set { _customButtonLabel = value; OnPropertyChanged(nameof(CustomButtonLabel)); }
        }

        private ICommand? _backCommand;
        public ICommand? BackCommand
        {
            get { return _backCommand; }
            set { _backCommand = value; OnPropertyChanged(nameof(BackCommand)); }
        }

        private ICommand? _customCommand;
        public ICommand? CustomCommand
        {
            get { return _customCommand; }
            set { _customCommand = value; OnPropertyChanged(nameof(CustomCommand)); }
        }

        private ICommand? _skipCommand;
        public ICommand? SkipCommand
        {
            get { return _skipCommand; }
            set { _skipCommand = value; OnPropertyChanged(nameof(SkipCommand)); }
        }

        private ICommand? _nextCommand;
        public ICommand? NextCommand
        {
            get { return _nextCommand; }
            set { _nextCommand = value; OnPropertyChanged(nameof(NextCommand)); }
        }

        private int _currentItemIndex;

        private string? _nextButtonLabelText;
        private string? _backButtonLabelText;
        private string? _finishButtonLabelText;
        private string? _skipButtonLabelText;
        private bool _isAnimationEnabled;

        private Func<bool, Task> _translateContentToBeforeFunc;
        private Func<Task> _translateContentToAfterFunc;

        private readonly IServiceProvider? _serviceProvider;

        public WizardViewModel(IEnumerable<WizardItemViewModel> items, Func<bool, Task> translateContentToBeforeFunc, 
                               Func<Task> translateContentToAfterFunc, bool isAnimationEnabled = true, string? nextLabelText = null,
                               string? backLabelText = null, string? finishLabelText = null, string? skipLabelText = null,
                               Color? progressBarColor = null, IServiceProvider? serviceProvider = null)
        {
            _serviceProvider = serviceProvider;

            _translateContentToBeforeFunc = translateContentToBeforeFunc;
            _translateContentToAfterFunc = translateContentToAfterFunc;

            _isAnimationEnabled = isAnimationEnabled;

            if (!String.IsNullOrEmpty(nextLabelText))
                _nextButtonLabelText = NextButtonLabel = nextLabelText;

            if (!String.IsNullOrEmpty(backLabelText))
                _backButtonLabelText = BackButtonLabel = backLabelText;

            if (!String.IsNullOrEmpty(finishLabelText))
                _finishButtonLabelText = finishLabelText;

            if (!String.IsNullOrEmpty(skipLabelText))
                _skipButtonLabelText = SkipButtonLabel = skipLabelText;

            //Default color - green
            ProgressBarColor = progressBarColor ?? Colors.Green;

            if (!items.Any())
                throw new ArgumentException("Provide items.", nameof(items));

            Items = new ObservableCollection<WizardItemViewModel>(items.ToList());

            var item = Items[0];

            InitializeWizardItem(item);

            //Initialize commands
            BackCommand = new Command(async () => await BackAsync());
            NextCommand = new Command(async () => await NextAsync());
            SkipCommand = new Command(async () => await SkipAsync());
            CustomCommand = new Command(async () => await CustomActionAsync());
        }

        private async Task BackAsync()
        {
            if (Items == null)
                return;

            var currentItem = Items[GetCurrentItemIndex()].ViewModel;

            var result = await DecreaseCurrentItemIndex();
            if (result)
                await UpdateCurrentItem(false, currentItem);
        }

        private async Task NextAsync()
        {
            if (Items == null)
                return;

            var currentItem = Items[GetCurrentItemIndex()].ViewModel;

            var result = await IncreaseCurrentItemIndex();
            if (result)
                await UpdateCurrentItem(true, currentItem);
        }

        private async Task SkipAsync()
        {
            if (Items == null)
                return;

            var currentItem = Items[GetCurrentItemIndex()].ViewModel;

            var result = await IncreaseCurrentItemIndex(true);
            if (result)
                await UpdateCurrentItem(true, currentItem);
        }

        private async Task CustomActionAsync()
        {
            if (CustomButtonAction == null)
                return;

            await CustomButtonAction.Invoke();
        }

        private async Task UpdateCurrentItem(bool isNext, IWizardViewModel? previousViewModel = null)
        {
            if (Items == null)
                return;

            var item = Items[GetCurrentItemIndex()];

            if (item == null)
                return;

            await InitializeWizardItem(item, true, isNext);

            await CurrentItem.View.OnAppearingAsync();
        }

        public async Task<bool> IncreaseCurrentItemIndex(bool skip = false)
        {
            var newIndex = _currentItemIndex + 1;

            var item = Items[_currentItemIndex].View;
            var itemViewModel = Items[_currentItemIndex].ViewModel;

            //if skip, don't call on next
            if (!skip)
            {
                var result = await item.OnNextAsync(itemViewModel);
                if (!result)
                    return false;
            }

            //if finished, short circuit and exit
            if (newIndex == Items.Count)
            {
                OnFinished?.Invoke(null, new WizardFinishedEventArgs(Items));
                return false;
            }

            if (newIndex > Items.Count() - 1)
                return false;

            if (newIndex > 0)
                IsNotFirstItem = true;

            if (newIndex == Items.Count() - 1)
            {
                IsLastItem = true;
                NextButtonLabel = _finishButtonLabelText;
            }

            await item.OnDisappearingAsync();

            _currentItemIndex = newIndex;
            ProgressBarProgress = Math.Truncate(10 * (double)(_currentItemIndex + 1) / (Items.Count == 0 ? 1 : Items.Count)) / 10;

            return true;
        }

        public async Task<bool> DecreaseCurrentItemIndex()
        {
            var newIndex = _currentItemIndex - 1;
            if (newIndex < 0)
                return false;

            var item = Items[_currentItemIndex].View;
            var itemViewModel = Items[_currentItemIndex].ViewModel;

            var result = await item.OnPreviousAsync(itemViewModel);
            if (!result)
                return false;

            if (newIndex == 0)
                IsNotFirstItem = false;

            if (newIndex != Items.Count() - 1)
            {
                IsLastItem = false;
                NextButtonLabel = _nextButtonLabelText;
            }

            await item.OnDisappearingAsync();

            _currentItemIndex = newIndex;
            ProgressBarProgress = Math.Truncate(10 * (double)(_currentItemIndex + 1) / (Items.Count == 0 ? 1 : Items.Count)) / 10;

            return true;
        }

        public int GetCurrentItemIndex()
        {
            return _currentItemIndex;
        }

        private async Task InitializeWizardItem(WizardItemViewModel item, bool performTranslation = false, bool isNext = false)
        {
            var args = new List<object>(1 + item.AdditionalConstructorParameters.Count());
            args.Add(item.ViewModel);

            if (item.AdditionalConstructorParameters.Any())
                args.AddRange(item.AdditionalConstructorParameters);

            if (!item.Type.IsSubclassOf(typeof(View)) && item.Type != typeof(View))
                throw new ArgumentException(item.Type + " has to be derived from View");

            IWizardContentView? view = null;

            //If service provider is present, try to get view via DI
            if (_serviceProvider != null)
                view = _serviceProvider.GetService(item.Type) as IWizardContentView;

            //If view is still null, create it manually
            if (view == null)
                view = Activator.CreateInstance(item.Type, args.ToArray()) as IWizardContentView;

            if (view is not IWizardContentView)
                throw new ArgumentException(item.Type + " must implement IWizardContentView interface");

            item.View = view;

            if (performTranslation &&
                _isAnimationEnabled &&
                _translateContentToBeforeFunc != null)
                await _translateContentToBeforeFunc(isNext);

            CurrentItem = item;
            Title = item.Title;
            IsSkippable = item.IsSkippable;
            CustomButtonAction = item.CustomButtonAction;
            CustomButtonLabel = item.CustomButtonLabel;
            IsCustomButtonVisible = item.CustomButtonAction != null;

            if (performTranslation && 
                _isAnimationEnabled &&
                _translateContentToAfterFunc != null)
                await _translateContentToAfterFunc();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
