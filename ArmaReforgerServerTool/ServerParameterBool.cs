namespace ReforgerServerApp
{
    public partial class ServerParameterBool : ServerParameter
    {
        private ServerParameter associatedControl;

        public ServerParameterBool()
        {
            InitializeComponent();
            underlyingControl = parameterValue;
        }

        public override object ParameterValue
        {
            get => parameterValue.Checked;
            set => parameterValue.Checked = (bool)value;
        }

        public ServerParameter AssociatedControl
        {
            get => associatedControl;
            set => associatedControl = value;
        }

        public void OnCheckChanged(object sender, EventArgs e)
        {
            if (associatedControl != null)
            {
                associatedControl.SetFieldEnabled((bool)ParameterValue);
            }
        }
    }
}
