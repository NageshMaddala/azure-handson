using System.ComponentModel.DataAnnotations;

namespace SampleBlobProject.Models;
public class Container
{
    [Required]
    public string Name { get; set; }
}