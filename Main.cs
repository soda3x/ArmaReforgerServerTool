namespace ReforgerServerApp
{
    public partial class Main : Form
    {
        readonly string MOD_DATABASE_FILE = "./mod_database.txt";
        public Main()
        {
            InitializeComponent();
            if (File.Exists(MOD_DATABASE_FILE))
            {
                ReadModsDatabase();
            }
        }

        public ListBox GetEnabledModsList()
        {
            return enabledMods;
        }

        public ListBox GetAvailableModsList()
        {
            return availableMods;
        }

        private void AddModBtnPressed(object sender, EventArgs e)
        {
            AddModDialog addModDialog = new(this);
            addModDialog.ShowDialog();
        }

        private void RemoveSelectedModBtnPressed(object sender, EventArgs e)
        {
            GetAvailableModsList().Items.Remove(GetAvailableModsList().SelectedItem);
        }

        private void AddToEnabledModsBtnPressed(object sender, EventArgs e)
        {
            if ((Mod)GetAvailableModsList().SelectedItem != null)
            {
                Mod m = (Mod)GetAvailableModsList().SelectedItem;
                GetAvailableModsList().Items.Remove(m);
                GetEnabledModsList().Items.Add(m);
            }
        }

        private void RemovedFromEnabledModsBtnPressed(object sender, EventArgs e)
        {
            if ((Mod)GetEnabledModsList().SelectedItem != null)
            {
                Mod m = (Mod)GetEnabledModsList().SelectedItem;
                GetEnabledModsList().Items.Remove(m);
                GetAvailableModsList().Items.Add(m);
            }
        }

        private void SaveSettingsToFileBtnPressed(object sender, EventArgs e)
        {

        }

        private void LoadSettingsFromFileBtnPressed(object sender, EventArgs e)
        {

        }

        public void WriteModsDatabase()
        {
            using StreamWriter sw = File.AppendText(MOD_DATABASE_FILE);

            foreach (Mod mod in availableMods.Items)
            {
                sw.WriteLine(mod.GetModID() + "," + mod.GetModName() + "," + mod.GetModVersion());
            }
        }

        public void ReadModsDatabase()
        {
            using StreamReader sr = File.OpenText(MOD_DATABASE_FILE);

            string line = sr.ReadLine();

            if (line != null)
            {
                string[] split = line.Split(",");
                GetAvailableModsList().Items.Add(new Mod(split[0], split[1], split[2]));
            }
        }
    }
}