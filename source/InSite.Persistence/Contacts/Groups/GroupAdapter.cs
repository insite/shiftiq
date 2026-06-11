using InSite.Application.Contacts.Read;

using Shift.Common;

using GroupModel = InSite.Domain.Foundations.Group;
using GroupTypeEnum = Shift.Constant.GroupType;

namespace InSite.Persistence
{
    public static class GroupAdapter
    {
        public static GroupModel CreateGroupPacket(QGroup group)
        {
            var packet = new GroupModel
            {
                Identifier = group.GroupIdentifier,
                Type = group.GroupType.ToEnum<GroupTypeEnum>(),
                Name = group.GroupName
            };

            return packet;
        }

        public static GroupModel CreateGroupPacket(VGroupDetail group)
        {
            var packet = new GroupModel
            {
                Identifier = group.GroupIdentifier,
                Type = group.GroupType.ToEnum<GroupTypeEnum>(),
                Name = group.GroupName
            };

            return packet;
        }
    }
}