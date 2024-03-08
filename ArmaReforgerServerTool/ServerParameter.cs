namespace ReforgerServerApp

{
    public partial class ServerParameter : UserControl
    {
        private string parameterProperName = string.Empty;
        protected Control underlyingControl;

        public ServerParameter()
        {
            InitializeComponent();
        }

        public virtual object ParameterValue
        {
            get { throw new Exception("This method must be overridden"); }
            set { throw new Exception("This method must be overridden"); }
        }

        public string ParameterName
        {
            get => parameterProperName;
            set => parameterProperName = value;
        }

        public string ParameterFriendlyName
        {
            get => parameterName.Text;
            set => parameterName.Text = value;
        }

        public Control UnderlyingType
        {
            get => underlyingControl;
        }

        public void SetFieldEnabled(bool fieldEnabled)
        {
            underlyingControl.Enabled = fieldEnabled;
        }
    }
}
