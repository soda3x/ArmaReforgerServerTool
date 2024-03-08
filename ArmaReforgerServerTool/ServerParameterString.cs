namespace ReforgerServerApp
{
    public partial class ServerParameterString : ServerParameter
    {
        public ServerParameterString()
        {
            InitializeComponent();
            underlyingControl = parameterValue;
        }

        public override object ParameterValue
        {
            get => parameterValue.Text;
            set => parameterValue.Text = (string) value;
        }
    }
}
