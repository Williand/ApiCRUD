namespace Person.Models
{
    public class PersonModel
    {
        public Guid Id { get; init; }
        public string?  Name { get; private set; }

        public PersonModel(string? name)
        {
            Name = name;
            Id = Guid.NewGuid();
        }

        public void ChangeName(string name)
        {
            Name = name;
        }

        public void SetInactive()
        {
            Name = "Desativado";
        }
    }
}
