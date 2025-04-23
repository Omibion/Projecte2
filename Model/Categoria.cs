using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class Categoria : INotifyPropertyChanged
{
    public ObjectId Id { get; set; }
    public string Nom { get; set; }
    public ObjectId? IdCategoriaPare { get; set; }

    [BsonIgnore]
    public List<Categoria> Subcategories { get; set; } = new List<Categoria>();

    public string Descripcio { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public List<Categoria> GetSubcategories()
    {
        return Subcategories;
    }

   
}