namespace FcmsPortalUI.Services
{
    public class LayoutTrigger
    {
        /// <summary>
        /// Event fired when layout-related state has changed
        /// and the MainLayout should re-run its initialization logic.
        /// </summary>
        public event Action? OnChange;

        public void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }
    }
}
