namespace ReforgerServerApp
{
    public partial class ServerParameterNumeric : ServerParameter
    {

        public ServerParameterNumeric()
        {
            InitializeComponent();
            underlyingControl = parameterValue;
        }

        public override object ParameterValue
        {
            get => parameterValue.Value;
            set => parameterValue.Value = (Int32) value;
        }

        public decimal ParameterMin
        {
            get => parameterValue.Minimum;
            set => parameterValue.Minimum = value;
        }

        public decimal ParameterMax
        {
            get => parameterValue.Maximum;
            set => parameterValue.Maximum = value;
        }

        public decimal ParameterIncrement
        {
            get => parameterValue.Increment;
            set => parameterValue.Increment = value;
        }
    }
}
