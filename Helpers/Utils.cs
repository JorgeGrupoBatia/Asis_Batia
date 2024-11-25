namespace Asis_Batia.Helpers;

public static class Utils {

    public static bool IsConnectedInternet() {
        return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }
}