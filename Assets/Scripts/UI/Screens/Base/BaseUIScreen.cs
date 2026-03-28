namespace UI.Screens.Base
{
    public class BaseUIScreen : BaseScreen
    {
       // [SerializeField] private PeopleBaseUIPage _peoplePanel;
       // [SerializeField] private BottomPanelBaseUIPage _bottomPanel;
       // [SerializeField] private DateTimeBaseUIPage _timePanel;
       // [SerializeField] private ResourcesBaseUIPage _resourcesPanel;

        public override void Show()
        {
            base.Show();

            //_bottomPanel.OnShow();
            //_peoplePanel.OnShow();
            //_timePanel.OnShow();
            //_resourcesPanel.OnShow();

            RegisterEvents(); 
        }

        private void RegisterEvents()
        { 

        }

        private void UnregisterEvents()
        { 

        }

        public override void Hide()
        {
            base.Hide();

           //_bottomPanel.OnHide();
           //_peoplePanel.OnHide();
           //_timePanel.OnHide();
           //_resourcesPanel.OnHide();

            UnregisterEvents();
        }
    }
}