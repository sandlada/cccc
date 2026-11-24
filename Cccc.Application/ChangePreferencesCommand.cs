using System;

namespace Cccc.Application
{
    public sealed class ChangePreferencesCommand
    {
        public bool? ShowPreview;
        public bool? UseMaterial;
        public LayoutMode? Layout;
    }
}
