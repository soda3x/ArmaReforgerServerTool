namespace ReforgerServerApp

{
    public partial class ServerParameter : UserControl
    {
        private string parameterProperName = string.Empty;
        protected Control underlyingControl;
        private ToolTip toolTip;

        public ServerParameter()
        {
            InitializeComponent();
            toolTip = new();
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

        public string ParameterTooltip
        {
            set { toolTip.SetToolTip(parameterName, value); toolTip.SetToolTip(underlyingControl, value); toolTip.SetToolTip(this, value); }
        }

        public void SetFieldEnabled(bool fieldEnabled)
        {
            underlyingControl.Enabled = fieldEnabled;
        }
    }
}
