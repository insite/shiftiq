using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InSite.Application.Standards.Read
{
    public interface IStandardSearch
    {
        bool Exists(Guid organizationId, int assetNumber);
        QStandard GetStandard(Guid standardId);
        int CountStandards(QStandardFilter filter);
        List<QStandard> GetStandards(QStandardFilter filter, params Expression<Func<QStandard, object>>[] includes);
        QStandardCategory GetStandardCategory(Guid standardId, Guid categoryId);
        QStandardConnection GetStandardConnection(Guid fromStandardId, Guid toStandardId);
        List<QStandardConnection> GetAllStandardConnections();
        QStandardContainment GetStandardContainment(Guid parentStandardId, Guid childStandardId);
        List<QStandardContainment> GetAllStandardContainments();
        QStandardOrganization GetStandardOrganization(Guid standardId, Guid organizationId);
        QStandardAchievement GetStandardAchievement(Guid standardId, Guid achievementId);
        QStandardGroup GetStandardGroup(Guid standardId, Guid groupId);
        int CountStandardCategories(Guid standardId);
        int CountStandardConnections(Guid fromStandardId);
        int CountStandardContainments(Guid parentStandardId);
        int CountStandardOrganizations(Guid standardId);
        int CountStandardAchievements(Guid standardId);
        int CountStandardGroups(Guid standardId);
    }
}
