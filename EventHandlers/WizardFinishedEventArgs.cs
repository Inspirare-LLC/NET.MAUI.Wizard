using NET.MAUI.Wizard.ViewModels;

namespace NET.MAUI.Wizard.EventHandlers
{
    public class WizardFinishedEventArgs : EventArgs
    {
        public WizardFinishedEventArgs(IEnumerable<WizardItemViewModel> wizardItemViewModels)
        {
            WizardItemViewModels = wizardItemViewModels;
        }

        public IEnumerable<WizardItemViewModel> WizardItemViewModels { get; private set; }
    }
}
