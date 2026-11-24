using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cccc.Application;

namespace Cccc.Application.Tests
{
    internal sealed class MemoryPreferencesStore : IPreferencesStore
    {
        public PreferencesLoadResult LoadResult = new();
        public PreferencesSaveResult SaveResult = new() { Success = true };
        public DeleteResult DeleteResult = new() { Success = true };
        public PreferencesModel Saved;
        public int SaveCount;
        public int DeleteCount;

        public PreferencesLoadResult Load() => LoadResult;

        public PreferencesSaveResult Save(PreferencesModel model)
        {
            SaveCount++;
            Saved = model;
            return SaveResult;
        }

        public DeleteResult Delete()
        {
            DeleteCount++;
            return DeleteResult;
        }
    }
}
