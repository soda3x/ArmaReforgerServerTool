namespace Longbow.Models
{
  internal class RconPlayer
  {
    public int PlayerNumber { get; set; }
    public string PlayerId { get; set; }
    public string PlayerName { get; set; }

    public RconPlayer(int playerNumber, string playerId, string playerName)
    {
      PlayerNumber = playerNumber;
      PlayerId = playerId;
      PlayerName = playerName;
    }

    public RconPlayer(string playersCmdOutput)
    {
      // Parse an Rcon players line into an RconPlayer instance
      // e.g. 1;78addc4f - 7b1c - 471e-b3e5 - 62567cf6c7d7;soda3x
      string[] playerSplit = playersCmdOutput.Split(';');

      if (playerSplit.Length < 3)
      {
        throw new ArgumentException($"Line does not contain enough data: '{playersCmdOutput}'");
      }

      if (!int.TryParse(playerSplit[0].Trim(), out int playerNum))
      {
        throw new ArgumentException($"Invalid player number: '{playerSplit[0]}'");
      }

      PlayerNumber = playerNum;
      PlayerId = playerSplit[1].Trim();
      PlayerName = playerSplit[2].Trim();
    }
  }
}
