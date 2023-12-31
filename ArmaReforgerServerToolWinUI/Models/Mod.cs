namespace ArmaReforgerServerToolWinUI.Models
{
    internal class Mod
    {
        public string ModID;
        public string ModName;

        public override string ToString()
        {
            return ModName;
        }

        public override bool Equals(object? obj)
        {
            if (obj == this)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() == typeof(Mod))
            {
                return ModName.Equals(((Mod)obj).ModName) && ModID.Equals(((Mod)obj).ModID);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ModID.GetHashCode();
        }
    }
}
