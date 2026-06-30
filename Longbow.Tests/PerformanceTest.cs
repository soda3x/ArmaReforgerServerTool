/******************************************************************************
 * File Name:    PerformanceTest.cs
 * Project:      Longbow.Tests
 * Description:  Performance test for ModValidationService to verify it
 *               completes validation in under 100ms for 50 mods.
 *
 * Author:       Bradley Newman
 ******************************************************************************/

using System.Diagnostics;
using Xunit;
using ReforgerServerApp;
using ReforgerServerApp.Managers;

namespace Longbow.Tests
{
  public class PerformanceTest
  {
    /// <summary>
    /// Performance Test: Verify validation completes in under 100ms for 50 mods
    /// </summary>
    [Fact]
    public void TestValidationPerformance_50Mods_UnderHundredMs()
    {
      var service = ModValidationService.GetInstance();

      // Create 50 mods (mix of dependent and independent)
      var mods = new List<Mod>();

      // Add base mods (no dependencies)
      for (int i = 0; i < 10; i++)
      {
        mods.Add(new Mod($"base_mod_{i}", $"Base Mod {i}", "1.0.0", false)
        {
          ModVersion = "1.0.0"
        });
      }

      // Add dependent mods
      for (int i = 0; i < 40; i++)
      {
        mods.Add(new Mod($"dep_mod_{i}", $"Dependent Mod {i}", "1.0.0", false)
        {
          ModVersion = "1.0.0"
        });
      }

      // Measure validation time
      var sw = Stopwatch.StartNew();
      var result = service.ValidateMods(mods);
      sw.Stop();

      // Assert performance
      Assert.True(sw.ElapsedMilliseconds < 100,
        $"Validation took {sw.ElapsedMilliseconds}ms, expected < 100ms");

      // Verify validation still works
      Assert.True(result.IsValid);
      Assert.NotEmpty(result.SortedMods);
    }
  }
}
