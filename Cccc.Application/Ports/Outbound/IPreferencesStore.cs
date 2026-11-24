namespace Cccc.Application
{
    public interface IPreferencesStore
    {
        PreferencesLoadResult Load();
        PreferencesSaveResult Save(PreferencesModel model);
        DeleteResult Delete();
    }
}
