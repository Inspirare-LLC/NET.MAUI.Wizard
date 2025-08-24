using NET.MAUI.Wizard.Abstractions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NET.MAUI.Wizard.ViewModels
{
    public class WizardItemViewModel : INotifyPropertyChanged
    {
        private string? _title;
        public string? Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        private Type? _type;
        public Type? Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(nameof(Type)); }
        }

        private IWizardViewModel? _viewModel;
        public IWizardViewModel? ViewModel
        {
            get { return _viewModel; }
            set { _viewModel = value; OnPropertyChanged(nameof(ViewModel)); }
        }

        private IEnumerable<object>? _additionalConstructorParameters;
        public IEnumerable<object>? AdditionalConstructorParameters
        {
            get { return _additionalConstructorParameters; }
            set { _additionalConstructorParameters = value; OnPropertyChanged(nameof(AdditionalConstructorParameters)); }
        }

        private IWizardContentView? _view;
        public IWizardContentView? View
        {
            get { return _view; }
            set { _view = value; OnPropertyChanged(nameof(View)); }
        }

        private bool _isSkippable;
        public bool IsSkippable
        {
            get { return _isSkippable; }
            set { _isSkippable = value; OnPropertyChanged(nameof(IsSkippable)); }
        }

        private Func<Task>? _customButtonAction;
        public Func<Task>? CustomButtonAction
        {
            get { return _customButtonAction; }
            set { _customButtonAction = value; OnPropertyChanged(nameof(CustomButtonAction)); }
        }

        private string? _customButtonLabel;
        public string? CustomButtonLabel
        {
            get { return _customButtonLabel; }
            set { _customButtonLabel = value; OnPropertyChanged(nameof(CustomButtonLabel)); }
        }

        private WizardItemViewModel()
        {
            AdditionalConstructorParameters = new List<object>();
        }

        public WizardItemViewModel(string title, Type type, IWizardViewModel viewModel, bool isSkippable = false, Func<Task>? customButtonAction = null, 
                                   string? customButtonLabel = null, params object[] additionalParameters) : this()
        {
            Title = title;
            Type = type;
            ViewModel = viewModel;
            AdditionalConstructorParameters = additionalParameters ?? Array.Empty<object>();
            IsSkippable = isSkippable;
            CustomButtonAction = customButtonAction;
            CustomButtonLabel = customButtonLabel;
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
