using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class Enviament : INotifyPropertyChanged
{
    [BsonId]
    public ObjectId Id { get; set; }

    private string _nom;
    public string nom
    {
        get => _nom;
        set { _nom = value; OnPropertyChanged(); }
    }

    private double _preuBase;
    public double preu_base
    {
        get => _preuBase;
        set { _preuBase = value; OnPropertyChanged(); }
    }

    private double _preuMinimGratuit;
    public double preu_minim_gratuit
    {
        get => _preuMinimGratuit;
        set { _preuMinimGratuit = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return $"{nom} - {(preu_base > 0 ? preu_base.ToString("C2") : "Gratis")}";
    }
}