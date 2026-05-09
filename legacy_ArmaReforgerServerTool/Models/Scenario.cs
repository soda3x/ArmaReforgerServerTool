/******************************************************************************
 * File Name:    Scenario.cs
 * Project:      Longbow
 * Description:  This file contains the Scenario class which represents
 *               a default Arma Reforger scenario with a name and path
 *               for ease of use for the end-user
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace Longbow.Models
{
  internal class Scenario
  {

    /// <summary>
    /// The scenario's friendly name
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The scenario's ".conf" path
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Constructs a Scenario with a name and path
    /// </summary>
    /// <param name="name"></param>
    /// <param name="path"></param>
    public Scenario(string name, string path)
    {
      this.Name = name;
      this.Path = path;
    }

    public override string ToString()
    {
      return $"{Name} // {Path}";
    }
  }
}
