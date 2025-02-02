namespace BlApi;
public static class Factory
{
    //Return an initialized object of a type that implements the IBl interface.
    public static IBl Get() => new BlImplementation.Bl();
}
