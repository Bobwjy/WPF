using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easi365UI.Models.SkyDrive
{
    public interface ISkyDriveModelCache
    {
        SkyDriveItemModel RegisterModel(SkyDriveItemModel model);
        void RegisterChildModels(SkyDriveItemModel parentModel, SkyDriveItemModel[] childModels);
        SkyDriveItemModel[] GetChildModel(SkyDriveItemModel parentModel);

        string GetUniqueId(string path);
        string GetPath(string uniqueId);
        SkyDriveItemModel GetModel(string uniqueId);
    }

    class SkyDriveModelCache
    {
    }
}
