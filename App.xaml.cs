namespace KavaPryct
{
    public partial class App : Application
    {
        public static string AccessCode="";
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "KAVA: Agenda Virtual" };
        }
    }
}
