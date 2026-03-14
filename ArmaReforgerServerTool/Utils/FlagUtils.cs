/******************************************************************************
 * File Name:    FlagUtils.cs
 * Project:      Longbow
 * Description:  Static class containing utility methods for 
 *               getting Country flags
 * 
 * Author:       Bradley Newman
 ******************************************************************************/
using FlagsISO;

namespace Longbow.Utils
{
  internal class FlagUtils
  {
    private static readonly Dictionary<string, string> PingSiteToCountryCode = new(StringComparer.OrdinalIgnoreCase)
{
    // North America
    { "new_york", "us" },
    { "washington", "us" },
    { "los_angeles", "us" },
    { "miami", "us" },
    { "chicago", "us" },
    { "dallas", "us" },
    { "seattle", "us" },
    { "atlanta", "us" },
    { "montreal", "ca" },
    { "toronto", "ca" },

    // Europe
    { "frankfurt", "de" },
    { "london", "gb" },
    { "paris", "fr" },
    { "amsterdam", "nl" },
    { "stockholm", "se" },
    { "warsaw", "pl" },
    { "madrid", "es" },

    // Oceania & Asia
    { "sydney", "au" },
    { "melbourne", "au" },
    { "singapore", "sg" },
    { "tokyo", "jp" },
    { "hong_kong", "hk" },
    { "seoul", "kr" },

    // South America & Africa
    { "sao_paulo", "br" },
    { "johannesburg", "za" }
};

    public static Bitmap ChooseFlag(string pingSite)
    {
      const bool shinyFlag = true;
      if (PingSiteToCountryCode.TryGetValue(pingSite, out string countryCode))
      {
        byte[] flagBytes = CountryFlagsISO.GetForCountry(countryCode, FlagSizes.Size_x_32, shinyFlag);
        if (flagBytes != null && flagBytes.Length > 0)
        {
          using (MemoryStream ms = new MemoryStream(flagBytes))
          {
            return new Bitmap(ms);
          }
        }
      }
      return null;
    }
  }
}
