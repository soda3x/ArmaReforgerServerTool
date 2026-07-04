namespace Longbow.Models
{
  internal class RconBan
  {
    public string PlayerUid { get; set; }
    public string PlayerName { get; set; }

    public RconBan(string banLine)
    {
      if (banLine.StartsWith("- "))
      {
        banLine = banLine.Substring(2);
      }

      // 2. Split on the pipe character
      string[] split = banLine.Split('|');
      if (split.Length < 2)
      {
        throw new ArgumentException($"Invalid ban line: '{banLine}'");
      }

      PlayerUid = split[0].Trim();
      PlayerName = split[1].Trim();
    }
  }
}
