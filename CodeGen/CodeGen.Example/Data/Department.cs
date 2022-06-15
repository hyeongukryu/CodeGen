namespace CodeGen.Example.Data;

public class Department
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public Person[] People { get; set; } = Array.Empty<Person>();
}