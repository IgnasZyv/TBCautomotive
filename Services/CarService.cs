using System.Diagnostics;
using CarHostingWeb.Models;
using Google.Cloud.Firestore;

namespace CarHostingWeb.Services;

public class CarService()
{
    private readonly CollectionReference? _carsCollection;


    public CarService(FirestoreDb firestoreDb) : this()
    {
        this._carsCollection = firestoreDb.Collection("cars");
    }
    
    public async Task<List<Car>> GetAllCarsAsync()
    {
        var cars = new List<Car>();

        Debug.Assert(_carsCollection != null, nameof(_carsCollection) + " != null");
        var snapshot = await _carsCollection.GetSnapshotAsync();
        
        foreach (var doc in snapshot.Documents)
        {
            var car = doc.ConvertTo<Car>();
            cars.Add(car);
        }

        return cars;
    }

    public async Task<Car?> GetCarByIdAsync(string carId)
    {
        Debug.Assert(_carsCollection != null, nameof(_carsCollection) + " != null");
        
        var docRef = _carsCollection.Document(carId);
        var snapshot = await docRef.GetSnapshotAsync();

        return snapshot.Exists ? snapshot.ConvertTo<Car>() : null;
    }

    public async Task UpdateCarAsync(string carId, Car updatedCar)
    {
        Debug.Assert(_carsCollection != null, nameof(_carsCollection) + " != null");

        var docRef = _carsCollection.Document(carId);
        await docRef.SetAsync(updatedCar, SetOptions.MergeAll);
    }
    
    

    public async Task<string> CreateCarAsync(Car car)
    {
        Debug.Assert(_carsCollection != null, nameof(_carsCollection) + " != null");

        var docRef = _carsCollection.Document();
        await docRef.SetAsync(car);
        return docRef.Id;
    }

    public async Task<bool> DeleteCarAsync(string carId)
    {
        try
        {
            Debug.Assert(_carsCollection != null, nameof(_carsCollection) + " != null");

            if (string.IsNullOrEmpty(carId))
                throw new ArgumentException(@"Car ID cannot be null or empty", nameof(carId));


            var docRef = _carsCollection.Document(carId);
            await docRef.DeleteAsync();

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(@$"Error deleting car {carId}: {e.Message}");
            return false;
        }

    }
    
}

